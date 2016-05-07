using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Utils
{
    public class LambdaConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var lambda = parameter as Func<object, object>;
            if(lambda == null)
            {
                return null;
            }
            else
            {
                return lambda(value);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
