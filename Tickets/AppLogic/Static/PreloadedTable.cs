using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLiteShared.Models;

namespace AppLogic.Static
{
    public class PreloadedTable<T> where T : BaseModel
    {
        private Task loadTask;

        private IEnumerable<T> data;
        public IEnumerable<T> Data
        {
            get
            {
                if (!loadTask.IsCompleted)
                {
                    loadTask.Wait(15 * 1000);
                }
                return data;
            }
            private set
            {
                data = value;
            }
        }

        private void LoadData()
        {
            using (var accessor = new SQLiteShared.SQLiteDataAccessor())
            {
                data = accessor.CreateQuery<T>().ToArray();
            }
        }

        internal PreloadedTable()
        {
            loadTask = new Task(LoadData);
            loadTask.Start();
        }
    }
}
