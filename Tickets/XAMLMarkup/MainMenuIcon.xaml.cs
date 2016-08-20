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

namespace XAMLMarkup {
    public sealed partial class MainMenuIcon : UserControl {
        private const double translucentValue = 0.6;

        public MainMenuIcon() {
            MakeTranslucent = true;
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

        // VisibleProperty
        public static readonly DependencyProperty OpacityProperty =
            DependencyProperty.Register("MakeTranslucent", typeof(Boolean), typeof(MainMenuIcon), null);

        public Boolean MakeTranslucent {
            get {
                return (Boolean)GetValue(OpacityProperty);
            }
            set {
                SetValue(OpacityProperty, value);
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
        //

        private void Image_PointerEntered( object sender, PointerRoutedEventArgs e ) {
            if(sender as Image != null) {
                (sender as Image).Opacity = 1;
            }
        }

        private void Image_PointerExited( object sender, PointerRoutedEventArgs e ) {
            if(sender as Image != null) {
                if(MakeTranslucent == true) {
                    (sender as Image).Opacity = translucentValue;
                }
            }
        }

        private void Image_PointerPressed( object sender, PointerRoutedEventArgs e ) {
            if(sender as Image != null) {
                (sender as Image).Height = (sender as Image).Height - 20;
                (sender as Image).Width = (sender as Image).Width - 20;
            }
        }

        private void Image_PointerReleased( object sender, PointerRoutedEventArgs e ) {
            if(sender as Image != null) {
                (sender as Image).Height = (sender as Image).Height + 20;
                (sender as Image).Width = (sender as Image).Width + 20;
            }
        }

        private void Image_Loaded( object sender, RoutedEventArgs e ) {
            if(sender as Image != null) {
                if(MakeTranslucent == true) {
                    (sender as Image).Opacity = translucentValue;
                }
            }
        }

    }
}
