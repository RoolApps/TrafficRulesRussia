using AppLogic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLogic
{
    class Session : ISession
    {
        #region Constructor
        internal Session(ISessionParameters parameters)
        {
            LoadQuestions(parameters);
        }
        #endregion

        #region Public Methods
        public IEnumerable<IQuestion> Questions { get; private set; }

        public byte[] Serialize()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Private Methods
        private void LoadQuestions(ISessionParameters parameters)
        {
            throw new NotImplementedException();
        }

        private int[] GetTicketNums()
        {
            throw new NotImplementedException();
        }

        private int[] GetQuestionIdsByTickets(int[] tickets)
        {
            throw new NotImplementedException();
        }

        private IQuestion GetQuestionById(int id)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
