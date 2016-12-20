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
    public sealed partial class MainPage : Page {

        #region Event Handlers
        protected override void OnNavigatedTo( NavigationEventArgs e ) {
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

        void ExamBtn_Tapped( object sender, TappedRoutedEventArgs e ) {
            SessionParameters sp = new SessionParameters() { Mode = QuestionsGenerationMode.ExamTicket};
            ISession session;
            var sf = SessionFactory.CreateSession(sp, out session);
            this.Frame.Navigate(typeof(QuestionsContentPage), Serializer.SerializeToString(session));
        }

        void ticketBtn_Tapped( object sender, TappedRoutedEventArgs e ) {
            this.Frame.Navigate(typeof(ExamParametersPage));
        }

        void RulesBtn_Tapped( object sender, TappedRoutedEventArgs e ) {
            this.Frame.Navigate(typeof(RulesPage));
        }
        #endregion

        #region Constructor
        public MainPage() {
            this.InitializeComponent();
            ticketsBtn.Tapped += ticketBtn_Tapped;
            ExamBtn.Tapped += ExamBtn_Tapped;
            RulesBtn.Tapped += RulesBtn_Tapped;
        }
        #endregion
    }

    #region Additional Classes
    class SessionParameters : ISessionParameters {
        public bool Shuffle {
            get;
            set;
        }

        public int[] TicketNums {
            get;
            set;
        }

        public QuestionsGenerationMode Mode {
            get;
            set;
        }

        public IEnumerable<IQuestion> Questions {
            get { return null; }
        }
    }
    #endregion
}
