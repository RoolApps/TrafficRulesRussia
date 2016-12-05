using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AppLogic.Interfaces;
using Utils;
using System.Runtime.Serialization;

namespace AppLogic {
    [DataContract]
    [KnownType(typeof(Session))]
    public class SessionContainer{
        [DataMember]
        private List<ISession> sessionContainer;

        public SessionContainer() {
            sessionContainer = new List<ISession>();
        }

        public void AddNewSession( ISession session ) {
            sessionContainer.Add(session);
        }

        public ISession GetNext() {
            ISession first = sessionContainer.First();
            sessionContainer.RemoveAt(0);
            Log.d("get: {0}", first.Tickets.Select(t=>t.Number).ToString());
            return first;
        }

        public void Show() {
            Log.m(sessionContainer.SelectMany(s => s.Tickets).Select(t => t.Number).ToArray());
        }
    }
}
