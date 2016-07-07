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
using AppLogic.Interfaces;
using XAMLMarkup;
using Utils;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Tickets
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class QuestionPage : Page
    {
        ISession Session;
        PagedCanvas PagedCanvas;

        public QuestionPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if(e.NavigationMode != NavigationMode.New) {
                string sessionState = await SettingSaver.GetSettingFromFile(GlobalConstants.sesstionState);
                Session = Serializer.DeserializeFromString<Session>(sessionState);
            } else {
                Session = Serializer.DeserializeFromString<Session>(e.Parameter as string);
            }
            var pagedCanvas = flippingCanvas.Children.OfType<PagedCanvas>().Single();
            pagedCanvas.ItemsSource = new PagedCollection<IQuestion>(2) { DataSource = Session.Tickets.SelectMany(ticket => ticket.Questions) };
            flippingCanvas.OnCompleted += flippingCanvas_OnCompleted;
        }

        protected override async void OnNavigatedFrom( NavigationEventArgs e ) {
            await SettingSaver.SaveSettingToFile(GlobalConstants.sesstionState, Serializer.SerializeToString(Session));
        }

        void flippingCanvas_OnCompleted( object sender, XAMLMarkup.EventHandlers.OnFlipCompleted e ) {
            if(e.Direction == XAMLMarkup.Enums.MoveDirection.ToNext) {
                PagedCanvas.LoadNext();
            } else if(e.Direction == XAMLMarkup.Enums.MoveDirection.ToPrevious) {
                PagedCanvas.LoadPrevious();
            }
        }

        private void TextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var element = e.OriginalSource as FrameworkElement;
            var answer = element.DataContext as IAnswer;
            answer.IsSelected = !answer.IsSelected;

            btnEndSession.Visibility = Session.Tickets.SelectMany(ticket => ticket.Questions).All(question => question.SelectedAnswered != null) ? Windows.UI.Xaml.Visibility.Visible : Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void btnEndSession_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SessionResultsPage), Serializer.SerializeToString(Session));
        }
    }

    public class BorderColorConverter : IValueConverter
    {
        const String Selected = "Red";
        const String NotSelected = "White";

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value == null)
            {
                return NotSelected;
            }
            else
            {
                bool selected;
                bool.TryParse(value.ToString(), out selected);
                return selected ? Selected : NotSelected;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
