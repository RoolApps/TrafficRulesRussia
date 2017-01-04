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
using SQLiteShared;
using SQLiteShared.Models;
using Tickets.CommonUI;
using Utils;

namespace Tickets {
    public sealed partial class RulesCategoryPage : Page {
        public RulesCategoryPage() {
            this.InitializeComponent();
        }

        #region Event Handlers
        protected override void OnNavigatedTo( NavigationEventArgs e ) {
            base.OnNavigatedTo(e);
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
            SQLiteDataAccessor sql = new SQLiteDataAccessor();
            var lst = sql.CreateQuery<Chapters>().Select(n=>n.name);
            lstView.ItemsSource = lst;
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

        private void lstView_Tapped( object sender, TappedRoutedEventArgs e ) {
            var index = (sender as ListView).SelectedIndex + 1;
            this.Frame.Navigate(typeof(RulesPage), index);
        }
        #endregion
    }
}
