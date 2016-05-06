using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Windows.ApplicationModel;
using AppData;
using AppLogic.Interfaces;
using AppLogic.Enums;

namespace AppLogic.Tests
{
    [TestClass]
    public class SessionStatisticsTests
    {
        class Session : ISession
        {
            public IEnumerable<ITicket> Tickets { get; set; }

            public byte[] Serialize()
            {
                throw new NotImplementedException();
            }
        }

        class Ticket : ITicket
        {
            public int Number
            {
                get { throw new NotImplementedException(); }
            }

            public IEnumerable<IQuestion> Questions { get; set; }
        }

        class SessionParameters : ISessionParameters
        {
            public bool Shuffle
            {
                get { throw new NotImplementedException(); }
            }

            public int[] TicketNums
            {
                get { throw new NotImplementedException(); }
            }

            public Enums.QuestionsGenerationMode Mode
            {
                get { return Enums.QuestionsGenerationMode.RandomTicket; }
            }
        }

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            Resources.ConnectionString = Path.Combine(Package.Current.InstalledLocation.Path, Resources.DBFileName);
        }

        [TestMethod]
        public void SessionStatisticsCannotBeCreatedWithoutParameters()
        {
            ISessionStatistics sessionStatistics;
            var creationResult = SessionStatisticsFactory.CreateSessionStatistics(null, out sessionStatistics);
            Assert.IsNull(sessionStatistics);
            Assert.AreEqual(SessionValidationResult.SessionNull, creationResult);
        }

        [TestMethod]
        public void SessionStatisticsCannotBeCreatedWithInvalidParameters()
        {
            Session session = new Session();
            ISessionStatistics sessionStatistics;
            var creationResult = SessionStatisticsFactory.CreateSessionStatistics(session, out sessionStatistics);
            Assert.IsNull(sessionStatistics);
            Assert.AreEqual(SessionValidationResult.NoTickets, creationResult);

            session.Tickets = new Ticket[] { new Ticket() };
            creationResult = SessionStatisticsFactory.CreateSessionStatistics(session, out sessionStatistics);
            Assert.IsNull(sessionStatistics);
            Assert.AreEqual(SessionValidationResult.NoQuestions, creationResult);
        }

        [TestMethod]
        public void SessionStatistisCalculatesValidData()
        {
            ISession session;
            var sessionCreationResult = SessionFactory.CreateSession(new SessionParameters(), out session);
            Assert.AreEqual(ParametersValidationResult.Valid, sessionCreationResult);
            ISessionStatistics sessionStatistics;
            var sessionStatisticsCreationResult = SessionStatisticsFactory.CreateSessionStatistics(session, out sessionStatistics);
            Assert.AreEqual(SessionValidationResult.Valid, sessionStatisticsCreationResult);
            Assert.AreEqual(1, sessionStatistics.TakenTickets);
            Assert.AreEqual(0, sessionStatistics.PassedTickets);
            Assert.AreEqual(0, sessionStatistics.PassedTicketsPercentage);
            Assert.AreEqual(20, sessionStatistics.TakenQuestions);
            Assert.AreEqual(0, sessionStatistics.PassedQuestions);
            Assert.AreEqual(0, sessionStatistics.PassedQuestionsPercentage);

            var answers = session.Tickets.SelectMany(ticket => ticket.Questions).SelectMany(question => question.Answers);
            foreach(var answer in answers)
            {
                if(answer.IsRight)
                {
                    answer.IsSelected = true;
                }
            }

            sessionStatisticsCreationResult = SessionStatisticsFactory.CreateSessionStatistics(session, out sessionStatistics);
            Assert.AreEqual(SessionValidationResult.Valid, sessionStatisticsCreationResult);
            Assert.AreEqual(1, sessionStatistics.TakenTickets);
            Assert.AreEqual(1, sessionStatistics.PassedTickets);
            Assert.AreEqual(100, sessionStatistics.PassedTicketsPercentage);
            Assert.AreEqual(20, sessionStatistics.TakenQuestions);
            Assert.AreEqual(20, sessionStatistics.PassedQuestions);
            Assert.AreEqual(100, sessionStatistics.PassedQuestionsPercentage);

            bool isSelected = false;
            foreach(var answer in answers)
            {
                if(answer.IsSelected)
                {
                    answer.IsSelected = isSelected;
                    isSelected = !isSelected;
                }
            }

            sessionStatisticsCreationResult = SessionStatisticsFactory.CreateSessionStatistics(session, out sessionStatistics);
            Assert.AreEqual(SessionValidationResult.Valid, sessionStatisticsCreationResult);
            Assert.AreEqual(1, sessionStatistics.TakenTickets);
            Assert.AreEqual(0, sessionStatistics.PassedTickets);
            Assert.AreEqual(0, sessionStatistics.PassedTicketsPercentage);
            Assert.AreEqual(20, sessionStatistics.TakenQuestions);
            Assert.AreEqual(10, sessionStatistics.PassedQuestions);
            Assert.AreEqual(50, sessionStatistics.PassedQuestionsPercentage);
        }
    }
}
