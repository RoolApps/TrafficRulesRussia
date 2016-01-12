using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

namespace SQLiteShared
{
    public class Access
    {
        private const String DBFileName = "tickets.db";
        private static readonly StorageFolder LocalFolder = ApplicationData.Current.LocalFolder;
        private static readonly StorageFolder InstalledLocationFolder = Package.Current.InstalledLocation;

        public static void Connect()
        {

            CheckDBFile().ContinueWith(async (result) =>
            {
                bool exists = await result;
                if (!exists)
                {
                    await CopyDBFile();
                }

                using (var connection = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), 
                    Path.Combine(LocalFolder.Path, DBFileName)))
                {
                    var answers = connection.Query<Models.Answers>("select * from Answers");
                    answers.ToString();
                }
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
