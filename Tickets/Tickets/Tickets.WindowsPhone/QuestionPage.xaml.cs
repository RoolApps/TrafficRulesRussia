﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Input;
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
using Windows.Phone.UI.Input;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Tickets
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class QuestionPage : Page
    {
        ISession Session;

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
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            if(e.NavigationMode != NavigationMode.New) {
                string sessionState = await SettingSaver.GetSettingFromFile(GlobalConstants.sesstionState);
                Session = Serializer.DeserializeFromString<Session>(sessionState);
            } else {
                Session = Serializer.DeserializeFromString<Session>(e.Parameter as string);
            }
            this.mainGrid.Resources.GetPropertyHolder("sessionMode").Value = Session.Mode;
            if(Session.Mode == AppLogic.Enums.QuestionsGenerationMode.ExamTicket)
            {
                questionsCanvas.ManipulationMode = ManipulationModes.TranslateY;
                btnEndSession.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            if (Session.Mode == AppLogic.Enums.QuestionsGenerationMode.Questions)
            {
                btnEndSession.Content = "Главное меню";
            }
            else
            {
                btnEndSession.Content = "Завершить экзамен";
            }
            int index = 0;
            var questions = Session.Tickets.SelectMany(ticket => ticket.Questions);
            var length = questions.Count();

            questionsCanvas.DataSource = new VirtualLinkedList<IQuestion>(questions,
                (dataSource, current) =>
                {
                    if (index + 1 < length)
                    {
                        index++;
                    }
                    else
                    {
                        index = 0;
                    }
                    return dataSource.ElementAt(index);
                },
                (dataSource, current) =>
                {
                    if (index > 0)
                    {
                        index--;
                    }
                    else
                    {
                        index = length - 1;
                    }
                    return dataSource.ElementAt(index);
                });
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            Frame.Navigate(typeof(MainPage));
        }

        protected override async void OnNavigatedFrom( NavigationEventArgs e ) {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
            await SettingSaver.SaveSettingToFile(GlobalConstants.sesstionState, Serializer.SerializeToString(Session));
        }

        private void TextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (Session.Mode != AppLogic.Enums.QuestionsGenerationMode.Questions)
            {
                var element = e.OriginalSource as FrameworkElement;
                var answer = element.DataContext as IAnswer;
                answer.IsSelected = !answer.IsSelected;

                if (answer.IsSelected)
                {
                    if (Session.Mode == AppLogic.Enums.QuestionsGenerationMode.ExamTicket
                        && Session.Tickets.SelectMany(ticket => ticket.Questions).All(question => question.SelectedAnswered != null))
                    {
                        this.Frame.Navigate(typeof(SessionResultsPage), Serializer.SerializeToString(Session));
                    }
                    else
                    {
                        questionsCanvas.ChangeCanvasContent(true);
                    }
                }
            }
        }

        private void btnEndSession_Click(object sender, RoutedEventArgs e)
        {
            if(Session.Mode == AppLogic.Enums.QuestionsGenerationMode.Questions)
            {
                this.Frame.Navigate(typeof(MainPage));
            }
            else
            {
                this.Frame.Navigate(typeof(SessionResultsPage), Serializer.SerializeToString(Session));
            }
        }
    }

    public class BorderColorConverter : BooleanConverter<String>
    {
        protected override String Selected { get { return "Brown"; } }
        protected override String NotSelected { get { return "Brown"; } }
    }

    public class BorderOpacityConverter : BooleanConverter<double>
    {
        protected override double Selected { get { return 1; } }
        protected override double NotSelected { get { return 1; } }
    }

    public class BorderThicknessConverter : BooleanConverter<Thickness>
    {
        private readonly Thickness selected = new Thickness(3);
        private readonly Thickness notSelected = new Thickness(1);

        protected override Thickness Selected { get { return selected; } }
        protected override Thickness NotSelected { get { return notSelected; } }
    }

    public class BorderBackgroundConverter : IValueConverter
    {
        const string Default = "#F8F6F1";

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool isRight;
            PropertyHolder holder;
            AppLogic.Enums.QuestionsGenerationMode mode;

            if(ChangeType(value, out isRight)
                && ChangeType(parameter, out holder)
                && ChangeType(holder.Value, out mode)
                && mode == AppLogic.Enums.QuestionsGenerationMode.Questions && isRight)
            {
                return "#C6FFBD";
            }
            else
            {
                return Default;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        private bool ChangeType<T>(object obj, out T typed)
        {
            try
            {
                typed = (T)obj;
                return true;
            }
            catch (Exception)
            {
                typed = default(T);
                return false;
            }
        }
    }

    public abstract class BooleanConverter<T> : IValueConverter
    {
        protected abstract T Selected { get; }
        protected abstract T NotSelected { get; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
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
