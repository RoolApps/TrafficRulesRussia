using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using SQLite.Net;
using SQLite.Net.Platform.WinRT;
using System.Linq.Expressions;

namespace SQLiteShared
{
    public static class Accessor
    {
        #region Private Members
        private const String DBFileName = "tickets.db";
        private const String SelectObjectQueryPattern = "select {0} from {1}";
        private const String SelectModelQueryPattern = "select * from {0}";
        private const String FilterQueryPattern = " where {0}";
        private static readonly StorageFolder LocalFolder = ApplicationData.Current.LocalFolder;
        private static readonly StorageFolder InstalledLocationFolder = Package.Current.InstalledLocation;
        #endregion

        #region Public Methods
        public static void InitDB()
        {
            CheckDBFile().ContinueWith(async (result) =>
            {
                bool exists = await result;
                if (!exists)
                {
                    await CopyDBFile();
                }
            });
        }

        public static List<TModel> GetModelsList<TModel>(String filter = null)
            where TModel: Models.BaseModel
        {
            List<TModel> result = null;
            Query(connection =>
            {
                String query = String.Format(SelectModelQueryPattern, typeof(TModel).Name);
                if(!String.IsNullOrEmpty(filter))
                {
                    query += String.Format(FilterQueryPattern, filter);
                }
                result = connection.Query<TModel>(query);
            });
            return result;
        }

        public static List<TResult> GetObjectsList<TModel, TResult>(Expression<Func<TModel, TResult>> field,  String filter = null)
            where TModel : Models.BaseModel
        {
            List<TResult> result = new List<TResult>();
            Query(connection =>
            {
                var fieldName = (field.Body as MemberExpression).Member.Name;
                String query = String.Format(SelectObjectQueryPattern, fieldName, typeof(TModel).Name);
                if(!String.IsNullOrEmpty(filter))
                {
                    query += String.Format(FilterQueryPattern, filter);
                }
                var models = connection.Query<TModel>(query);
                foreach(var model in models)
                {
                    result.Add(field.Compile().Invoke(model));
                }
            });
            return result;
        }
        #endregion

        #region Private Methods
        private static void Query(Action<SQLiteConnection> query)
        {
            using (var connection = new SQLiteConnection(new SQLitePlatformWinRT(), Path.Combine(LocalFolder.Path, DBFileName)))
            {
                query(connection);
            }
        }

        private static async Task<bool> CheckDBFile()
        {
            try
            {
                await LocalFolder.GetFileAsync(DBFileName);
                return true;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
        }

        private static async Task<bool> CopyDBFile()
        {
            try
            {
                var dbFile = await InstalledLocationFolder.GetFileAsync(DBFileName);
                await dbFile.CopyAsync(LocalFolder);
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }
        #endregion
    }
}
