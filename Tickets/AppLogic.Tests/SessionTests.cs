using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using AppLogic.Interfaces;
using AppLogic.Enums;
using AppData;
using Windows.ApplicationModel;
using System.IO;

namespace AppLogic.Tests
{
    [TestClass]
    public class SessionTests
    {
        class SessionParameters : ISessionParameters
        {
            public bool Shuffle { get; set; }

            public int[] TicketNums { get; set; }

            public QuestionsGenerationMode Mode { get; set; }
        }

        public SessionTests()
        {
            Resources.ConnectionString = Path.Combine(Package.Current.InstalledLocation.Path, Resources.DBFileName);
        }

        [TestMethod]
        public void SessionCannotBeCreatedWithoutParameters()
        {
            ISession session;
            var creationResult = SessionFactory.CreateSession(null, out session);
            Assert.IsNull(session);
            Assert.AreEqual(ParametersValidationResult.ParamsNull, creationResult);
        }

        [TestMethod]
        public void SessionCannotBeCreatedWithInvalidParameters()
        {
            ISession session;
            var parameters = new SessionParameters() { Mode = QuestionsGenerationMode.SelectedTickets };
            var creationResult = SessionFactory.CreateSession(parameters, out session);
            Assert.IsNull(session);
            Assert.AreEqual(ParametersValidationResult.NoTickets, creationResult);

            parameters = new SessionParameters() { Mode = QuestionsGenerationMode.SelectedTickets, TicketNums = new int[] { 1, 1 } };
            creationResult = SessionFactory.CreateSession(parameters, out session);
            Assert.IsNull(session);
            Assert.AreEqual(ParametersValidationResult.DuplicateTickets, creationResult);
        }

        [TestMethod]
        public void SessionCanBeCreatedWithValidParameters()
        {
            ISession session;
            var parameters = new SessionParameters() { Mode = QuestionsGenerationMode.RandomTicket };
            var creationResult = SessionFactory.CreateSession(parameters, out session);
            Assert.IsNotNull(session);
            Assert.AreEqual(ParametersValidationResult.Valid, creationResult);
        }

        [TestMethod]
        public void SessionSelectsRightQuestions()
        {
            var ticketNums = new int[] { 1 };
            ISession session;
            var parameters = new SessionParameters() { Mode = QuestionsGenerationMode.SelectedTickets, TicketNums = ticketNums };
            var creationResult = SessionFactory.CreateSession(parameters, out session);

            IEnumerable<string> questions;
            using(var accessor = new SQLiteShared.SQLiteDataAccessor())
            {
                var ticketIds = accessor.CreateQuery<SQLiteShared.Models.Tickets>().Where(ticket => ticketNums.Contains(ticket.num)).Select(ticket => ticket.id);
                questions = accessor.CreateQuery<SQLiteShared.Models.Questions>().Where(question => ticketIds.Contains(question.ticket_id)).Select(question => question.question).ToArray();
            }

            Assert.IsNotNull(session);
            Assert.AreEqual(ParametersValidationResult.Valid, creationResult);
            Assert.IsTrue(session.Tickets.Any());
            Assert.IsTrue(questions.All(question => session.Tickets.SelectMany(ticket => ticket.Questions).Any(q => q.Text == question)));
        }

        [TestMethod]
        public void SessionCreatesWithCorrectHierarchy()
        {
            ISession session;
            var parameters = new SessionParameters() { Mode = QuestionsGenerationMode.RandomTicket };
            var creationResult = SessionFactory.CreateSession(parameters, out session);
            Assert.IsNotNull(session);
            Assert.AreEqual(ParametersValidationResult.Valid, creationResult);
            var ticket = session.Tickets.SingleOrDefault();
            Assert.IsNotNull(ticket);
            var question = ticket.Questions.FirstOrDefault();
            Assert.IsNotNull(question);
            Assert.AreEqual(question.Ticket, ticket);
            var answer = question.Answers.FirstOrDefault();
            Assert.IsNotNull(answer);
            Assert.AreEqual(answer.Question, question);
        }

        [TestMethod]
        [Ignore]
        public void SessionReturnsRandomTicket()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [Ignore]
        public void SessionShufflesQuestions()
        {
            throw new NotImplementedException();
        }
    }
}
