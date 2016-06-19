using AppLogic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AppLogic
{
    [DataContract(IsReference=true)]
    public class Answer : IAnswer, System.ComponentModel.INotifyPropertyChanged
    {
        [DataMember]
        private bool isSelected;

        public event EventHandler<IsSelectedChangedEventArgs> IsSelectedChanged;
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        [DataMember]
        public IQuestion Question { get; internal set; }

        [DataMember]
        public bool IsRight { get; internal set; }

        [DataMember]
        public string Text { get; internal set; }

        [DataMember]
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
