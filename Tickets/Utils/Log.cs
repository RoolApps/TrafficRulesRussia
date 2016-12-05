using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils {
    public static class Log {
        public static void d( String format, params object[] args) {
            System.Diagnostics.Debug.WriteLine(format, args);
        }

        public static void m<T>( T[] o, String tag = null) {
            int c = 0;
            if(tag != null) {
                Log.d(" ---------- {0} -----------", tag);
            }
            foreach(var v in o) {
                Log.d("c({0}): {1}", c, v);
                c++;
            }
        }
    }
}
