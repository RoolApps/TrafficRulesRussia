using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Reflection;
using XAMLMarkup.Interfaces;
using Utils;

namespace XAMLMarkup
{
    public sealed partial class PagedCanvas : UserControl
    {
        #region Private Members
        private IEnumerable dataSource = null;
        private event EventHandler loadNextRequested;
        private event EventHandler loadPreviousRequested;
        #endregion

        #region Public Properties
        public IEnumerable DataSource
        {
            get
            {
                return dataSource;
            }
            set
            {
                this.dataSource = value;
                int counter = 0;
                foreach (var elem in value)
                {
                    AddCanvasItem(counter++, elem);
                }
                var pagedCollection = value as IPagedCollection;
                if(pagedCollection != null)
                {
                    this.loadNextRequested += (s, e) =>
                    {
                        pagedCollection.MoveSelectedIndex(MoveDirection.Forward);
                    };
                    this.loadPreviousRequested += (s, e) =>
                    {
                        pagedCollection.MoveSelectedIndex(MoveDirection.Back);
                    };
                }
                var changableCollection = value as INotifyCollectionChanged;
                if(changableCollection != null)
                {
                    changableCollection.CollectionChanged += changableCollection_CollectionChanged;
                }
            }
        }
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
                //RemoveCanvasItem(e.NewStartingIndex);   
                RemoveCanvasItem(e.OldStartingIndex);   
            }
            else
            {
                throw new NotImplementedException("We do not plan to implement this part");
            }
        }

        public object CanvasContent
        {
            get { return (object)GetValue(CanvasContentProperty); }
            set { SetValue(CanvasContentProperty, value); }
        }

        public static readonly DependencyProperty CanvasContentProperty =
            DependencyProperty.Register("CanvasContent", typeof(object), typeof(PagedCanvas), null);

        private void AddCanvasItem(int index, object content)
        {
            UIElement CC = CanvasContent as UIElement;
            if(CC != null) {
                (CC as Canvas).DataContext = content;
                UIElement element = UIElementProperties.Clone(CC);
                if (element != null) {
                    double left;
                    double offset = Window.Current.Bounds.Width;
                    if(canvas.Children.Count == 0)
                    {
                        left = 0;
                    }
                    else if(index > 0)
                    {
                        left = Canvas.GetLeft(canvas.Children.Last()) + offset;
                    }
                    else
                    {
                        left = Canvas.GetLeft(canvas.Children.First()) - offset;
                    }
                    canvas.Children.Insert(index, element);
                    Canvas.SetLeft(element, left);
                }
            } else {
                throw new NotImplementedException("Not implemented yet");
            }
        }
        
        private void RemoveCanvasItem(int index)
        {
            //throw new NotImplementedException("Not implemented yet");
            canvas.Children.RemoveAt(index);
        }

        private void ClearCanvas()
        {
            //throw new NotImplementedException("Not implemented yet");
            canvas.Children.Clear();
        }

        #endregion

        public PagedCanvas()
        {
            this.InitializeComponent();
        }
    }
}
