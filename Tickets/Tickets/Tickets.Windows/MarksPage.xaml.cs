using SQLiteShared;
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
using SQLiteShared.Models;
using Tickets.CommonUI;

namespace Tickets {
    public sealed partial class MarksPage : Page {
        private SQLiteDataAccessor sql;
        public MarksPage() {
            this.InitializeComponent();
        }

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

            sql = new SQLiteDataAccessor();
            var signs = sql.CreateQuery<Marks>();
            marksGv.ItemsSource = signs;
            RichTextBlockContent.onBlockTapped += RichTextBlockContent_onBlockTapped;
        }

        protected override void OnNavigatedFrom( NavigationEventArgs e ) {
            base.OnNavigatedFrom(e);
            RichTextBlockContent.onBlockTapped -= RichTextBlockContent_onBlockTapped;
        }

        private void RichTextBlockContent_onBlockTapped( object sender, HLContent e ) {
            switch(e.Type) {
                case "signs": {
                        var v = sql.CreateQuery<Signs>().Where(s => s.num == e.Data).FirstOrDefault();
                        if(v != null) {
                            popupText.Text = v.num;
                            popupRtb.DataContext = v.description;
                            popupImage.DataContext = v.image;
                        }
                        break;
                    }
                case "marks": {
                        var v = sql.CreateQuery<Marks>().Where(s => s.num == e.Data).FirstOrDefault();
                        if(v != null) {
                            popupText.Text = v.num;
                            popupRtb.DataContext = v.description;
                            popupImage.DataContext = v.image;
                        }
                        break;
                    }
                default:
                    break;
            }
            if(!contentPopup.IsOpen) {
                contentPopup.IsOpen = true;
            }
        }

        private void setPopupContent(Marks mark) {
            popupText.Text = mark.num;
            popupRtb.DataContext = mark.description;
            popupImage.DataContext = mark.image;
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

        private void Grid_Tapped( object sender, TappedRoutedEventArgs e ) {
            var gridView = sender as GridView;
            if(gridView == null) {
                return;
            }
            int index = gridView.SelectedIndex + 1;
            var v = sql.CreateQuery<Marks>().Where(s => s.id == index).FirstOrDefault();
            setPopupContent(v);
            if(!contentPopup.IsOpen) {
                contentPopup.IsOpen = true;
            }
        }
    }
}
