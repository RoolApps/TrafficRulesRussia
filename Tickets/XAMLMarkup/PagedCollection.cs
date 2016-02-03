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
        private int dataSourceLastIndex = 0;
        private int dataSourceFirstIndex = 0;
        private int dataSourceLength = 0;
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
                dataSourceLength = value.Count();
                dataSourceLastIndex = dataSourceLength - 1;
            }
        }
        #endregion

        #region Public Methods
        public void MoveSelectedIndex(MoveDirection direction)
        {
            MoveIndex(direction);
        }

        public IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            var from = Math.Max(dataSourceFirstIndex, currentIndex);
            var to = Math.Min(dataSourceLength, currentIndex);
            return this.DataSource.Skip(from).Take(to - from + 1).GetEnumerator();
        }
        
        public IEnumerator IEnumerable.GetEnumerator()
        {
            return (this as IEnumerable<T>).GetEnumerator();
        }
        #endregion

        #region Constructor
        public PagedCollection(int pagingSize)
        {
            this.pagingSize = pagingSize;
        }
        #endregion

        #region Private Methods
        private void MoveIndex(MoveDirection direction)
        {
            int step = 1;
            int multiplier = direction == MoveDirection.Forward ? 1 : -1;
            int upperBound = direction == MoveDirection.Forward ? dataSourceLastIndex : dataSourceFirstIndex;
            int lowerBound = dataSourceLastIndex - upperBound;
            if(currentIndex * multiplier + step <= upperBound)
            {
                currentIndex += multiplier * step;
                if(CollectionChanged != null)
                {
                    var upperElement = currentIndex + pagingSize * multiplier;
                    if(upperElement * multiplier <= upperBound * multiplier)
                    {
                        CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
                            new List<T> { DataSource.ElementAt(upperElement) }, upperElement));
                    }
                    var lowerElement = currentIndex - multiplier * (pagingSize + step);
                    if(lowerElement * multiplier >= lowerBound * multiplier )
                    {
                        CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
                            new List<T> { DataSource.ElementAt(lowerElement) }, lowerElement));
                    }
                    //TODO: проверить формулы
                }
            }
        }
        #endregion
    }
}
