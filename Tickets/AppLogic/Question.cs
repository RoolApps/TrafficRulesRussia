using AppLogic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLogic
{
    class Question : IQuestion
    {
        public byte[] Image { get; internal set; }

        public IEnumerable<IAnswer> Answers { get; internal set; }

        public bool IsAnswered 
        {
            get
            {
                return Answers.Any(answer => answer.IsSelected);
            }
        }

        public String Text { get; internal set; }
    }
}
