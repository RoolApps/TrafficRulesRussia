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

        #region public Members
        public static string Answered = "#ff248F40";
        public static string NotAnswered = "#ffBF3330";
        public static string Transparent = "Transparent";
        #endregion

        #region Event Handlers
        protected override void OnNavigatedTo(NavigationEventArgs e) {
            if(this.Frame.CanGoBack) {
                BackButton.IsEnabled = true;
            } else {
                BackButton.IsEnabled = false;
            }

            if(this.Frame.CanGoForward) {
                ForwardButton.IsEnabled = true;
            } else {
                ForwardButton.IsEnabled = false;
            }

            ticket.Add(Serializer.DeserializeFromString<Ticket>(e.Parameter as string));
            cvsMain.Source = ticket;

            base.OnNavigatedTo(e);
        }

        private void AppBarBackButton_Click( object sender, RoutedEventArgs e ) {
            if(this.Frame.CanGoBack) {
                this.Frame.GoBack();
            }
        }

        private void AppBarForwardButton_Click( object sender, RoutedEventArgs e ) {
            if(this.Frame.CanGoForward) {
                this.Frame.GoForward();
            }
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

        private void AppBarHomeButton_Click( object sender, RoutedEventArgs e ) {
            this.Frame.Navigate(typeof(MainPage));
        }
    }

    #region Additional Classes
    public class AnswersConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            string state = QuestionResultPage.Transparent;
            var answer = value as IAnswer;
            if ( answer != null ) {
                if ( answer.IsRight ) {
                    state = QuestionResultPage.Answered;
                }
                if ( answer.IsSelected && !answer.IsRight ) {
                    state = QuestionResultPage.NotAnswered;
                }
            }
            return state;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }
    }

    public class QuestionsConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            string state = QuestionResultPage.Transparent;
            var answer = (value as IQuestion).SelectedAnswered;
            if ( answer != null ) {
                if ( answer.IsRight ) {
                    state = QuestionResultPage.Answered;
                } else {
                    state = QuestionResultPage.NotAnswered;
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
