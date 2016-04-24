using AppLogic;
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
        #region Private Members
        IEnumerable<TicketPresenter> Tickets;
        #endregion

        public SessionParametersPage()
        {
            this.InitializeComponent();
            InitPage();
        }

        #region Private Methods
        private void InitPage()
        {
            //fetch ticket ids from database
            using (var dataAccessor = new SQLiteShared.SQLiteDataAccessor())
            {
                Tickets = dataAccessor.CreateQuery<SQLiteShared.Models.Tickets>().Select(ticket => new TicketPresenter(ticket)).ToArray();
            }
            //assign them to listview
            listTickets.ItemsSource = Tickets;
            listTickets.SelectionChanged += listTickets_SelectionChanged;
            //disable "start" button
            EnableControls();
        }

        /// <summary>
        /// Method used to change controls behavior
        /// </summary>
        private void EnableControls()
        {
            if (chkRandomTicket.IsChecked == true)
            {
                this.listTickets.IsEnabled = false;
                this.btnStart.IsEnabled = true;
            }
            else
            {
                this.listTickets.IsEnabled = true;
                if (listTickets.SelectedItems.Any())
                {
                    this.btnStart.IsEnabled = true;
                }
                else
                {
                    this.btnStart.IsEnabled = false;
                }
            }
        }
        #endregion Private Methods

        #region Event Handlers
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
            EnableControls();
        }

        private void chkRandomTicket_Unchecked(object sender, RoutedEventArgs e)
        {
            EnableControls();
        }

        private void listTickets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EnableControls();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            //creating parameters...
            var mode = chkRandomTicket.IsChecked == true ? AppLogic.Enums.QuestionsGenerationMode.RandomTicket : AppLogic.Enums.QuestionsGenerationMode.SelectedTickets;
            var ticketNums = listTickets.SelectedItems.Cast<TicketPresenter>().Select(ticket => ticket.Num).ToArray();
            var parameters = new SessionParameters(mode, ticketNums);
            ISession session;
            //creating session...
            var creationResult = SessionFactory.CreateSession(parameters, out session);
            if(creationResult == AppLogic.Enums.ParametersValidationResult.Valid)
            {
                //for future use: this.Frame.NavigateTo(typeof(...));
                throw new NotImplementedException("Not implemented yet");
            }
            else
            {
                throw new Exception("This should never happen! Developers, please check parameters creation logic");
            }
        }
        #endregion

        #region Additional Classes
        /// <summary>
        /// class used to display ticket number with user-friendly caption
        /// </summary>
        class TicketPresenter
        {
            private SQLiteShared.Models.Tickets ticket;

            public TicketPresenter(SQLiteShared.Models.Tickets ticket)
            {
                this.ticket = ticket;
            }

            public int Num { get { return ticket.num; } }

            public string NumString { get { return String.Format("Билет № {0}", ticket.num); } }
        }

        /// <summary>
        /// class used to preserve session parameters
        /// </summary>
        class SessionParameters : ISessionParameters
        {
            public SessionParameters(AppLogic.Enums.QuestionsGenerationMode mode, int[] ticketNums = null)
            {
                this.Mode = mode;
                this.TicketNums = ticketNums;
            }

            public bool Shuffle
            {
                get { throw new NotImplementedException(); }
            }

            public int[] TicketNums { get; private set; }

            public AppLogic.Enums.QuestionsGenerationMode Mode { get; private set; }
        }
        #endregion
    }
}
