using SQLiteShared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Tickets.CommonUI
{
    class CommonObject
    {
        Signs sign = null;
        Marks mark = null;

        public String Num
        {
            get
            {
                return GetFieldValue(f => f.num, f => f.num);
            }
        }

        public byte[] Image
        {
            get
            {
                return GetFieldValue(f => f.image, f => f.image);
            }
        }

        public String Description
        {
            get
            {
                return GetFieldValue(f => f.description, f => f.description);
            }
        }

        public CommonObject(Signs sign)
        {
            this.sign = sign;
        }

        public CommonObject(Marks mark)
        {
            this.mark = mark;
        }

        private T GetFieldValue<T>(Expression<Func<Signs, T>> signPropertyLambda, Expression<Func<Marks, T>> markPropertyLambda)
        {
            if (this.sign != null)
            {
                return signPropertyLambda.Compile()(sign);
            }
            else if (this.mark != null)
            {
                return markPropertyLambda.Compile()(mark);
            }
            else
            {
                return default(T);
            }
        }
    }
}
