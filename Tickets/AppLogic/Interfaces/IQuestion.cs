using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLogic.Interfaces
{
    public interface IQuestion
    {
        byte[] Image { get; }
        IEnumerable<IAnswer> Answers { get; }
        bool IsAnswered { get; }
        String Text { get; }
    }
}
