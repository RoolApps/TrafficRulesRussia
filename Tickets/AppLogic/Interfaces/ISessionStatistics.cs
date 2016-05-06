using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLogic.Interfaces
{
    public interface ISessionStatistics
    {
        int TakenTickets { get; }
        int PassedTickets { get; }
        int PassedTicketsPercentage { get; }

        int TakenQuestions { get; }
        int PassedQuestions { get; }
        int PassedQuestionsPercentage { get; }
    }
}
