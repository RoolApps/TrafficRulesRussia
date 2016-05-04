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
    public sealed partial class SessionResultsPage : Page
    {
        public class GroupInfoList<T> : List<object>
        {

            public object Key { get; set; }


            public new IEnumerator<object> GetEnumerator()
            {
                return (System.Collections.Generic.IEnumerator<object>)base.GetEnumerator();
            }
        } 

        class Ticket
        {
            public int Num { get; set; }

            public IEnumerable<Question> Questions { get; set; }

            public override string ToString()
            {
                return Num.ToString();
            }
        }

        class Question
        {
            public int Num { get; set; }

            public IEnumerable<Answer> Answers { get; set; }
        }

        class Answer
        {
            public String Text { get; set; }
        }

        public SessionResultsPage()
        {
            this.InitializeComponent();

            this.pivot.ItemsSource = Enumerable.Range(1, 40).Select(i => new Ticket { Num = i, Questions = Enumerable.Range(1, 20).Select(j => new Question { Num = j, Answers = Enumerable.Range(1, 3).Select(h => new Answer { Text = String.Format("Answer {0}", h) }) }) });
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void SemanticZoom_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            (sender as SemanticZoom).ToggleActiveView();
        }
    }
}
