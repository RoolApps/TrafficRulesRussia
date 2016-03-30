using AppLogic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLogic
{
    class Answer : IAnswer
    {
        private bool isSelected;

        public event EventHandler<IsSelectedChangedEventArgs> IsSelectedChanged;

        public bool IsRight { get; internal set; }

        public string Text { get; internal set; }

        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                if(value != IsSelected && IsSelectedChanged != null)
                {
                    isSelected = value;
                    IsSelectedChanged(this, new IsSelectedChangedEventArgs(IsSelected));
                }
            }
        }
    }
}
