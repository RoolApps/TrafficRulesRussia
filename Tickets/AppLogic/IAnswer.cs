using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLogic
{
    public interface IAnswer
    {
        int Id { get; set; }
        bool IsRight { get; set; }
        String Text { get; set; }
    }
}
