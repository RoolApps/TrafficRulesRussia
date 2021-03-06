﻿using System;
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
                        CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
                            new List<T> { DataSource.ElementAt(lowerElement) }, Math.Min(pagingSize - multiplier * pagingSize, lowerElement)));
                    }

                    var upperElement = currentIndex + pagingSize * multiplier;
                    if(upperElement * multiplier <= upperBound * multiplier)
                    {
                        CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
                            new List<T> { DataSource.ElementAt(upperElement) }, Math.Min(pagingSize + multiplier * pagingSize, upperElement)));
                    }
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

        #region Constructor
        public PagedCollection(int pagingSize)
        {
            this.pagingSize = pagingSize;
        }
        #endregion
    }
}
