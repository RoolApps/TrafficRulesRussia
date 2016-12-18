using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteShared.Models
{
    public class Tickets : BaseModel
    {
        public int num { get; set; }
    }
    
    public class Questions : BaseModel
    {
        public int num { get; set; }

        public String question { get; set; }

        public byte[] image { get; set; }

        public int ticket_id { get; set; }
    }

    public class Answers : BaseModel
    {
        public String answer { get; set; }

        public bool is_right { get; set; }

        public int question_id { get; set; }
    }

    public class Chapters : BaseModel
    {
        public String name { get; set; }

        public String content { get; set; }
    }

    public class Signs : BaseModel
    {
        public String num { get; set; }

        public String description { get; set; }

        public byte[] image { get; set; }
    }

    public class Marks : BaseModel
    {
        public String num { get; set; }

        public String description { get; set; }

        public byte[] image { get; set; }
    }

    public abstract class BaseModel
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
    }
}
