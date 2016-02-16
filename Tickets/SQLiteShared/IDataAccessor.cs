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
        List<TModel> GetModelsList<TModel>(String filter = null) where TModel : class;

        List<TResult> GetObjectsList<TModel, TResult>(Expression<Func<TModel, TResult>> field, String filter) where TModel : class;
    }
}
