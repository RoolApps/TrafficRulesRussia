using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
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

namespace Tickets
{
    public sealed partial class MainPage : Page
    {
        List<Question_> Questions = new List<Question_>();
        PagedCanvas paged_canvas = null;

        public MainPage()
        {
            Questions.AddRange(Enumerable.Range(0, 10000).Select(i => new Question_()
            {
                Question = String.Format("Question Q{0}", i),
                Answer1 = String.Format("Answer 1 Q{0}", i),
                Answer2 = String.Format("Answer 2 Q{0}", i),
                Answer3 = String.Format("Answer 3 Q{0}", i)
            }).ToList());

            this.InitializeComponent();

            
            paged_canvas = flipping_canvas.CanvasContent as PagedCanvas;
            PagedCollection<Question_> paged_col = new PagedCollection<Question_>(2);
            paged_col.DataSource = Questions;
            paged_canvas.DataSource = paged_col;
        }
    }

    
   

    class Question_ : INotifyPropertyChanged
    {
        private string question;
        private string answer1;
        private string answer2;
        private string answer3;
        
        public event PropertyChangedEventHandler PropertyChanged;
        
        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        
        public string Question
        {
            get { return question; }
            set { question = value; RaisePropertyChanged("Question"); }
        }

        public string Answer1
        {
            get { return answer1; }
            set { answer1 = value; RaisePropertyChanged("Answer1"); }
        }

        public string Answer2
        {
            get { return answer2; }
            set { answer2 = value; RaisePropertyChanged("Answer2"); }
        }

        public string Answer3
        {
            get { return answer3; }
            set { answer3 = value; RaisePropertyChanged("Answer3"); }
        }
    }
}
