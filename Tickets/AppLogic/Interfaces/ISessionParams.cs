using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppLogic.Enums;

namespace AppLogic.Interfaces
{
    //has to be implemented in some UI project to reflect user interface
    public interface ISessionParameters
    {
        bool Shuffle { get; }
        int[] TicketNums { get; }
        QuestionsGenerationMode Mode { get; }
    }
}
