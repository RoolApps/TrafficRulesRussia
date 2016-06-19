using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppLogic.Interfaces;
using System.Runtime.Serialization;

namespace AppLogic
{
    [DataContract]
    [KnownType(typeof(Question))]
    public class Ticket : ITicket
    {
        [DataMember]
        public int Number { get; internal set; }
        [DataMember]
        public IEnumerable<IQuestion> Questions { get; internal set; }
    }
}
