using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Utils
{
    public class PropertyHolder : System.ComponentModel.INotifyPropertyChanged
    {
        private object value = true;
        public object Value
        {
            get
            {
                return value;
            }
            set
            {
                if (!value.Equals(this.value))
                {
                    this.value = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("Value"));
                    }
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    }

    public static class PropertyHolderExtensions
    {
        public static PropertyHolder GetPropertyHolder(this ResourceDictionary resourceDictionary, String propertyHolderName)
        {
            return resourceDictionary[propertyHolderName] as PropertyHolder;
        }
    }
}
