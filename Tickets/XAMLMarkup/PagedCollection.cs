using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XAMLMarkup.Interfaces;

namespace XAMLMarkup
{
    public class PagedCollection<T> : IPagedCollection, INotifyCollectionChanged, IEnumerable<T>
    {
        #region Private Members
        int currentIndex = 0;
        int pagingSize = 0;
        private IEnumerable<T> dataSource = null;
        #endregion

        #region Events
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        #endregion

        #region Public Properties
        public IEnumerable<T> DataSource
        {
            get
            {
                return dataSource;
            }
            set
            {
                this.dataSource = value;
            }
        }
        #endregion

        #region Public Methods
        public void MoveSelectedIndex(MoveDirection direction)
        {
            if (direction == MoveDirection.Forward)
            {
                currentIndex++;
                if (CollectionChanged != null)
                {
                    throw new NotImplementedException("Not implemented yet");   
                    //load object from datasource
                    //raise event
                    //delete other object
                }
            }
            else if (direction == MoveDirection.Back)
            {
                currentIndex--;
                if (CollectionChanged != null)
                {
                    throw new NotImplementedException("Not implemented yet");
                    //load object from datasource
                    //raise event
                    //delete other object
                }
            }
            else throw new NotImplementedException("Not supported direction type");
        }

        public IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this.DataSource.OfType<T>().Skip(currentIndex).Take(pagingSize).GetEnumerator();
        }
        
        public IEnumerator IEnumerable.GetEnumerator()
        {
            return this.DataSource.OfType<T>().Skip(currentIndex).Take(pagingSize).GetEnumerator();
        }
        #endregion

        public PagedCollection(int pagingSize)
        {
            this.pagingSize = pagingSize;
        }
    }
}
