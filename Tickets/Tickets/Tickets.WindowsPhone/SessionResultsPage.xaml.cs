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
        public SessionResultsPage()
        {
            this.InitializeComponent();

            pivot.ItemsSource = Enumerable.Range(0, 3).Select(t => new Ticket
            {
                Number = t,
                Questions = Enumerable.Range(0, 20).Select(q => new Question
                {
                    Number = q,
                    Text = String.Format("Текст вопроса {0}", q),
                    Answers = Enumerable.Range(0, 3).Select(a => new Answer
                    {
                        Text = String.Format("Ответ {0}", a)
                    }).ToArray()
                }).ToArray()
            }).ToArray();
            
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void semanticZoom_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (args.NewValue != null)
                ((sender as SemanticZoom).ZoomedOutView as ListViewBase).ItemsSource = (args.NewValue as ICollectionView).CollectionGroups;
        }

        class Ticket
        {
            public int Number { get; set; }
            public IEnumerable<Question> Questions { get; set; }

            public override string ToString()
            {
                return String.Format("Билет {0}", Number);
            }
        }

        class Question
        {
            public int Number { get; set; }
            public String Text { get; set; }
            public IEnumerable<Answer> Answers { get; set; }
        }

        class Answer
        {
            public String Text { get; set; }
        }
    }
}
