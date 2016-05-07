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
        private ISession session;

        #region Constructor
        public ResultsPage() {
            this.InitializeComponent();
            //loadTickets();
        }
        #endregion

        private void loadTickets() {
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            session = e.Parameter as ISession;
            

            var tickets = session.Tickets;
            var questions = tickets.SelectMany(q => q.Questions);
            var numbers = questions.Select(n => n.Number);
            var selectedAnswers = questions.Select(a => a.SelectedAnswered);
            int i =0;
            foreach ( var a in selectedAnswers ) {
                if ( a != null ) {
                    i++;
                    System.Diagnostics.Debug.WriteLine("is selected: {0}:{1}", i, a.IsSelected.ToString());
                }
            }
            foreach ( int n in numbers ) {
                System.Diagnostics.Debug.WriteLine("n: {0}", n);
            }

            firstGrid.ItemsSource = session.Tickets;
            
        }

        private void SemanticZoom_ViewChangeStarted(object sender, SemanticZoomViewChangedEventArgs e) {
            if (e.SourceItem.Item == null)
                return;

            e.DestinationItem = new SemanticZoomLocation {
                Item = e.SourceItem.Item
            };
        }

    }

    public class IsAnsweredQuestion : IValueConverter {
        const string Answered = "Green";
        const string NotAnswered = "Red";

        public object Convert(object value, Type targetType, object parameter, string language) {
            string state = NotAnswered;
            var selectedAnswer = (value as IQuestion).SelectedAnswered;
            if ( selectedAnswer != null ) {
                if ( selectedAnswer.IsRight ) {
                    state = Answered;
                } else {
                    state = NotAnswered;
                }
            } else {
                state = "Yellow";
            }
            return state;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }
    }

    public class AnswersConverter : IValueConverter {
        const string Answered = "Green";
        const string NotAnswered = "Red";

        public object Convert(object value, Type targetType, object parameter, string language) {
            string state = NotAnswered;
            var selectedAnswer = (value as IQuestion).SelectedAnswered;
            if ( selectedAnswer != null ) {
                if ( selectedAnswer.IsRight ) {
                    state = Answered;
                } else {
                    state = NotAnswered;
                }
            } else {
                state = "Yellow";
            }
            return state;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }
    }
}
