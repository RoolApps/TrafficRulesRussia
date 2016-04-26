using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseGenerator
{
    class Data
    {
        public Ticket[] Tickets { get; set; }
    }

    class Ticket
    {
        public int Num { get; set; }
        public Question[] Questions { get; set; }
    }

    class Question
    {
        public String Text { get; set; }
        public int Num { get; set; }
        public byte[] Image { get; set; }
        public Ticket Ticket { get; set; }
        public Answer[] Answers { get; set; }
    }

    class Answer
    {
        public String Text { get; set; }
        public bool IsRight { get; set; }
        public Question Question { get; set; }
    }
}
