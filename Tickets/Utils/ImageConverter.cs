using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Data;

namespace Utils
{
    public class ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            //System.Diagnostics.Debug.WriteLine("value == {0}", value);
            if (value == null || !(value is byte[])) {
                return null;
            }

            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream()) {
                using (DataWriter writer = new DataWriter(stream.GetOutputStreamAt(0))) {
                    writer.WriteBytes((byte[])value);
                    writer.StoreAsync().GetResults();
                }
                BitmapImage image = new BitmapImage();
                image.SetSource(stream);
                return image;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
