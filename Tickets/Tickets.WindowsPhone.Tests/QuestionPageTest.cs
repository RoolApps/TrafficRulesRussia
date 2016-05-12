using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using AppData;
using AppLogic;
using AppLogic.Interfaces;
using XAMLMarkup;

namespace Tickets.WindowsPhone.Tests
{
    [TestClass]
    public class QuestionPageTest
    {
        public class SessionParameters : ISessionParameters
        {

            public bool Shuffle
            {
                get { throw new NotImplementedException(); }
            }

            public int[] TicketNums
            {
                get { throw new NotImplementedException(); }
            }

            public AppLogic.Enums.QuestionsGenerationMode Mode
            {
                get { return AppLogic.Enums.QuestionsGenerationMode.RandomTicket; }
            }
        }

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            Resources.ConnectionString = Path.Combine(Package.Current.InstalledLocation.Path, Resources.DBFileName);
        }

        [TestMethod]
        public async Task QuestionContentFitsToTextBlock()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                var frame = new Frame();
                ISession session;
                var creationResult = SessionFactory.CreateSession(new SessionParameters(), out session);
                Assert.AreEqual(AppLogic.Enums.ParametersValidationResult.Valid, creationResult);
                frame.Navigate(typeof(QuestionPage), session);

                var questionPage = frame.Content as QuestionPage;
                var flippingCanvas = questionPage.GetType().GetRuntimeField("flippingCanvas").GetValue(questionPage) as FlippingCanvas;
                
            });
        }
    }
}
