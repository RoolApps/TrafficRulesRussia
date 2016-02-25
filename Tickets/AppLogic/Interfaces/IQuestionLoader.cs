using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLogic.Interfaces
{
    interface IQuestionLoader
    {
        IEnumerable<IQuestion> LoadQuestions(ISessionParameters parameters);
    }
}
