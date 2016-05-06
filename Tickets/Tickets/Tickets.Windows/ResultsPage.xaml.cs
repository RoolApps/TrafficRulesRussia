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
using Utils;
using AppLogic;
using AppLogic.Enums;
using AppLogic.Interfaces;
using Tickets;
using System.Collections.ObjectModel;

namespace Tickets {
    public sealed partial class ResultsPage : Page {
        private ObservableCollection<Ticket> _tickets;
        private ObservableCollection<TicketGroup> _groups;

        #region Constructor
        public ResultsPage() {
            this.InitializeComponent();
            loadTickets();
        }
        #endregion

        private void loadTickets() {
            _tickets = new ObservableCollection<Ticket>();

            List<Question> tmpListOfQuestions = new List<Question>();
            for ( int i = 1; i <= 3; i++ ) {
                TicketGroup tmpGroup = new TicketGroup();
                tmpListOfQuestions.Clear();
                for ( int j = 1; j <= 20; j++ ) {
                    tmpListOfQuestions.Add(new Question {
                        Number = j,
                        Text = "q" + j.ToString(),
                        Image = "i" + j.ToString(),
                        Answers = new List<Answer> { 
                            new Answer { Text = "a", isRight = true }, 
                            new Answer { Text = "b", isRight = false }, 
                            new Answer { Text = "v", isRight = false }
                        },
                        isAnswered = (j % 3 == 0) ? true : false
                    });
                }

                _tickets.Add(new Ticket { Number = i, Questions = tmpListOfQuestions });
            }

            _groups = new ObservableCollection<TicketGroup>();
            var groups = _tickets.OrderBy(x => x.Number).GroupBy(x => x.Number);
            foreach(System.Linq.IGrouping<int, Ticket> item in groups){
                _groups.Add(new TicketGroup{ GroupName = item.Key, Tickets = new ObservableCollection<Ticket>(item)});
            }

            cvsMain.Source = _groups;
            zoomedOutDridView.ItemsSource = _groups;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {

        }

        private void SemanticZoom_ViewChangeStarted(object sender, SemanticZoomViewChangedEventArgs e) {
            if (e.SourceItem.Item == null)
                return;

            e.DestinationItem = new SemanticZoomLocation {
                Item = e.SourceItem.Item
            };
        }

    }


    public class Ticket {
        public int Number { get; set; }
        public List<Question> Questions { get; set; }
    }

    public class Question {
        public int Number { get; set; }
        public string Text { get; set; }
        public List<Answer> Answers { get; set; }
        public string Image { get; set; }
        public Boolean isAnswered { get; set; }
    }

    public class Answer {
        public string Text { get; set; }
        public Boolean isRight { get; set; }
        public Boolean isSelected { get; set; }
    }

    public class TicketGroup {
        public int GroupName { get; set; }
        public ObservableCollection<Ticket> Tickets { get; set; }

        public TicketGroup(){
            Tickets = new ObservableCollection<Ticket>();
        }

        public override string ToString() {
            return "Билет №" + GroupName.ToString();
        }
    }

    public class IsAnsweredQuestion : IValueConverter {
        const String Answered = "Green";
        const String NotAnswered = "Red";

        public object Convert(object value, Type targetType, object parameter, string language) {
            return (bool)value ? Answered : NotAnswered;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }
    }
}
