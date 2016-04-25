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
using System.Collections;

namespace XAMLMarkup
{
    public sealed partial class QuestionDesign : UserControl
    {
        public static readonly DependencyProperty QuestionProperty =
            DependencyProperty.Register("Question", typeof(string), typeof(QuestionDesign), null);

        public string Question
        {
            get
            {
                return (string)GetValue(QuestionProperty);
            }
            set
            {
                SetValue(QuestionProperty, value);
            }
        }

        public static readonly DependencyProperty AnswersProperty =
            DependencyProperty.Register("Answers", typeof(IEnumerable), typeof(QuestionDesign), null);

        public IEnumerable Answers
        {
            get
            {
                return (IEnumerable)GetValue(AnswersProperty);
            }
            set
            {
                SetValue(AnswersProperty, value);
            }
        }

        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(byte[]), typeof(QuestionDesign), null);

        public byte[] Image
        {
            get
            {
                return (byte[])GetValue(ImageProperty);
            }
            set
            {
                SetValue(ImageProperty, value);
            }
        }

        #region Constructor
        public QuestionDesign()
        {
            this.InitializeComponent();
            (this.Content as FrameworkElement).DataContext = this;
        }
        #endregion
    }
}
