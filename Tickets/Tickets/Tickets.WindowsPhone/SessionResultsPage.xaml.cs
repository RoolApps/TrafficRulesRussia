using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
using AppLogic.Enums;

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
            if(e.NavigationMode != NavigationMode.New) {
                string sessionState = await SettingSaver.GetSettingFromFile(GlobalConstants.sesstionState);
                session = Serializer.DeserializeFromString<Session>(sessionState);
            } else {
                session = Serializer.DeserializeFromString<Session>(e.Parameter as string);
            }
            ISessionStatistics statistics;
            var creationResult = SessionStatisticsFactory.CreateSessionStatistics(session, out statistics);
            gridResults.DataContext = statistics;
        }

        protected override async void OnNavigatedFrom( NavigationEventArgs e ) {
            await SettingSaver.SaveSettingToFile(GlobalConstants.sesstionState, Serializer.SerializeToString(session));
        }

        private void btnFinish_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private void btnMistakes_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //Some hacky hacks used here because of wrong architecture. Very bad code
            var wrongQuestions = session.Tickets.SelectMany(ticket => ticket.Questions.Where(question => question.SelectedAnswered == null || !question.SelectedAnswered.IsRight)).ToArray();
            
            ISession newSession;
            var creationResult = SessionFactory.CreateSession(new SessionParameters(wrongQuestions), out newSession);
            if(creationResult == ParametersValidationResult.Valid)
            {
                this.Frame.Navigate(typeof(QuestionPage), Serializer.SerializeToString(newSession));
            }
        }

        class SessionParameters : ISessionParameters
        {

            public SessionParameters(IEnumerable<IQuestion> questions)
            {
                Questions = questions;
            }

            public bool Shuffle
            {
                get { return false; }
            }

            public IEnumerable<IQuestion> Questions { get; private set; }

            public int[] TicketNums { get { return null; } }

            public QuestionsGenerationMode Mode { get { return QuestionsGenerationMode.Questions; } }
        }
    }
}
