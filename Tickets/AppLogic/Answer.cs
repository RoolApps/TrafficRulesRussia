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
        public event EventHandler<IsSelectedChangedEventArgs> IsSelectedChanged;

        public int Id
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsRight
        {
            get { throw new NotImplementedException(); }
        }

        public string Text
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsSelected
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                if(value != IsSelected && IsSelectedChanged != null)
                {
                    IsSelectedChanged(this, new IsSelectedChangedEventArgs(IsSelected));
                }
                throw new NotImplementedException();
            }
        }

        
    }
}
