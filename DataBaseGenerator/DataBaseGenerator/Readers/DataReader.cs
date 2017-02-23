using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataBaseGenerator.DataBase;

namespace DataBaseGenerator.Readers
{
    public static class DataReader
    {
        public static DataBase.Data Read()
        {
            return new Data()
            {
                Tickets = TicketReader.Read(),
                Signs = SignsReader.Read(),
                Marks = MarksReader.Read(),
                Chapters = ChapterReader.Read()
            };
        }
    }
}
