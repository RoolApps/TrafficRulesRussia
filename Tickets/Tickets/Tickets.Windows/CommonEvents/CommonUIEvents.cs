using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Tickets.CommonEvents {
    public static class CommonUIEvents {
        public static void Popup_Loaded( object sender, RoutedEventArgs e ) {
            var pup = sender as Popup;
            if(pup != null) {
                var margin = pup.Margin;
                margin.Left = Window.Current.Bounds.Width / 8;
                margin.Top = 50;
                pup.Margin = margin;
            }
        }

        public static void PopUpContent_Loaded( object sender, RoutedEventArgs e ) {
            var b = sender as Border;
            if(b != null) {
                b.Width = Window.Current.Bounds.Width - (Window.Current.Bounds.Width / 4);
                b.MaxHeight = Window.Current.Bounds.Height - 175;
            }
        }
    }
}
