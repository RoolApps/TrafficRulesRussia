using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Utils.Extensions
{
    public static class PanelExtensions
    {
        public static IEnumerable<FrameworkElement> AllChildren(this Panel panel)
        {
            return panel.Children.OfType<FrameworkElement>().Union(panel.Children.OfType<Panel>().SelectMany(child => AllChildren(child)));
        }
    }
}
