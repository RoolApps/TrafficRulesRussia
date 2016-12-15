using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XAMLMarkup.Interfaces
{
    public interface IVirtualLinkedList
    {
        bool Next();
        bool Previous();

        object Current { get; }
    }
}
