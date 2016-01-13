using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteShared.Models
{
    [Table("Tickets")]
    public class Ticket : BaseModel
    {
        public int num { get; set; }
    }
    
    [Table("Questions")]
    public class Question : BaseModel
    {
        public int num { get; set; }

        public String question { get; set; }

        public byte[] image { get; set; }

        public int ticket_id { get; set; }
    }

    [Table("Answers")]
    public class Answer : BaseModel
    {
        public String answer { get; set; }

        public bool is_right { get; set; }

        public int questions_id { get; set; }
    }

    public abstract class BaseModel
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
    }
}
