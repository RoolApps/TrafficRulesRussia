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
using AppLogic.Interfaces;
using Utils;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Tickets
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SessionResultsPage : Page
    {
        public SessionResultsPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var session = e.Parameter as ISession;
            pivot.ItemsSource = session.Tickets;
        }

        private void semanticZoom_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (args.NewValue != null)
                ((sender as SemanticZoom).ZoomedOutView as ListViewBase).ItemsSource = (args.NewValue as ICollectionView).CollectionGroups;
        }

        private void semanticZoom_ViewChangeStarted(object sender, SemanticZoomViewChangedEventArgs e)
        {
            (Resources["IsPivotVisible"] as PropertyHolder).Value = !e.IsSourceZoomedInView;
        }

        private void btnMainMenu_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void btnStatistics_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    public class QuestionColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var question = value as IQuestion;
            if(question.SelectedAnswered.IsRight)
            {
                return "Green";
            }
            else
            {
                return "Red";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class AnswerColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var answer = value as IAnswer;
            if(answer.IsRight)
            {
                return "Green";
            }
            else if(answer.IsSelected)
            {
                return "Red";
            } 
            else
            {
                return "White";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
