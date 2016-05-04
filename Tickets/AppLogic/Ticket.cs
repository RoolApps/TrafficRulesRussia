using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppLogic.Interfaces;

namespace AppLogic
{
    public class Ticket : ITicket
    {
        public int Number { get; internal set; }

        public IEnumerable<IQuestion> Questions { get; internal set; }
    }
}
