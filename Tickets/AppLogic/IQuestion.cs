using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLogic
{
    public interface IQuestion
    {
        int Id { get; set; }
        byte[] Image { get; set; }
        IEnumerable<IAnswer> Answers { get; set; }
        IAnswer SelectedAnswer { get; set; }
    }
}
