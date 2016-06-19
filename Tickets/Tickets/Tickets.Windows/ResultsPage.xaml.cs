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
using Utils;
using AppLogic;
using AppLogic.Enums;
using AppLogic.Interfaces;
using Tickets;
using System.Collections.ObjectModel;

namespace Tickets {
    public sealed partial class ResultsPage : Page {
        #region private Members
        private ISession session;
        #endregion

        #region Event Handlers
        protected override async void OnNavigatedTo(NavigationEventArgs e) {
            if(e.NavigationMode != NavigationMode.New) {
                string sessionState = await SettingSaver.TakeSettingFromFile("SessionState");
                session = Serializer.DeserializeFromString<Session>(sessionState);
            } else {
                session = Serializer.DeserializeFromString<Session>(e.Parameter as string);
            }
            if ( session == null ) {
                return;
            }
            gridView.ItemsSource = session.Tickets;
        }

        protected override async void OnNavigatedFrom( NavigationEventArgs e ) {
            await SettingSaver.SaveSettingToFile("SessionState", Serializer.SerializeToString(session));
        }

        private void grdView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var myGridView = sender as GridView;
            if ( myGridView == null ) {
                return;
            }
            this.Frame.Navigate(typeof(QuestionResultPage), Serializer.SerializeToString(myGridView.SelectedItem as ITicket));
        }

        private void goHomePage(object sender, RoutedEventArgs e) {
            this.Frame.Navigate(typeof(MainPage));
        }
        #endregion

        #region Constructor
        public ResultsPage() {
            this.InitializeComponent();
        }
        #endregion
    }

    #region Additional Classes
    public class QuestionStateConverter : IValueConverter {
        private const int rightAnswersToPassExam = 18;

        public object Convert(object value, Type targetType, object parameter, string language) {
            string state = "Билет не сдан";
            IEnumerable<IQuestion> questions = (value as ITicket).Questions;
            if ( questions != null ) {
                int rightAnsweredQuestions = questions.Where(q => q.SelectedAnswered != null && q.SelectedAnswered.IsRight).Count();
                if ( rightAnsweredQuestions >= rightAnswersToPassExam ) {
                    state = "Билет сдан";
                }
            }
            return state;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }
    }

    public class QuestionErrorsConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            string state = "";
            IEnumerable<IQuestion> questions = (value as ITicket).Questions;
            if ( questions != null ) {
                int notRightAnsweredQuestions = questions.Where(q => q.SelectedAnswered != null && !q.SelectedAnswered.IsRight).Count();
                state = String.Format("Ошибок: {0}", notRightAnsweredQuestions);
            }
            return state;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }
    }

    public class QuestionBorderConverter : IValueConverter {
        const string Passed = "Green";
        const string NotPassed = "Red";
        private const int rightAnswersToPassExam = 18;

        public object Convert(object value, Type targetType, object parameter, string language) {
            string state = "White";
            IEnumerable<IQuestion> questions = (value as ITicket).Questions;
            if ( questions != null ) {
                int rightAnsweredQuestions = questions.Where(q => q.SelectedAnswered != null && q.SelectedAnswered.IsRight).Count();
                if ( rightAnsweredQuestions >= rightAnswersToPassExam ) {
                    state = Passed;
                } else {
                    state = NotPassed;
                }
            }
            return state;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }
    }
    #endregion
}