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
        const string SignsUrl = "http://pdd.drom.ru/pdd/signs/";
        const string MarksUrl = "http://pdd.drom.ru/pdd/marking/";

        public static DataBase.Data Read()
        {
            return new Data()
            {
                Tickets = TicketReader.Read(),
                Signs = RuleObjectReader.Read<Sign>(SignsUrl),
                Marks = RuleObjectReader.Read<Mark>(MarksUrl),
                Chapters = ChapterReader.Read()
            };
        }
    }
}
