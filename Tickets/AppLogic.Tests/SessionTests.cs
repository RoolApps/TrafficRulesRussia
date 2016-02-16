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
            ISession session;
            var parameters = new SessionParameters() { Mode = QuestionsGenerationMode.SelectedTickets, TicketNums = new int[] { 1 } };
            var creationResult = SessionFactory.CreateSession(parameters, out session);

            IEnumerable<string> questions;
            using(var accessor = new SQLiteShared.SQLiteDataAccessor())
            {
                //this test is very fragile because of column name using
                questions = accessor.GetObjectsList<SQLiteShared.Models.Questions, String>(q => q.question, "ticket_id = 1");
            }

            Assert.IsNotNull(session);
            Assert.AreEqual(ParametersValidationResult.Valid, creationResult);
            Assert.IsTrue(session.Questions.Any());
            Assert.IsTrue(questions.All(question => session.Questions.Any(q => q.Text == question)));
        }
    }
}
