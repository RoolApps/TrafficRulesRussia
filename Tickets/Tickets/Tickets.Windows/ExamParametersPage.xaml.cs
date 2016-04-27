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

namespace Tickets {
    public sealed partial class ExamParametersPage : Page {
        #region private Members
        private ISession session;
        private int[] ticket = new int[1];
        private IList<int> ticketsToFill = new List<int>();
        #endregion

        #region Private Methods
        private void fillTicketGrid() {
            for (int i = 1; i <= 40; i++) {
                ticketsToFill.Add(i);
            }
        }

        private void GoToQuestionPage() {
            throw new NotImplementedException("Not implemented yet");
        }

        private void CreateSession(QuestionsGenerationMode mode, int ticket = 0) {
            SessionParameters sp = new SessionParameters() {
                Mode = mode,
                TicketNums = new int[] { ticket },
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
        private void Button_Click(object sender, RoutedEventArgs e) {
            CreateSession(QuestionsGenerationMode.SelectedTickets, (int)(((Button)sender).Content));
            GoToQuestionPage();
        }

        private void Button_Click_Rnd(object sender, RoutedEventArgs e) {
            CreateSession(QuestionsGenerationMode.RandomTicket);
            GoToQuestionPage();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {

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
