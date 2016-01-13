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

namespace SQLiteShared
{
    public class Access
    {
        private const String DBFileName = "tickets.db";
        private static readonly StorageFolder LocalFolder = ApplicationData.Current.LocalFolder;
        private static readonly StorageFolder InstalledLocationFolder = Package.Current.InstalledLocation;

        private static void Query(Action<SQLiteConnection> query)
        {
            using(var connection = new SQLiteConnection(new SQLitePlatformWinRT(), Path.Combine(LocalFolder.Path, DBFileName)))
            {
                query(connection);
            }
        }

        public static void Connect()
        {
            CheckDBFile().ContinueWith(async (result) =>
            {
                bool exists = await result;
                if (!exists)
                {
                    await CopyDBFile();
                }

                Query(connection =>
                {
                    var answers = connection.Query<Models.Answer>("select * from Answers");
                    answers.ToString();
                });
            });
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

        public static async Task<bool> CopyDBFile()
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
    }
}
