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
using XAMLMarkup.Enums;
using Utils;
using AppLogic;
using AppLogic.Enums;
using AppLogic.Interfaces;
using Tickets;
using Windows.UI;
using Windows.UI.Xaml.Media.Animation;

namespace Tickets
{
    public sealed partial class QuestionsContentPage : Page {
        #region private Members
        private ISession session;
        private PagedCanvas pagedCanvas;
        #endregion

        #region Event Handlers
        protected override void OnNavigatedTo(NavigationEventArgs e) {
            session = e.Parameter as ISession;
            pagedCanvas = flipping_canvas.Children.OfType<PagedCanvas>().Single();
            PagedCollection<IQuestion> paged_col = new PagedCollection<IQuestion>(2);
            paged_col.DataSource = session.Tickets.SelectMany(ticket => ticket.Questions);
            pagedCanvas.ItemsSource = paged_col;
        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e) {
            TextBlock tb = e.OriginalSource as TextBlock;
            if (tb != null) {
                IAnswer answer = ((tb).DataContext) as IAnswer;
                answer.IsSelected = !answer.IsSelected;
                Boolean allQuestionsIsAnswered = session.Tickets.SelectMany(ticket => ticket.Questions).All(question => question.SelectedAnswered != null);
                if ( allQuestionsIsAnswered ) {
                    flipping_canvas.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    endExamButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                flipping_canvas.SlideCanvas(MoveDirection.ToNext);
            }
        }

        private void endExamButton_Tapped(object sender, TappedRoutedEventArgs e) {
            this.Frame.Navigate(typeof(ResultsPage), session);
        }
        #endregion

        #region Constructor
        public QuestionsContentPage() {
            this.InitializeComponent();
        }
        #endregion
    }

    #region Additional Classes
    public class BorderBackgroundColorConverter : IValueConverter {
        const String Selected = "Gray";
        const String NotSelected = "Black";

        public object Convert(object value, Type targetType, object parameter, string language) {
            return (bool)value ? Selected : NotSelected;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }
    }
    #endregion
}
