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
    public class SQLiteDataAccessor : IDataAccessor
    {
        #region Private Members
        private const String SelectObjectQueryPattern = "select {0} from {1}";
        private const String SelectModelQueryPattern = "select * from {0}";
        private const String FilterQueryPattern = " where {0}";
        private readonly SQLiteConnection Connection = null;
        #endregion

        #region Constructor
        public SQLiteDataAccessor()
        {
            Connection = new SQLiteConnection(new SQLitePlatformWinRT(), AppData.Resources.ConnectionString);
        }
        #endregion

        #region Public Methods
        public List<TModel> GetModelsList<TModel>(String filter = null)
            where TModel : class
        {
            List<TModel> result = null;
            String query = String.Format(SelectModelQueryPattern, typeof(TModel).Name);
            if (!String.IsNullOrEmpty(filter))
            {
                query += String.Format(FilterQueryPattern, filter);
            }
            result = Connection.Query<TModel>(query);
            return result;
        }

        public List<TResult> GetObjectsList<TModel, TResult>(Expression<Func<TModel, TResult>> field, String filter = null)
            where TModel : class
        {
            List<TResult> result = new List<TResult>();
            var fieldName = (field.Body as MemberExpression).Member.Name;
            String query = String.Format(SelectObjectQueryPattern, fieldName, typeof(TModel).Name);
            if (!String.IsNullOrEmpty(filter))
            {
                query += String.Format(FilterQueryPattern, filter);
            }
            var models = Connection.Query<TModel>(query);
            foreach (var model in models)
            {
                result.Add(field.Compile().Invoke(model));
            }
            return result;
        }

        public void Dispose()
        {
            Connection.Dispose();
        }
        #endregion
    }
}
