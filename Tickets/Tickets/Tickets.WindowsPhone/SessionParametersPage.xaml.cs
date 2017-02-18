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
using Utils;
using Utils.Extensions;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Tickets
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SessionParametersPage : Page
    {
        #region Private Members
        IEnumerable<SQLiteShared.Models.Tickets> Tickets;
        #endregion

        public SessionParametersPage()
        {
            this.InitializeComponent();
            InitPage();
        }

        #region Private Methods
        private void InitPage()
        {
            btnShuffleQuestions.Tag = false;

            //fetch ticket ids from database
            using (var dataAccessor = new SQLiteShared.SQLiteDataAccessor())
            {
                Tickets = dataAccessor.CreateQuery<SQLiteShared.Models.Tickets>().ToArray();
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
            //EnableControls();
            foreach(var item in e.AddedItems.Select(item => new { Item = item, Selected = true }).Union(e.RemovedItems.Select(item => new { Item = item, Selected = false })))
            {
                var container = listTickets.ContainerFromItem(item.Item);
                var image = container.FindChildrenOfType<Image>().SingleOrDefault();
                if (image != null)
                {
                    String imageName = null;
                    if(item.Selected)
                    {
                        imageName = "imgCheckChecked.png";
                    }
                    else
                    {
                        imageName = "imgCheckUnchecked.png";
                    }
                    String url = String.Format("ms-appx:///Assets/{0}", imageName);
                    image.Source = new BitmapImage(new Uri(url, UriKind.Absolute));
                }
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            //creating parameters...
            var mode = AppLogic.Enums.QuestionsGenerationMode.SelectedTickets;
            var ticketNums = listTickets.SelectedItems.Cast<SQLiteShared.Models.Tickets>().Select(ticket => ticket.num).ToArray();
            var shuffleChecked = Convert.ToBoolean(btnRandomTicket.Tag);
            var parameters = new SessionParameters(mode, shuffleChecked, ticketNums);
            ISession session;
            //creating session...
            var creationResult = SessionFactory.CreateSession(parameters, out session);
            if(creationResult == AppLogic.Enums.ParametersValidationResult.Valid)
            {
                this.Frame.Navigate(typeof(QuestionPage), Serializer.SerializeToString(session));
            }
            else
            {
                throw new Exception("This should never happen! Developers, please check parameters creation logic");
            }
        }

        private void btnRandomTicket_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var mode = AppLogic.Enums.QuestionsGenerationMode.RandomTicket;
            var parameters = new SessionParameters(mode, false, null);
            ISession session;
            var creationResult = SessionFactory.CreateSession(parameters, out session);
            if(creationResult == AppLogic.Enums.ParametersValidationResult.Valid)
            {
                this.Frame.Navigate(typeof(QuestionPage), Serializer.SerializeToString(session));
            }
            else
            {
                throw new Exception("This should never happen! Developers, please check parameters creation logic (2)");
            }
        }

        private void btnShuffleQuestions_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var button = sender as Button;
            if(sender == null)
            {
                return;
            }

            var value = !Convert.ToBoolean(button.Tag);
            button.Tag = value;
            String imageName = null;

            if(value)
            {
                imageName = "imgCheckChecked.png";
            }
            else
            {
                imageName = "imgCheckUnchecked.png";
            }

            String url = String.Format("ms-appx:///Assets/{0}", imageName);

            var image = button.FindChildrenOfType<Image>().SingleOrDefault();
            if (image != null)
            {
                image.Source = new BitmapImage(new Uri(url, UriKind.Absolute));   
            }
        }
        #endregion
    }
}
