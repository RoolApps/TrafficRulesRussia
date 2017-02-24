using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace XAMLMarkup
{
    public class ScrollableGrid : StackPanel
    {
        public DataTemplate Content { get; set; }
        public int ColumnsCount { get; set; }

        public ScrollableGrid()
        {
            this.DataContextChanged += ScrollableGrid_DataContextChanged;
        }

        void ScrollableGrid_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            DataSource = DataContext as IEnumerable<object>;
        }

        public IEnumerable<object> DataSource
        {
            get
            {
                return GetValue(DataSourceProperty) as IEnumerable<object>;
            }
            set
            {
                SetValue(DataSourceProperty, value);
            }
        }

        public static readonly DependencyProperty DataSourceProperty = DependencyProperty.Register(
            "DataSource",
            typeof(IEnumerable<object>),
            typeof(ScrollableGrid),
            new PropertyMetadata(null, DataSource_Changed));

        private static void DataSource_Changed(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue == args.OldValue)
                return;

            var grid = dependencyObject as ScrollableGrid;
            if (grid == null)
                return;

            if (grid.Content != null)
            {
                IEnumerable<object> data = new object[] { };
                if (args.NewValue != null)
                {
                    data = grid.DataSource;
                }
                grid.ChangeGridContent(data);
            }
            else
            {
                throw new NotImplementedException("Either define Content first before assigning DataSource or implement DependencyProperty \"ContentProperty\"");
            }
        }

        public void ChangeGridContent(IEnumerable<object> data)
        {
            this.Children.Clear();

            foreach(var pack in data.Split(ColumnsCount))
            {
                var grid = CreateGrid(pack);
                this.Children.Add(grid);
            }
        }

        private Grid CreateGrid(IEnumerable<object> data)
        {
            var grid = new Grid();

            Repeat().ForEach(() =>
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            });

            var counter = new Counter();
            data.Select(item => new { Item = item, Column = counter.Next() }).ForEach(item =>
            {
                FrameworkElement element = Content.LoadContent() as FrameworkElement;
                element.DataContext = item.Item;
                element.SetValue(Grid.ColumnProperty, item.Column);

                grid.Children.Add(element);
            });

            return grid;
        }

        private IEnumerable<int> Repeat()
        {
            return Enumerable.Range(0, ColumnsCount);
        }
    }

    class Counter
    {
        int count = 0;

        public int Next()
        {
            return count++;
        }
    }

    static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action action)
        {
            foreach(var e in enumerable)
            {
                action();
            }
        }

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var e in enumerable)
            {
                action(e);
            }
        }

        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> array, int size)
        {
            for (var i = 0; i < (float)array.Count() / size; i++)
            {
                yield return array.Skip(i * size).Take(size);
            }
        }
    }
}
