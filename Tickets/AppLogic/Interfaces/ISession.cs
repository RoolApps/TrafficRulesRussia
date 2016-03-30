using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLogic.Interfaces
{
    public interface ISession
    {
        IEnumerable<IQuestion> Questions { get; }

        byte[] Serialize();
    }
}
