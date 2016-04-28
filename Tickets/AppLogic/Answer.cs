using AppLogic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLogic
{
    class Answer : IAnswer, System.ComponentModel.INotifyPropertyChanged
    {
        private bool isSelected;

        public event EventHandler<IsSelectedChangedEventArgs> IsSelectedChanged;
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

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
                if(value != IsSelected)
                {
                    isSelected = value;
                    if (IsSelectedChanged != null)
                    {
                        IsSelectedChanged(this, new IsSelectedChangedEventArgs(IsSelected, this));
                    }
                    if(PropertyChanged != null)
                    {
                        PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("IsSelected"));
                    }
                }
            }
        }
    }
}
