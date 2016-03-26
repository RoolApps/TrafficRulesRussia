using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Collections.Specialized;

namespace XAMLMarkup.Tests
{
    [TestClass]
    public class PagedCollectionTests
    {
        class CollectionEntry
        {
        }

        private IEnumerable<CollectionEntry> TestCollection
        {
            get
            {
                return Enumerable.Range(0, 5).Select(i => new CollectionEntry()).ToArray();
            }
        }

        private PagedCollection<T> GetPagedCollection<T>(IEnumerable<T> collection, int paging = 1)
        {
            var pagedCollection = new PagedCollection<T>(paging);
            pagedCollection.DataSource = collection;
            return pagedCollection;
        }

        private IEnumerable<T> GetPagedCollectionData<T>(IEnumerable<T> collection, int paging = 1)
        {
            return GetPagedCollection(collection, paging);
        }

        private IEnumerable<T> GetPagedCollectionData<T>(PagedCollection<T> collection)
        {
            return collection;
        }

        /// <summary>
        /// Check if paged collection can be casted to IEnumerable 
        /// </summary>
        [TestMethod]
        public void PagedCollectionDataSourceAssignable()
        {
            var pagedCollection = GetPagedCollectionData(TestCollection);
            Assert.IsNotNull(pagedCollection as System.Collections.IEnumerable);
        }

        /// <summary>
        /// Check if paged collection returns valid data on assign
        /// </summary>
        [TestMethod]
        public void PagedCollectionReturnsValidDataOnAssign()
        {
            var collection = TestCollection;
            var pagedCollection = GetPagedCollectionData(collection);
            Assert.AreEqual(2, pagedCollection.Count());
            Assert.AreEqual(collection.ElementAt(0), pagedCollection.ElementAt(0));
            Assert.AreEqual(collection.ElementAt(1), pagedCollection.ElementAt(1));
        }

        /// <summary>
        /// Check if paged collection returns valid data after index move
        /// </summary>
        [TestMethod]
        public void PagedCollectionValidDataOnIndexChange()
        {
            var collection = TestCollection;
            var pagedCollection = GetPagedCollection(collection);

            pagedCollection.MoveSelectedIndex(Interfaces.MoveDirection.Forward);
            //index 1
            Assert.AreEqual(3, pagedCollection.Count());
            Assert.AreEqual(collection.ElementAt(0), pagedCollection.ElementAt(0));
            Assert.AreEqual(collection.ElementAt(1), pagedCollection.ElementAt(1));
            Assert.AreEqual(collection.ElementAt(2), pagedCollection.ElementAt(2));

            pagedCollection.MoveSelectedIndex(Interfaces.MoveDirection.Forward);
            //index 2
            Assert.AreEqual(3, pagedCollection.Count());
            Assert.AreEqual(collection.ElementAt(1), pagedCollection.ElementAt(0));
            Assert.AreEqual(collection.ElementAt(2), pagedCollection.ElementAt(1));
            Assert.AreEqual(collection.ElementAt(3), pagedCollection.ElementAt(2));

            pagedCollection.MoveSelectedIndex(Interfaces.MoveDirection.Forward);
            //index 3
            Assert.AreEqual(3, pagedCollection.Count());
            Assert.AreEqual(collection.ElementAt(2), pagedCollection.ElementAt(0));
            Assert.AreEqual(collection.ElementAt(3), pagedCollection.ElementAt(1));
            Assert.AreEqual(collection.ElementAt(4), pagedCollection.ElementAt(2));

            pagedCollection.MoveSelectedIndex(Interfaces.MoveDirection.Forward);
            //index 4
            Assert.AreEqual(2, pagedCollection.Count());
            Assert.AreEqual(collection.ElementAt(3), pagedCollection.ElementAt(0));
            Assert.AreEqual(collection.ElementAt(4), pagedCollection.ElementAt(1));

            pagedCollection.MoveSelectedIndex(Interfaces.MoveDirection.Back);
            //index 3
            Assert.AreEqual(3, pagedCollection.Count());
            Assert.AreEqual(collection.ElementAt(2), pagedCollection.ElementAt(0));
            Assert.AreEqual(collection.ElementAt(3), pagedCollection.ElementAt(1));
            Assert.AreEqual(collection.ElementAt(4), pagedCollection.ElementAt(2));

            pagedCollection.MoveSelectedIndex(Interfaces.MoveDirection.Back);
            //index 2
            Assert.AreEqual(3, pagedCollection.Count());
            Assert.AreEqual(collection.ElementAt(1), pagedCollection.ElementAt(0));
            Assert.AreEqual(collection.ElementAt(2), pagedCollection.ElementAt(1));
            Assert.AreEqual(collection.ElementAt(3), pagedCollection.ElementAt(2));

            pagedCollection.MoveSelectedIndex(Interfaces.MoveDirection.Back);
            //index 1
            Assert.AreEqual(3, pagedCollection.Count());
            Assert.AreEqual(collection.ElementAt(0), pagedCollection.ElementAt(0));
            Assert.AreEqual(collection.ElementAt(1), pagedCollection.ElementAt(1));
            Assert.AreEqual(collection.ElementAt(2), pagedCollection.ElementAt(2));

            pagedCollection.MoveSelectedIndex(Interfaces.MoveDirection.Back);
            //index 0
            Assert.AreEqual(2, pagedCollection.Count());
            Assert.AreEqual(collection.ElementAt(0), pagedCollection.ElementAt(0));
            Assert.AreEqual(collection.ElementAt(1), pagedCollection.ElementAt(1));
        }

