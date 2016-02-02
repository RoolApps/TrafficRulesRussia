using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XAMLMarkup.Interfaces
{
    interface IPagedCollection
    {
        void MoveSelectedIndex(MoveDirection direction);
    }

    enum MoveDirection { Forward, Back }
}
