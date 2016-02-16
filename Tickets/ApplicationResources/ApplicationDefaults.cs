using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace AppData
{
    class ApplicationDefaults
    {
        internal static void InitializeDefaultValues()
        {
            Resources.ConnectionStringDefaultValue = Path.Combine(ApplicationData.Current.LocalFolder.Path, Resources.DBFileNameDefaultValue);
        }
    }
}
