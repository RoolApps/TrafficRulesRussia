using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLiteShared;
using SQLiteShared.Models;

namespace AppLogic
{
    public static class Queries
    {
        public static IEnumerable<int> GetTicketNums()
        {
            using(var accessor = new SQLiteDataAccessor())
            {
                return accessor.GetObjectsList<Tickets, int>(t => t.num);
            }
        }

        internal static IEnumerable<int> GetTicketIds()
        {
            using(var acessor = new SQLiteDataAccessor())
            {
                return acessor.GetObjectsList<Tickets, int>(t => t.id);
            }
        }

        internal static IEnumerable<int> GetTicketIdsByNums(IEnumerable<int> ticketNums)
        {
            using(var accessor = new SQLiteDataAccessor())
            {
                //TODO: get rid of columns names in strings
                return accessor.GetObjectsList<Tickets, int>(t => t.id, String.Format("id in ({0})", String.Join(",", ticketNums)));
            }
        }

        internal static IEnumerable<Questions> GetQuestionsByTicketIds(IEnumerable<int> ticketIds)
        {
            using(var accessor = new SQLiteDataAccessor())
            {
                return accessor.GetModelsList<Questions>(String.Format("ticket_id in ({0})", String.Join(",", ticketIds)));
            }
        }

        internal static IEnumerable<Answers> GetAnswersByQuestionIds(IEnumerable<int> questionIds)
        {
            using(var accessor = new SQLiteDataAccessor())
            {
                return accessor.GetModelsList<Answers>(String.Format("question_id in ({0})", String.Join(",", questionIds)));
            }
        }
    }
}
