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
        public int Id
        {
            get { throw new NotImplementedException(); }
        }

        public byte[] Image
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<IAnswer> Answers
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsAnswered
        {
            get { throw new NotImplementedException(); }
        }
    }
}
