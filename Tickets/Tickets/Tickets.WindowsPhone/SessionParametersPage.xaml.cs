using AppLogic.Interfaces;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Tickets
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SessionParametersPage : Page
    {
        SessionParameters Parameters = new SessionParameters();

        IEnumerable<TicketPresenter> Tickets;

        public SessionParametersPage()
        {
            this.InitializeComponent();
            GetTicketIds();
            listTickets.ItemsSource = Tickets;
            listTickets.SelectionChanged += listTickets_SelectionChanged;
            EnableControls();
        }

        private void GetTicketIds()
        {
            using(var dataAccessor = new SQLiteShared.SQLiteDataAccessor())
            {
                Tickets = dataAccessor.CreateQuery<SQLiteShared.Models.Tickets>().Select(ticket => new TicketPresenter(ticket)).ToArray();
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void chkRandomTicket_Checked(object sender, RoutedEventArgs e)
        {
            Parameters.Mode = AppLogic.Enums.QuestionsGenerationMode.RandomTicket;
            EnableControls();
        }

        private void chkRandomTicket_Unchecked(object sender, RoutedEventArgs e)
        {
            Parameters.Mode = AppLogic.Enums.QuestionsGenerationMode.SelectedTickets;
            EnableControls();
        }

        void listTickets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EnableControls();
        }

        private void EnableControls()
        {
            if(Parameters.Mode == AppLogic.Enums.QuestionsGenerationMode.RandomTicket)
            {
                this.listTickets.IsEnabled = false;
                this.btnStart.IsEnabled = true;
            }
            else
            {
                this.listTickets.IsEnabled = true;
                if(listTickets.SelectedItems.Any())
                {
                    this.btnStart.IsEnabled = true;
                }
                else
                {
                    this.btnStart.IsEnabled = false;
                }
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {

        }

        class TicketPresenter
        {
            private SQLiteShared.Models.Tickets ticket;

            public TicketPresenter(SQLiteShared.Models.Tickets ticket)
            {
                this.ticket = ticket;
            }

            public int Id { get { return ticket.id; } }

            public string Num { get { return String.Format("Билет № {0}", ticket.num); } }
        }

        class SessionParameters : ISessionParameters
        {
            public SessionParameters()
            {
                this.Mode = AppLogic.Enums.QuestionsGenerationMode.SelectedTickets;
            }

            public bool Shuffle
            {
                get { throw new NotImplementedException(); }
            }

            public int[] TicketNums
            {
                get { throw new NotImplementedException(); }
            }

            public AppLogic.Enums.QuestionsGenerationMode Mode { get; set; }
        }
    }
}
