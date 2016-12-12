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
using Utils;

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
            SessionParameters sp = new SessionParameters() {
                Shuffle = shuffleQuestionCB.IsChecked ?? false,
                Mode = mode,
                TicketNums = ticket,
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

        private void Button_Click_Rnd(object sender, RoutedEventArgs e) {
            CreateSession(QuestionsGenerationMode.RandomTicket);
            GoToQuestionPage();
        }

        private void backButton_Click(object sender, RoutedEventArgs e) {
            if ( this.Frame != null && this.Frame.CanGoBack )
                this.Frame.GoBack();
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
            DataContext = this;
        }
        #endregion

    }
}
