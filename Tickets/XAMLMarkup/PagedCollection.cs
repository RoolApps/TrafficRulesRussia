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
                var oldItems = this.AsEnumerable();
                this.dataSource = value;
                dataSourceLength = value.Count();
                dataSourceLastIndex = dataSourceLength - 1;
                currentIndex = 0;
                RaiseDataSourceChanged(oldItems);
            }
        }
        #endregion

        #region Public Methods
        public void MoveSelectedIndex(MoveDirection direction)
        {
            MoveIndex(direction);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        #endregion

        #region Constructor
        public PagedCollection(int pagingSize)
        {
            this.pagingSize = pagingSize;
        }
        #endregion

        #region Private Methods
        private IEnumerator<T> GetEnumerator()
        {
            var from = Math.Max(dataSourceFirstIndex, currentIndex - pagingSize);
            var to = Math.Min(dataSourceLength, currentIndex + pagingSize);
            return this.DataSource.Skip(from).Take(to - from + 1).GetEnumerator();
        }

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
                    var lowerElement = currentIndex - multiplier * (pagingSize + step);
                    if (lowerElement * multiplier >= lowerBound * multiplier)
                    {
                        //судя по тестам, может быть либо pagingSize * 2 либо 0
                        //т.е. step + multiplier * step
                        CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
                            new List<T> { DataSource.ElementAt(lowerElement) }, step - multiplier * step));
                    }

                    var upperElement = currentIndex + pagingSize * multiplier;
                    if(upperElement * multiplier <= upperBound * multiplier)
                    {
                        //ну чтож. тут нужно возвращать либо 0 либо Min(pagingSize * 2 + 1, upperElement), в зависимости от Direction
                        CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
                            new List<T> { DataSource.ElementAt(upperElement) }, step + multiplier * step));
                    }
                    
                    //TODO: проверить формулы
                }
            }
        }

        private void RaiseDataSourceChanged(IEnumerable<T> oldDataSource)
        {
            if(oldDataSource == null || !oldDataSource.Any())
            {
                return;
            }
            var list = oldDataSource.ToList();
            if(CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, list, list.IndexOf(list.First())));
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (this as IEnumerable<T>).ToList(), dataSourceFirstIndex));
            }
        }
        #endregion
    }
}
