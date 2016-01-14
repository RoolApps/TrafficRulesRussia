using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLogic
{
    public interface IDataAccessor
    {
        int[] GetTicketNums();

        int[] GetQuestionIdsByTickets(int[] tickets);

        IQuestion GetQuestionById(int id);
    }
}
