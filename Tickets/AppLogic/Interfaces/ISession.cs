using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppLogic.Enums;

namespace AppLogic.Interfaces
{
    public interface ISession
    {
        IEnumerable<ITicket> Tickets { get; }

        QuestionsGenerationMode Mode { get; }

        byte[] Serialize();
    }
}
