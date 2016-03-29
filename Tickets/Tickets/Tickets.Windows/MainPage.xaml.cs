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
            Question_ Q1 = new Question_() {
                Question = "Question Q1",
                Answer1 = "Answer 1 Q1",
                Answer2 = "Answer 2 Q1",
                Answer3 = "Answer 3 Q1",
            };

            Question_ Q2 = new Question_() {
                Question = "Question Q2",
                Answer1 = "Answer 1 Q2",
                Answer2 = "Answer 2 Q2",
                Answer3 = "Answer 3 Q2"
            };

            Question_ Q3 = new Question_() {
                Question = "Question Q3",
                Answer1 = "Answer 1 Q3",
                Answer2 = "Answer 2 Q3",
                Answer3 = "Answer 3 Q3"
            };

            Question_ Q4 = new Question_() {
                Question = "Question Q4",
                Answer1 = "Answer 1 Q4",
                Answer2 = "Answer 2 Q4",
                Answer3 = "Answer 3 Q4"
            };

            //for(int i=0)
            
            
            Questions.Add(Q1);
            Questions.Add(Q2);
            Questions.Add(Q3);
            Questions.Add(Q4);
            
            this.InitializeComponent();

            
            paged_canvas = flipping_canvas.CanvasContent as PagedCanvas;
            //paged_canvas.DataSource = Questions;
            PagedCollection<Question_> paged_col = new PagedCollection<Question_>(1);
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
