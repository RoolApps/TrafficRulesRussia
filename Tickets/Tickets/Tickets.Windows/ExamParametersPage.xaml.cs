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
using AppLogic;
using AppLogic.Enums;
using AppLogic.Interfaces;
using AppLogic.Constants;
using Utils;
using Windows.UI;

namespace Tickets {
    public sealed partial class ExamParametersPage : Page {
        #region private Members
        private const int maxTicketsCount = 40;
        private ISession session;
        private int[] tickets;
        private IList<int> ticketsToFill = new List<int>();
        #endregion

        #region Private Methods
        private void fillTicketGrid() {
            ticketsToFill = Enumerable.Range(1, maxTicketsCount).ToList();
        }

        private void GoToQuestionPage() {
            this.Frame.Navigate(typeof(QuestionsContentPage), Serializer.SerializeToString(session));
        }

        private void CreateSession(QuestionsGenerationMode mode, int[] ticket = null) {
            Random rnd = new Random();
            SessionParameters sp = new SessionParameters() {
                Shuffle = tsShuffleQuestions.IsOn,
                Mode = mode,
                TicketNums = tsRandomTicket.IsOn ? (ticket != null ? new int [] { ticket.ElementAt(rnd.Next(ticket.Count())) } : new int[] { rnd.Next(AppLogic.Constants.GlobalConstants.ticketsCount) })  : (ticket != null ? ticket.OrderBy(t=>t).ToArray() : ticket)
            };
            var sf = SessionFactory.CreateSession(sp, out session);
        }
        #endregion

        #region Public Properties
        public IList<int> TicketsToFill {
            get {
                return ticketsToFill;
            }
            set {
                ticketsToFill = value;
            }
        }
        #endregion

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

        protected override void OnNavigatedFrom( NavigationEventArgs e ) {
        }

        private void Button_Start(object sender, RoutedEventArgs e) {
            if (tickets != null) {
                CreateSession(QuestionsGenerationMode.SelectedTickets, tickets);
            } else {
                CreateSession(QuestionsGenerationMode.RandomTicket);
            }
            GoToQuestionPage();
        }

        private void grdView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var myGridView = sender as GridView;
            if (myGridView == null) {
                return;
            }
            tickets = new int[myGridView.SelectedItems.Count];
            for (int i = 0; i < myGridView.SelectedItems.Count; i++) {
                tickets[i] = (int)(myGridView.SelectedItems[i]);
            }
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

        #endregion

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

        #region Constructor
        public ExamParametersPage() {
            fillTicketGrid();
            this.InitializeComponent();
            startBtn.Tapped += Button_Start;
            DataContext = this;
        }


        #endregion
    }
}
