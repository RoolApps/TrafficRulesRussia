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
using AppLogic;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Tickets
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SessionResultsPage : Page
    {
        #region private Members
        private ISession session;
        #endregion

        public SessionResultsPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            InitResources();
            if(e.NavigationMode != NavigationMode.New) {
                string sessionState = await SettingSaver.GetSettingFromFile(GlobalConstants.sesstionState);
                session = Serializer.DeserializeFromString<Session>(sessionState);
            } else {
                session = Serializer.DeserializeFromString<Session>(e.Parameter as string);
            }
            pivot.ItemsSource = session.Tickets;
            ISessionStatistics statistics;
            var creationResult = SessionStatisticsFactory.CreateSessionStatistics(session, out statistics);
            if(creationResult != AppLogic.Enums.SessionValidationResult.Valid)
            {
                btnStatistics.IsEnabled = false;
            }
            else
            {
                popupStatistics.DataContext = statistics;
            }
        }

        protected override async void OnNavigatedFrom( NavigationEventArgs e ) {
            await SettingSaver.SaveSettingToFile(GlobalConstants.sesstionState, Serializer.SerializeToString(session));
        }

        #region EventHandlers
        private void semanticZoom_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (args.NewValue != null)
            {
                ((sender as SemanticZoom).ZoomedOutView as ListViewBase).ItemsSource = (args.NewValue as ICollectionView).CollectionGroups;
            }
        }

        private void semanticZoom_ViewChangeStarted(object sender, SemanticZoomViewChangedEventArgs e)
        {
            Resources.GetPropertyHolder("IsPivotVisible").Value = !e.IsSourceZoomedInView;
        }

        private void btnMainMenu_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void btnStatistics_Click(object sender, RoutedEventArgs e)
        {
            popupStatistics.IsOpen = true;
        }
        #endregion

        #region Private Methods
        private void InitResources()
        {
            Resources.GetPropertyHolder("QuestionColorLambda").Value = new Func<object, object>(question =>
            {
                var iquestion = question as IQuestion;
                if (iquestion.SelectedAnswered.IsRight)
                {
                    return "Green";
                }
                else
                {
                    return "Red";
                }
            });

            Resources.GetPropertyHolder("AnswerColorLambda").Value = new Func<object, object>(answer =>
            {
                var ianswer = answer as IAnswer;
                if (ianswer.IsRight)
                {
                    return "Green";
                }
                else if (ianswer.IsSelected)
                {
                    return "Red";
                }
                else
                {
                    return "White";
                }
            });
        }
        #endregion
    }
}
