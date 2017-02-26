using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseGenerator.DataBase
{
    public class Data
    {
        public Ticket[] Tickets { get; set; }
        public RuleChapter[] Chapters { get; set; }
        public Sign[] Signs { get; set; }
        public Mark[] Marks { get; set; }
    }

    public interface IRuleObject
    {
        String Num { get; set; }
        byte[] Image { get; set; }
        String Description { get; set; }
    }

    public class Sign : IRuleObject
    {
        public String Num { get; set; }
        public byte[] Image { get; set; }
        public String Description { get; set; }
    }

    public class Mark : IRuleObject
    {
        public String Num { get; set; }
        public byte[] Image { get; set; }
        public String Description { get; set; }
    }

    public class RuleChapter
    {
        public String Name { get; set; }
        public String Content { get; set; }
    }

    public class Ticket
    {
        public int Num { get; set; }
        public Question[] Questions { get; set; }
    }

    public class Question
    {
        public String Text { get; set; }
        public int Num { get; set; }
        public byte[] Image { get; set; }
        public Ticket Ticket { get; set; }
        public Answer[] Answers { get; set; }
        public string Hint { get; set; }
    }

    public class Answer
    {
        public String Text { get; set; }
        public bool IsRight { get; set; }
        public Question Question { get; set; }
    }
}
