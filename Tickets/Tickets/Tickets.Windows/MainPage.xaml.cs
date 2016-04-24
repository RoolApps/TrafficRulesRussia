using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using XAMLMarkup;
using Utils;
using AppLogic;
using AppLogic.Enums;
using AppLogic.Interfaces;

namespace Tickets
{
    public sealed partial class MainPage : Page
    {
        private string question = "question";
        public string Question
        {
            get
            {
                return question;
            }
            set
            {
                question = value;
            }
        }

        public IEnumerable<Answer> testList = new Answer[] {
            new Answer("LOL1321321"),
            new Answer("Lol2"),
            new Answer("LOL3213213213"),
        };

        public IEnumerable<Answer> TestList
        {
            get
            {
                return testList;
            }
        }

        public IEnumerable<string> testList2 = new string[] {
            "LOL1",
            "Lol2",
            "LOL3",
        };

        public IEnumerable<string> TestList2
        {
            get
            {
                return testList2;
            }
        }

        ISession session;
        PagedCanvas paged_canvas = null;


        class sp : ISessionParameters
        {
            public bool Shuffle
            {
                get;
                set;
            }

            public int[] TicketNums
            {
                get;
                set;
            }

            public QuestionsGenerationMode Mode
            {
                get;
                set;
            }
        }

        #region Constructor
        public MainPage()
        {
            sp _sp = new sp {
                Mode = QuestionsGenerationMode.RandomTicket
            };

            var sf = SessionFactory.CreateSession(_sp, out session);

            System.Diagnostics.Debug.WriteLine(sf);

            
            this.InitializeComponent();

            paged_canvas = flipping_canvas.CanvasContent as PagedCanvas;
            PagedCollection<IQuestion> paged_col = new PagedCollection<IQuestion>(0);
            paged_col.DataSource = session.Questions;
            paged_canvas.DataSource = paged_col;
            //DataContext = this;
            
        }
        #endregion

        /*
        #region Constructor
        public MainPage()
        {
            
            this.InitializeComponent();
            foreach (var v in TestList) {
                System.Diagnostics.Debug.WriteLine(v.Text);
            }
            DataContext = this;
        }
        #endregion
         * */
    }

    public class Answer
    {
        private string text;
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
            }
        }

        public Answer(string text)
        {
            Text = text;
        }
    }
}
