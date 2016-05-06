using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLogic.Interfaces
{
    public interface IQuestion
    {
        ITicket Ticket { get; }
        int Number { get; }
        byte[] Image { get; }
        IEnumerable<IAnswer> Answers { get; }
        IAnswer SelectedAnswered { get; }
        String Text { get; }
    }
}
