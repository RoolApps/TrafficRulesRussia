using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tickets.CommonUI
{
    public class HLContent : EventArgs
    {
        private string type;
        private string data;

        public HLContent(string type, string data)
        {
            this.type = type;
            this.data = data;
        }

        public string Type
        {
            get
            {
                return type;
            }
        }

        public string Data
        {
            get
            {
                return data;
            }
        }
    }
}
