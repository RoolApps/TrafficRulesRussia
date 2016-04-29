using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using XAMLMarkup;
using Utils;
using AppLogic;
using AppLogic.Enums;
using AppLogic.Interfaces;

namespace Tickets
{
    public sealed partial class MainPage : Page
    {
        #region Constructor
        public MainPage()
        {
            this.InitializeComponent();
        }
        #endregion

        private void Button_Click(object sender, RoutedEventArgs e) {
            Frame root = new Frame();
            root.Navigate(typeof(ExamParametersPage));
            Window.Current.Content = root;
            Window.Current.Activate();
        }
    }
}