        /// <summary>
        /// Check if paged collection handles out of range "move selected index" request correctly
        /// </summary>
        [TestMethod]
        public void PagedCollectionIndexCannotBeOutOfRange()
        {
            var collection = TestCollection;
            var pagedCollection = GetPagedCollection(collection);
            
            foreach(var i in Enumerable.Range(0, collection.Count() + 1))
            {
                pagedCollection.MoveSelectedIndex(Interfaces.MoveDirection.Forward);
            }

            Assert.AreEqual(2, pagedCollection.Count());
            Assert.AreEqual(collection.ElementAt(3), pagedCollection.ElementAt(0));
            Assert.AreEqual(collection.ElementAt(4), pagedCollection.ElementAt(1));

            foreach(var i in Enumerable.Range(0, collection.Count() + 1))
            {
                pagedCollection.MoveSelectedIndex(Interfaces.MoveDirection.Back);
            }

            Assert.AreEqual(2, pagedCollection.Count());
            Assert.AreEqual(collection.ElementAt(0), pagedCollection.ElementAt(0));
            Assert.AreEqual(collection.ElementAt(1), pagedCollection.ElementAt(1));
        }

        /// <summary>
        /// Check if paged collection raises valid events on index change
        /// </summary>
        [TestMethod]
        public void PagedCollectionRaisesValidEventsOnIndexChange()
        {
            var collection = TestCollection;
            var pagedCollection = GetPagedCollection(collection);
            NotifyCollectionChangedEventHandler addHandler = null;
            NotifyCollectionChangedEventArgs addEventArgs = null;
            addHandler = (s, e) =>
            {
                if(e.Action == NotifyCollectionChangedAction.Add)
                {
                    addEventArgs = e;
                    pagedCollection.CollectionChanged -= addHandler;
                }
            };
            pagedCollection.CollectionChanged += addHandler;

            NotifyCollectionChangedEventHandler removeHandler = null;
            NotifyCollectionChangedEventArgs removeEventArgs = null;
            removeHandler = (s, e) =>
            {
                if(e.Action == NotifyCollectionChangedAction.Remove)
                {
                    removeEventArgs = e;
                    pagedCollection.CollectionChanged -= removeHandler;
                }
            };
            pagedCollection.CollectionChanged += removeHandler;


            pagedCollection.MoveSelectedIndex(Interfaces.MoveDirection.Forward);
            //index 1
            Assert.IsNotNull(addEventArgs);
            Assert.AreEqual(NotifyCollectionChangedAction.Add, addEventArgs.Action);
            Assert.AreEqual(1, addEventArgs.NewItems.Count);
            Assert.AreEqual(collection.ElementAt(2), addEventArgs.NewItems.OfType<Object>().Single());
            Assert.AreEqual(2, addEventArgs.NewStartingIndex);

            Assert.IsNull(removeEventArgs);

            addEventArgs = null;
            removeEventArgs = null;
            pagedCollection.CollectionChanged += addHandler;
            pagedCollection.CollectionChanged += removeHandler;

            pagedCollection.MoveSelectedIndex(Interfaces.MoveDirection.Forward);
            //index 2
            Assert.IsNotNull(addEventArgs);
            Assert.AreEqual(NotifyCollectionChangedAction.Add, addEventArgs.Action);
            Assert.AreEqual(1, addEventArgs.NewItems.Count);
            Assert.AreEqual(collection.ElementAt(3), addEventArgs.NewItems.OfType<Object>().Single());
            Assert.AreEqual(2, addEventArgs.NewStartingIndex);

            Assert.IsNotNull(removeEventArgs);
            Assert.AreEqual(NotifyCollectionChangedAction.Remove, removeEventArgs.Action);
            Assert.AreEqual(1, removeEventArgs.OldItems.Count);
            Assert.AreEqual(collection.ElementAt(0), removeEventArgs.OldItems.OfType<Object>().Single());
            Assert.AreEqual(0, removeEventArgs.OldStartingIndex);


            addEventArgs = null;
            removeEventArgs = null;
            pagedCollection.CollectionChanged += addHandler;
            pagedCollection.CollectionChanged += removeHandler;

            pagedCollection.MoveSelectedIndex(Interfaces.MoveDirection.Forward);
            //index 3
            Assert.IsNotNull(addEventArgs);
            Assert.AreEqual(NotifyCollectionChangedAction.Add, addEventArgs.Action);
            Assert.AreEqual(1, addEventArgs.NewItems.Count);
            Assert.AreEqual(collection.ElementAt(4), addEventArgs.NewItems.OfType<Object>().Single());
            Assert.AreEqual(2, addEventArgs.NewStartingIndex);

            Assert.IsNotNull(removeEventArgs);
            Assert.AreEqual(NotifyCollectionChangedAction.Remove, removeEventArgs.Action);
            Assert.AreEqual(1, removeEventArgs.OldItems.Count);
            Assert.AreEqual(collection.ElementAt(1), removeEventArgs.OldItems.OfType<Object>().Single());
            Assert.AreEqual(0, removeEventArgs.OldStartingIndex);


            addEventArgs = null;
            removeEventArgs = null;
            pagedCollection.CollectionChanged += addHandler;
            pagedCollection.CollectionChanged += removeHandler;

            pagedCollection.MoveSelectedIndex(Interfaces.MoveDirection.Forward);
            //index 4
            Assert.IsNull(addEventArgs);

            Assert.IsNotNull(removeEventArgs);
            Assert.AreEqual(NotifyCollectionChangedAction.Remove, removeEventArgs.Action);
            Assert.AreEqual(1, removeEventArgs.OldItems.Count);
            Assert.AreEqual(collection.ElementAt(2), removeEventArgs.OldItems.OfType<Object>().Single());
            Assert.AreEqual(0, removeEventArgs.OldStartingIndex);


            addEventArgs = null;
            removeEventArgs = null;
            pagedCollection.CollectionChanged += addHandler;
            pagedCollection.CollectionChanged += removeHandler;

            pagedCollection.MoveSelectedIndex(Interfaces.MoveDirection.Back);
            //index 3
            Assert.IsNotNull(addEventArgs);
            Assert.AreEqual(NotifyCollectionChangedAction.Add, addEventArgs.Action);
            Assert.AreEqual(1, addEventArgs.NewItems.Count);
            Assert.AreEqual(collection.ElementAt(2), addEventArgs.NewItems.OfType<Object>().Single());
            Assert.AreEqual(0, addEventArgs.NewStartingIndex);

            Assert.IsNull(removeEventArgs);


            addEventArgs = null;
            removeEventArgs = null;
            pagedCollection.CollectionChanged += addHandler;
            pagedCollection.CollectionChanged += removeHandler;

            pagedCollection.MoveSelectedIndex(Interfaces.MoveDirection.Back);
            //index 2
            Assert.IsNotNull(addEventArgs);
            Assert.AreEqual(NotifyCollectionChangedAction.Add, addEventArgs.Action);
            Assert.AreEqual(1, addEventArgs.NewItems.Count);
            Assert.AreEqual(collection.ElementAt(1), addEventArgs.NewItems.OfType<Object>().Single());
            Assert.AreEqual(0, addEventArgs.NewStartingIndex);

            Assert.IsNotNull(removeEventArgs);
            Assert.AreEqual(NotifyCollectionChangedAction.Remove, removeEventArgs.Action);
            Assert.AreEqual(1, removeEventArgs.OldItems.Count);
            Assert.AreEqual(collection.ElementAt(4), removeEventArgs.OldItems.OfType<Object>().Single());
            Assert.AreEqual(2, removeEventArgs.OldStartingIndex);


            addEventArgs = null;
            removeEventArgs = null;
            pagedCollection.CollectionChanged += addHandler;
            pagedCollection.CollectionChanged += removeHandler;

            pagedCollection.MoveSelectedIndex(Interfaces.MoveDirection.Back);
            //index 1
            Assert.IsNotNull(addEventArgs);
            Assert.AreEqual(NotifyCollectionChangedAction.Add, addEventArgs.Action);
            Assert.AreEqual(1, addEventArgs.NewItems.Count);
            Assert.AreEqual(collection.ElementAt(0), addEventArgs.NewItems.OfType<Object>().Single());
            Assert.AreEqual(0, addEventArgs.NewStartingIndex);

            Assert.IsNotNull(removeEventArgs);
            Assert.AreEqual(NotifyCollectionChangedAction.Remove, removeEventArgs.Action);
            Assert.AreEqual(1, removeEventArgs.OldItems.Count);
            Assert.AreEqual(collection.ElementAt(3), removeEventArgs.OldItems.OfType<Object>().Single());
            Assert.AreEqual(2, removeEventArgs.OldStartingIndex);


            addEventArgs = null;
            removeEventArgs = null;
            pagedCollection.CollectionChanged += addHandler;
            pagedCollection.CollectionChanged += removeHandler;

            pagedCollection.MoveSelectedIndex(Interfaces.MoveDirection.Back);
            //index 0
            Assert.IsNull(addEventArgs);

            Assert.IsNotNull(removeEventArgs);
            Assert.AreEqual(NotifyCollectionChangedAction.Remove, removeEventArgs.Action);
            Assert.AreEqual(1, removeEventArgs.OldItems.Count);
            Assert.AreEqual(collection.ElementAt(2), removeEventArgs.OldItems.OfType<Object>().Single());
            Assert.AreEqual(2, removeEventArgs.OldStartingIndex);
        }

