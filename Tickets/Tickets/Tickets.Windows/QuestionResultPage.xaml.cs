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
    public sealed partial class QuestionResultPage : Page {
        #region private Members
        private ObservableCollection<ITicket> ticket;
        #endregion

        #region Event Handlers
        protected override void OnNavigatedTo(NavigationEventArgs e) {
            ticket.Add(Serializer.DeserializeFromString<Ticket>(e.Parameter as string));
            cvsMain.Source = ticket;
        }

        private void backButton_Click(object sender, RoutedEventArgs e) {
            if ( this.Frame != null && this.Frame.CanGoBack )
                this.Frame.GoBack();
        }

        private void SemanticZoom_ViewChangeStarted(object sender, SemanticZoomViewChangedEventArgs e) {
            if ( e.SourceItem.Item == null )
                return;

            e.DestinationItem = new SemanticZoomLocation {
                Item = e.SourceItem.Item
            };
        }
        #endregion

        #region Constructor
        public QuestionResultPage() {
            this.InitializeComponent();
            ticket = new ObservableCollection<ITicket>();
        }
        #endregion
    }

    #region Additional Classes
    public class AnswersConverter : IValueConverter {
        const string Answered = "Green";
        const string NotAnswered = "Red";

        public object Convert(object value, Type targetType, object parameter, string language) {
            string state = "White";
            var answer = value as IAnswer;
            if ( answer != null ) {
                if ( answer.IsRight ) {
                    state = Answered;
                }
                if ( answer.IsSelected && !answer.IsRight ) {
                    state = NotAnswered;
                }
            }
            return state;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }
    }

    public class QuestionsConverter : IValueConverter {
        const string Answered = "Green";
        const string NotAnswered = "Red";

        public object Convert(object value, Type targetType, object parameter, string language) {
            string state = "White";
            var answer = (value as IQuestion).SelectedAnswered;
            if ( answer != null ) {
                if ( answer.IsRight ) {
                    state = Answered;
                } else {
                    state = NotAnswered;
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
