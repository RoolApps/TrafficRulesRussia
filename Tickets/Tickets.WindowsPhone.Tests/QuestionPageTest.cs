using System;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using AppData;
using AppLogic;
using AppLogic.Interfaces;
using XAMLMarkup;
using SQLiteShared;
using Utils;



namespace Tickets.WindowsPhone.Tests
{
    [TestClass]
    public class QuestionPageTest
    {
        public class SessionParameters : ISessionParameters
        {

            public bool Shuffle {
                get{
                    return false;
                }
            }

            public int[] TicketNums
            {
                get 
                {
                    using (var accessor = new SQLiteDataAccessor())
                    {
                        return accessor.CreateQuery<SQLiteShared.Models.Tickets>().Select(ticket => ticket.num).ToArray();
                    }
                }
            }

            public AppLogic.Enums.QuestionsGenerationMode Mode
            {
                get { return AppLogic.Enums.QuestionsGenerationMode.SelectedTickets; }
            }

            public System.Collections.Generic.IEnumerable<IQuestion> Questions
            {
                get { return null; }
            }
        }

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            Resources.ConnectionString = Path.Combine(Package.Current.InstalledLocation.Path, Resources.DBFileName);
        }

        [Ignore]
        [TestMethod]
        public async Task QuestionContentFitsToTextBlock()
        {
            ISession session;
            var sessionParameters = new SessionParameters();
            var creationResult = SessionFactory.CreateSession(sessionParameters, out session);
            Assert.AreEqual(AppLogic.Enums.ParametersValidationResult.Valid, creationResult);
            var left = session.Tickets.SelectMany(ticket => ticket.Questions).Count();
            Size? foundSize = null;
            Size allowedSize;

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                var frame = new Frame();
                frame.Navigate(typeof(QuestionPage), session);

                var questionPage = frame.Content as QuestionPage;
                var questionPageFields = questionPage.GetType().GetRuntimeFields();
                var pagedCanvas = questionPageFields.Single(f => f.FieldType == typeof(PagedCanvas)).GetValue(questionPage) as PagedCanvas;

                FrameworkElement element = pagedCanvas.ItemTemplate.LoadContent() as FrameworkElement;
                var questionTextBlock = (element as Canvas).Children.OfType<TextBlock>().Single();
                allowedSize = new Size(questionTextBlock.Width, questionTextBlock.Height);

                foreach (var question in session.Tickets.SelectMany(ticket => ticket.Questions))
                {
                    questionTextBlock.DataContext = question;
                    questionTextBlock.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                    if(questionTextBlock.ActualHeight > questionTextBlock.Height
                        || questionTextBlock.ActualWidth > questionTextBlock.Width)
                    {
                        foundSize = new Size(questionTextBlock.ActualHeight, questionTextBlock.ActualWidth);
                        break;
                    }
                }
            });
            Assert.IsNull(foundSize, "Question TextBlock does not fix: at least one text block's (width,height) is: ({0},{1}){2}Max allowed (width,height) is: ({3},{4})",
                (foundSize ?? new Size()).Width, (foundSize ?? new Size()).Height, Environment.NewLine, allowedSize.Width, allowedSize.Height);
        }
    }
}
