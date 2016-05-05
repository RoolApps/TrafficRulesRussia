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
            //var session = e.Parameter as ISession;
            //pivot.ItemsSource = session.Tickets;
            pivot.ItemsSource = Enumerable.Range(0, 40).Select(t => new Ticket
            {
                Number = t,
                Questions = Enumerable.Range(0, 20).Select(q => new Question
                {
                    Number = q,
                    Text = String.Format("Question {0}", q),
                    Answers = Enumerable.Range(0, 3).Select(a => new Answer
                    {
                        Text = String.Format("Answer {0}", a)
                    }).ToArray()
                }).ToArray()
            }).ToArray();
        }

        private void semanticZoom_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (args.NewValue != null)
                ((sender as SemanticZoom).ZoomedOutView as ListViewBase).ItemsSource = (args.NewValue as ICollectionView).CollectionGroups;
        }

        class Ticket
        {
            public int Number { get; set; }
            public IEnumerable<Question> Questions { get; set; }
        }

        class Question
        {
            public int Number { get; set; }
            public String Text { get; set; }
            public IEnumerable<Answer> Answers { get; set; }
        }

        class Answer
        {
            public String Text { get; set; }
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
}
