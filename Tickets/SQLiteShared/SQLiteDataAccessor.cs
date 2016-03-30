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
        private SQLiteConnection connection = null;

        private SQLiteConnection Connection
        {
            get
            {
                return connection = connection ?? new SQLiteConnection(new SQLitePlatformWinRT(), AppData.Resources.ConnectionString);
            }
        }
        #endregion

        #region Public Methods
        public IEnumerable<T> CreateQuery<T>() where T: class
        {
            return Connection.Table<T>();
        }

        public void Dispose()
        {
            Connection.Dispose();
        }
        #endregion
    }
}
