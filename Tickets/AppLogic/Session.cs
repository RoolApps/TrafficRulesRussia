using AppLogic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using AppLogic.Enums;

namespace AppLogic
{
    [DataContract]
    [KnownType(typeof(Ticket))]
    public class Session : ISession
    {
        private QuestionsGenerationMode mode;
        #region Constructor
        internal Session(ISessionParameters parameters)
        {
            mode = parameters.Mode;
            LoadQuestions(parameters);
        }
        #endregion

        #region Public Methods
        [DataMember]
        public QuestionsGenerationMode Mode {
            get {
                return mode;
            }
            private set {
                mode = value;
            }
        }

        [DataMember]
        public IEnumerable<ITicket> Tickets { get; private set; }

        public byte[] Serialize()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Private Methods
        private void LoadQuestions(ISessionParameters parameters)
        {
            //TODO: switch to factory pattern
            ITicketLoader loader = new TicketLoader();
            Tickets = loader.LoadTickets(parameters);
        }
        #endregion
    }
}
