using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XAMLMarkup.Interfaces;

namespace XAMLMarkup
{
    public class VirtualLinkedList<T> : IVirtualLinkedList where T : class
    {
        public delegate T Rule(IEnumerable<T> dataSource, T current);

        public IEnumerable<T> DataSource { get; private set; }
        public Rule ReturnNext { get; set; }
        public Rule ReturnPrevious { get; set; }

        public T Current { get; private set; }

        public VirtualLinkedList(IEnumerable<T> dataSource, Rule returnNext, Rule returnPrevious)
        {
            DataSource = dataSource.ToArray();
            Current = DataSource.FirstOrDefault();
            ReturnNext = returnNext;
            ReturnPrevious = returnPrevious;
        }

        public bool Next()
        {
            return GetNew(ReturnNext);
        }

        public bool Previous()
        {
            return GetNew(ReturnPrevious);
        }

        private bool GetNew(Rule rule)
        {
            var current = Current;
            var returned = rule(DataSource, current);
            Current = returned;
            return returned != current;
        }

        object IVirtualLinkedList.Current
        {
            get { return Current; }
        }
    }
}
