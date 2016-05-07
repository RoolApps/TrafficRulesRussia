using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Utils
{
    public class CollectionViewSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var groupedItemsViewSource = new CollectionViewSource();
            groupedItemsViewSource.IsSourceGrouped = true;
            groupedItemsViewSource.ItemsPath = new PropertyPath(parameter.ToString());
            groupedItemsViewSource.Source = value;
            return groupedItemsViewSource.View;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