        /// <summary>
        /// Check if paged collection ignores invalid index change and not raising events
        /// </summary>
        [TestMethod]
        public void PagedCollectionIsNotRaisingEventOnOutOfRangeIndex()
        {
            var collection = TestCollection;
            var pagedCollection = GetPagedCollection(collection);
            NotifyCollectionChangedEventHandler addHandler = null;
            NotifyCollectionChangedEventArgs addEventArgs = null;
            addHandler = (s, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    addEventArgs = e;
                    pagedCollection.CollectionChanged -= addHandler;
                }
            };
            pagedCollection.CollectionChanged += addHandler;

            NotifyCollectionChangedEventHandler removeHandler = null;
            NotifyCollectionChangedEventArgs removeEventArgs = null;
            removeHandler = (s, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    removeEventArgs = e;
                    pagedCollection.CollectionChanged -= removeHandler;
                }
            };
            pagedCollection.CollectionChanged += removeHandler;

            pagedCollection.MoveSelectedIndex(Interfaces.MoveDirection.Back);
            pagedCollection.CollectionChanged -= addHandler;
            pagedCollection.CollectionChanged -= removeHandler;

            Assert.IsNull(addEventArgs);
            Assert.IsNull(removeEventArgs);

            foreach(var i in Enumerable.Range(0, collection.Count() + 1))
            {
                pagedCollection.MoveSelectedIndex(Interfaces.MoveDirection.Forward);
            }

            pagedCollection.CollectionChanged += addHandler;
            pagedCollection.CollectionChanged += removeHandler;

            pagedCollection.MoveSelectedIndex(Interfaces.MoveDirection.Forward);
            pagedCollection.CollectionChanged -= addHandler;
            pagedCollection.CollectionChanged -= removeHandler;

            Assert.IsNull(addEventArgs);
            Assert.IsNull(removeEventArgs);
        }
    }
}
