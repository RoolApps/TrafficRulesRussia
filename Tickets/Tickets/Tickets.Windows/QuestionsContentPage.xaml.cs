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
using XAMLMarkup.EventHandlers;
using Utils;
using AppLogic;
using AppLogic.Enums;
using AppLogic.Interfaces;
using Tickets;
using Windows.UI;
using Windows.UI.Xaml.Media.Animation;
using System.Collections.ObjectModel;

namespace Tickets
{
    public sealed partial class QuestionsContentPage : Page {
        #region private Members
        private ISession session;
        private PagedCanvas pagedCanvas;
        private const int tabletFontSize = 28;
        private const int desktopFontSize = 34;
        #endregion

        #region Public Members
        static public int[] QuestionCount;
        #endregion

        #region Event Handlers
        protected override async void OnNavigatedTo(NavigationEventArgs e) {
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

            if(e.NavigationMode != NavigationMode.New) {
                string sessionState = await SettingSaver.GetSettingFromFile(GlobalConstants.sesstionState);
                session = Serializer.DeserializeFromString<Session>(sessionState);
            } else {
                session = Serializer.DeserializeFromString<Session>(e.Parameter as string);
            }
            pagedCanvas = flipping_canvas.Children.OfType<PagedCanvas>().Single();
            PagedCollection<IQuestion> paged_col = new PagedCollection<IQuestion>(2);
            paged_col.DataSource = session.Tickets.SelectMany(ticket => ticket.Questions);
            pagedCanvas.ItemsSource = paged_col;

            appbarText.ItemsSource = new ObservableCollection<ISession> { session };

            base.OnNavigatedTo(e);
        }

        private void flipping_canvas_OnCompleted(object sender, OnFlipCompleted e) {
            if ( pagedCanvas != null ) {
                if ( e.Direction == MoveDirection.ToNext ) {
                    pagedCanvas.LoadNext();
                } else if ( e.Direction == MoveDirection.ToPrevious ) {
                    pagedCanvas.LoadPrevious();
                }
            }
        }

        protected override async void OnNavigatedFrom( NavigationEventArgs e ) {
            await SettingSaver.SaveSettingToFile(GlobalConstants.sesstionState, Serializer.SerializeToString(session));
        }

        private void Grid_Tapped( object sender, TappedRoutedEventArgs e ) {
            IAnswer answer = e.OriginalSource as Grid != null ? (e.OriginalSource as Grid).DataContext as IAnswer : (e.OriginalSource as TextBlock != null ? (e.OriginalSource as TextBlock).DataContext as IAnswer : null);
            if(answer != null) {
                answer.IsSelected = !answer.IsSelected;
                Boolean allQuestionsIsAnswered = session.Tickets.SelectMany(ticket => ticket.Questions).All(question => question.SelectedAnswered != null);
                if(allQuestionsIsAnswered) {
                    this.Frame.Navigate(typeof(ResultsPage), Serializer.SerializeToString(session));
                }
                flipping_canvas.SlideCanvas(MoveDirection.ToNext);
            }
        }

        #endregion

        #region Constructor
        public QuestionsContentPage() {
            this.InitializeComponent();
            flipping_canvas.OnCompleted += flipping_canvas_OnCompleted;
            
        }
        #endregion

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

        private void NextQuestionTapped( object sender, TappedRoutedEventArgs e ) {
            flipping_canvas.SlideCanvas(MoveDirection.ToNext);
        }

        private void PreviousQuestionTapped( object sender, TappedRoutedEventArgs e ) {
            flipping_canvas.SlideCanvas(MoveDirection.ToPrevious);
        }

        private void Border_PointerExited( object sender, PointerRoutedEventArgs e ) {
            Image image = (sender as Border).Child as Image;
            if(image != null) {
                image.Opacity = Tickets.Global.GlobalConstants.translucentValue;
                (sender as Border).Opacity = Tickets.Global.GlobalConstants.translucentValue;
            }
        }

        private void Border_PointerEntered( object sender, PointerRoutedEventArgs e ) {
            Image image = (sender as Border).Child as Image;
            if(image != null) {
                image.Opacity = 1;
                (sender as Border).Opacity = 1;
            }
        }

        private void Border_Loaded( object sender, RoutedEventArgs e ) {
            Image image = (sender as Border).Child as Image;
            if(image != null && session.Mode == QuestionsGenerationMode.ExamTicket) {
                image.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                (sender as Border).PointerEntered -= Border_PointerEntered;
                (sender as Border).Tapped -= NextQuestionTapped;
                (sender as Border).Tapped -= PreviousQuestionTapped;
            }
        }
        private void NextPageBtn_Click( object sender, RoutedEventArgs e ) {
            this.Frame.Navigate(typeof(ResultsPage), Serializer.SerializeToString(session));
        }

        private void AppBarHomeButton_Click( object sender, RoutedEventArgs e ) {
            this.Frame.Navigate(typeof(MainPage));
        }

        private void ContentGrid_Loaded( object sender, RoutedEventArgs e ) {
            Grid g = sender as Grid;
            if(g != null) {
                g.Height = Window.Current.Bounds.Height - 90;
                g.Width = Window.Current.Bounds.Width - 200;
            }
        }

        private void QuestionGrid_Loaded( object sender, RoutedEventArgs e ) {
            Grid g = sender as Grid;
            if(g != null) {
                g.Width = Window.Current.Bounds.Width / 5;
            }
        }

        private void TextBlock_Loaded( object sender, RoutedEventArgs e ) {
            var tb = sender as TextBlock;
            if(tb != null) {
                if(Window.Current.Bounds.Height <= Tickets.Global.GlobalConstants.tabletheight) {
                    tb.FontSize = tabletFontSize;
                } else {
                    tb.FontSize = desktopFontSize;
                }
            }
        }
    }

    #region Additional Classes
    public class GridBackgroundColorConverter : IValueConverter {
        const String Selected = "#99FFC040";
        const String NotSelected = "Transparent";

        public object Convert(object value, Type targetType, object parameter, string language) {
            return (bool)value ? Selected : NotSelected;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }
    }

    public class TicketToStringConverter : IValueConverter {
        public object Convert( object value, Type targetType, object parameter, string language ) {
            if(value is ISession) {
                if((value as ISession).Mode == QuestionsGenerationMode.ExamTicket) {
                    return String.Format("Экзамен");
                } else {
                    string numbers = "";
                    if((value as ISession).Tickets.Select(t => t.Number).ToList().Count > 1) {
                        numbers = String.Join(", ", (value as ISession).Tickets.Select(t => t.Number).ToList());
                    } else {
                        numbers = (value as ISession).Tickets.Select(t => t.Number).First().ToString();
                    }
                    return String.Format("Билет № {0}", numbers);
                }
            }
            return "";
        }

        public object ConvertBack( object value, Type targetType, object parameter, string language ) {
            throw new NotImplementedException();
        }
    }

    #endregion
}
