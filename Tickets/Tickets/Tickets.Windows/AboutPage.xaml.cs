using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Tickets {
    public sealed partial class AboutPage : Page {
        public AboutPage() {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo( NavigationEventArgs e ) {
            if(this.Frame.CanGoBack) {
                BackButton.IsEnabled = true;
            } else {
                BackButton.IsEnabled = false;
            }

            if(this.Frame.CanGoForward) {
                ForwardButton.IsEnabled = true;
            } else {
                ForwardButton.IsEnabled = false;
            }
            base.OnNavigatedTo(e);
        }

        private void AppBarBackButton_Click( object sender, RoutedEventArgs e ) {
            if(this.Frame.CanGoBack) {
                this.Frame.GoBack();
            }
        }

        private void AppBarForwardButton_Click( object sender, RoutedEventArgs e ) {
            if(this.Frame.CanGoForward) {
                this.Frame.GoForward();
            }
        }

        private void AppBarHomeButton_Click( object sender, RoutedEventArgs e ) {
            this.Frame.Navigate(typeof(MainPage));
        }
    }
}
