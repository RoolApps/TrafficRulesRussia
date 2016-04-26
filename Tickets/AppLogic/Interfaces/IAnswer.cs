using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLogic.Interfaces
{
    public interface IAnswer
    {
        event EventHandler<IsSelectedChangedEventArgs> IsSelectedChanged;

        bool IsRight { get; }
        String Text { get; }
        bool IsSelected { get; set; }
    }

    //this class has to be somehow related to IAnswer. need to figure out how exactly
    public class IsSelectedChangedEventArgs : EventArgs
    {
        public bool IsSelected { get; private set; }
        public IAnswer Answer { get; private set; }

        internal IsSelectedChangedEventArgs(bool isSelected, IAnswer answer)
        {
            this.IsSelected = isSelected;
            this.Answer = answer;
        }
    }
}
