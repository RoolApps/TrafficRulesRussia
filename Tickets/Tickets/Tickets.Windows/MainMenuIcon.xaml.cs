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
using XAMLMarkup;
using Tickets;
using Utils;
using AppLogic;
using AppLogic.Enums;
using AppLogic.Interfaces;
using System.Collections.ObjectModel;
using Tickets.Global;

namespace Tickets {
    public sealed partial class MainMenuIcon : UserControl {
        public MainMenuIcon() {
            this.InitializeComponent();
            DataContext = this;
        }

        // TextProperty
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(MainMenuIcon), null);

        public string Text {
            get {
                return (string)GetValue(TextProperty);
            }
            set {
                SetValue(TextProperty, value);
            }
        }

        // ImageSourceProperty
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(string), typeof(MainMenuIcon), null);

        public string ImageSource {
            get {
                return (string)GetValue(ImageSourceProperty);
            }
            set {
                SetValue(ImageSourceProperty, value);
            }
        }

        private void Image_PointerEntered( object sender, PointerRoutedEventArgs e ) {
            if(sender as Image != null) {
                (sender as Image).Height = (sender as Image).Height + 20;
                (sender as Image).Width = (sender as Image).Width + 20;
            }
        }

        private void Image_PointerExited( object sender, PointerRoutedEventArgs e ) {
            if(sender as Image != null) {
                (sender as Image).Height = (sender as Image).Height - 20;
                (sender as Image).Width = (sender as Image).Width - 20;
            }
        }

        private void Image_PointerPressed( object sender, PointerRoutedEventArgs e ) {
            if(sender as Image != null) {
                (sender as Image).Opacity = Tickets.Global.GlobalConstants.translucentValue;
            }
        }

        private void Image_PointerReleased( object sender, PointerRoutedEventArgs e ) {
            if(sender as Image != null) {
                (sender as Image).Opacity = 1;
            }
        }
    }
}
