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

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace Tickets
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Вызывается перед отображением этой страницы во фрейме.
        /// </summary>
        /// <param name="e">Данные события, описывающие, каким образом была достигнута эта страница.
        /// Этот параметр обычно используется для настройки страницы.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Подготовьте здесь страницу для отображения.

            // TODO: Если приложение содержит несколько страниц, обеспечьте
            // обработку нажатия аппаратной кнопки "Назад", выполнив регистрацию на
            // событие Windows.Phone.UI.Input.HardwareButtons.BackPressed.
            // Если вы используете NavigationHelper, предоставляемый некоторыми шаблонами,
            // данное событие обрабатывается для вас.
        }

        private void imgTickets_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(SessionParametersPage));
        }

        private void imgExam_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var sessionParameters = new SessionParameters(AppLogic.Enums.QuestionsGenerationMode.ExamTicket);
            AppLogic.Interfaces.ISession session;
            var creationResult = AppLogic.SessionFactory.CreateSession(sessionParameters, out session);
            if(creationResult == AppLogic.Enums.ParametersValidationResult.Valid)
            {
                Frame.Navigate(typeof(QuestionPage), Utils.Serializer.SerializeToString(session));
            }
        }

        private void imgRules_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(RuleObjectsPage));
        }

        private void imgSigns_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(SignsMarksPage), "signs");
        }

        private void imgMarks_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(SignsMarksPage), "marks");
        }

        private void imgAbout_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(AboutPage));
        }
    }
}
