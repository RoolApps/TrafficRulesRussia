using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteShared
{
    interface IDataAccessor : IDisposable
    {
        IEnumerable<T> CreateQuery<T>() where T : class;
    }
}
