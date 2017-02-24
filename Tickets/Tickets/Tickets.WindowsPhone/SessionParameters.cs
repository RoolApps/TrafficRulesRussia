using AppLogic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tickets
{
    internal class SessionParameters : ISessionParameters
    {
        public SessionParameters(AppLogic.Enums.QuestionsGenerationMode mode)
        {
            this.Mode = mode;
        }

        public SessionParameters(AppLogic.Enums.QuestionsGenerationMode mode, bool shuffle, int[] ticketNums = null)
        {
            this.Mode = mode;
            this.Shuffle = shuffle;
            this.TicketNums = ticketNums;
        }

        public bool Shuffle { get; private set; }

        public int[] TicketNums { get; private set; }

        public AppLogic.Enums.QuestionsGenerationMode Mode { get; private set; }


        public IEnumerable<IQuestion> Questions { get { return null; } }
    }
}
