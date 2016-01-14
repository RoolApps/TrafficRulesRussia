using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLogic
{
    public class TestParams
    {
        public bool Shuffle { get; set; }
        public int[] TicketNums { get; set; }
        public TestMode Mode { get; set; }
    }

    public enum TestMode
    {
        RandomTicket,
        SelectedTickets
    }

    public enum ParamsValidationResult
    {
        Valid,
        NoTickets,
        DuplicateTickets,
        ParamsNull
    }
}
