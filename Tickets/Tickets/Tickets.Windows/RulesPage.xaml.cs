using SQLiteShared;
using SQLiteShared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Tickets.CommonUI;
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
    public sealed partial class RulesPage : Page {
        private SQLiteDataAccessor sql;

        public RulesPage() {
            this.InitializeComponent();
            sql = new SQLiteDataAccessor();
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

            int index = (int)e.Parameter;
            var list = sql.CreateQuery<Chapters>();
            Chapters c = list.Where(i => i.id == index).FirstOrDefault();
            if(c != null) {
                header.Text = c.name;
                rtb.DataContext = c.content;
            }
            RichTextBlockContent.onBlockTapped += RichTextBlockContent_onBlockTapped;
        }

        protected override void OnNavigatedFrom( NavigationEventArgs e ) {
            base.OnNavigatedFrom(e);
            RichTextBlockContent.onBlockTapped -= RichTextBlockContent_onBlockTapped;
        }

        private void RichTextBlockContent_onBlockTapped( object sender, HLContent e ) {
            System.Diagnostics.Debug.WriteLine("data: {0}, type: {1}", e.Data, e.Type);
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
    }
}
