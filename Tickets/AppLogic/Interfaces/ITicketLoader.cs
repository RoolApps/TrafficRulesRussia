using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLogic.Interfaces
{
    interface ITicketLoader
    {
        IEnumerable<ITicket> LoadTickets(ISessionParameters parameters);
    }
}
