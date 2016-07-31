using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using AppLogic.Interfaces;
using AppLogic.Enums;
using AppLogic;
using AppData;
using Windows.ApplicationModel;
using System.IO;
using System.Diagnostics;
using Utils;

namespace Utils.Tests
{
    [TestClass]
    public class SerializerTests
    {
        class SP : ISessionParameters {

            public bool Shuffle {
                get;
                set;
            }

            public int[] TicketNums {
                get;
                set;
            }

            public QuestionsGenerationMode Mode {
                get;
                set;
            }
        }

        [ClassInitialize]
        public static void Initialize( TestContext context ) {
            Resources.ConnectionString = Path.Combine(Package.Current.InstalledLocation.Path, Resources.DBFileName);
        }

        private T Serialize<T>(object obj){
            // arrange
            string serializedObj = null;
            T deserializedObj = default(T);
            

            // act
            if(obj != null) {
                if((serializedObj = Serializer.SerializeToString(obj)) != null) {
                    deserializedObj = Serializer.DeserializeFromString<T>(serializedObj);
                }
            }

            // assert
            Assert.IsNotNull(serializedObj, "serializedObj == null");
            Assert.IsNotNull(deserializedObj, "deserializedObj == null");
            Assert.IsInstanceOfType(serializedObj, typeof(string), "serializedObj != string");
            //Assert.AreEqual(obj, deserializedObj);
            return deserializedObj;
        }

        [TestMethod]
        public void CanSerializeAnyObject() {
            Serialize<int>(1);
            Serialize<double>(1.1);
            Serialize<string>("TestString");
        }

        [TestMethod]
        public void CanSerializeUserObject() {
            ISession sSession;
            SessionFactory.CreateSession(new SP() { Mode = QuestionsGenerationMode.RandomTicket}, out sSession);
            ISession dSession = Serialize<Session>(sSession);

            var ticketNumberEquals = sSession.Tickets.SelectMany(t1 => (dSession.Tickets.Where(t2 => t2.Number == t1.Number)).Select(item => item.Number)).Any();
            Assert.IsTrue(ticketNumberEquals, "ticketNumbers are not equal");

            var sSessionTicketQuestions = sSession.Tickets.SelectMany(t => t.Questions.Select(q => q.Text));
            var dSessionTicketQuestions = dSession.Tickets.SelectMany(t => t.Questions.Select(q => q.Text));
            Assert.IsTrue(sSessionTicketQuestions.SequenceEqual(dSessionTicketQuestions), "ticketQuestions are not equal");

            var sSessionTicketAnswers = sSession.Tickets.SelectMany(t => t.Questions.SelectMany(q => q.Answers.Select(a => a.Text)));
            var dSessionTicketAnswers = dSession.Tickets.SelectMany(t => t.Questions.SelectMany(q => q.Answers.Select(a => a.Text)));
            Assert.IsTrue(sSessionTicketAnswers.SequenceEqual(dSessionTicketAnswers), "ticketAnswers are not equal");
        }
    }
}
