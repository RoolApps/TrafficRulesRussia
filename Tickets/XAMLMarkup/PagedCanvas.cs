using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XAMLMarkup.Interfaces;

namespace XAMLMarkup
{
    public sealed class PagedCanvas : Canvas
    {
        #region Private Members
        private event EventHandler loadNextRequested;
        private event EventHandler loadPreviousRequested;
        #endregion

        #region Public Properties
        public DataTemplate ItemTemplate { get; set; }

        public IEnumerable<Object> ItemsSource
        {
            get { return (IEnumerable<Object>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource",
                        typeof(IEnumerable<Object>),
                        typeof(PagedCanvas),
                        new PropertyMetadata(0, ItemsSourceChangedCallback));
        #endregion

        #region Public Methods
        public void LoadNext()
        {
            if(loadNextRequested != null)
            {
                loadNextRequested(this, EventArgs.Empty);
            }
        }

        public void LoadPrevious()
        {
            if(loadPreviousRequested != null)
            {
                loadPreviousRequested(this, EventArgs.Empty);
            }
        }
        #endregion

        #region Private Methods
        private void changableCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == NotifyCollectionChangedAction.Add)
            {
                AddCanvasItem(e.NewStartingIndex, e.NewItems.OfType<Object>().First());
            }
            else if(e.Action == NotifyCollectionChangedAction.Remove)
            {
                RemoveCanvasItem(e.OldStartingIndex);   
            }
            else
            {
                throw new NotImplementedException("We do not plan to implement this part");
            }
        }

        private static void ItemsSourceChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue == null)
                return;

            if (args.NewValue == args.OldValue)
                return;

            PagedCanvas pagedCanvas = dependencyObject as PagedCanvas;
            if (pagedCanvas == null)
                return;

            
            var pagedCollection = args.NewValue as IPagedCollection;
            if (pagedCollection != null)
            {
                pagedCanvas.loadNextRequested += (s, e) =>
                {
                    pagedCollection.MoveSelectedIndex(MoveDirection.Forward);
                };
                pagedCanvas.loadPreviousRequested += (s, e) =>
                {
                    pagedCollection.MoveSelectedIndex(MoveDirection.Back);
                };
            }
            var changableCollection = args.NewValue as INotifyCollectionChanged;
            if (changableCollection != null)
            {
                changableCollection.CollectionChanged += pagedCanvas.changableCollection_CollectionChanged;
            }

            if (pagedCanvas.ItemsSource == null)
                return;

            pagedCanvas.ClearCanvas();
            int counter = 0;
            foreach (var elem in pagedCanvas.ItemsSource)
            {
                pagedCanvas.AddCanvasItem(counter++, elem);
            }
        }

        private void AddCanvasItem(int index, object content)
        {
            FrameworkElement element = ItemTemplate.LoadContent() as FrameworkElement;
            element.DataContext = content;
            double left;
            double offset = Window.Current.Bounds.Width;
            if (Children.Count == 0)
            {
                left = 0;
            }
            else if (index > 0)
            {
                left = Canvas.GetLeft(Children.Last()) + offset;
            }
            else
            {
                left = Canvas.GetLeft(Children.First()) - offset;
            }
            Children.Insert(index, element);
            Canvas.SetLeft(element, left);
        }
        
        private void RemoveCanvasItem(int index)
        {
            Children.RemoveAt(index);
        }

        private void ClearCanvas()
        {
            Children.Clear();
        }
        #endregion
    }
}
