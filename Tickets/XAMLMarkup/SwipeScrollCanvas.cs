using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using XAMLMarkup.Interfaces;

namespace XAMLMarkup
{
    public class SwipeScrollCanvas : Canvas
    {
        public DataTemplate Content { get; set; }
        public int MotionDuration { get; set; }
        //This is dangerous. Be sure to not handle user input when Canvas has more than one child
        private FrameworkElement Child
        {
            get
            {
                return this.Children.SingleOrDefault() as FrameworkElement;
            }
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            "ItemsSource",
            typeof(IVirtualLinkedList),
            typeof(SwipeScrollCanvas),
            new PropertyMetadata(0, DataSource_Changed));


        public SwipeScrollCanvas()
        {
            MotionDuration = 300;

            ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;

            ManipulationDelta += SwipeScrollCanvas_ManipulationDelta;
            ManipulationCompleted += SwipeScrollCanvas_ManipulationCompleted;
        }


        public IVirtualLinkedList DataSource 
        {
            get
            {
                return GetValue(ItemsSourceProperty) as IVirtualLinkedList;
            }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }


        private static void DataSource_Changed(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue == null)
                return;

            if (args.NewValue == args.OldValue)
                return;

            var canvas = dependencyObject as SwipeScrollCanvas;
            if (canvas == null)
                return;

            if(canvas.Content != null)
            {
                canvas.ChangeCanvasContent(canvas.DataSource.Current, true);
            }
            else
            {
                throw new NotImplementedException("Either define Content first before assigning DataSource or implement DependencyProperty \"ContentProperty\"");
            }
        }

        private void SwipeScrollCanvas_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            var canvasHeight = ActualHeight;
            var child = Child;
            var childHeight = child.ActualHeight;
            if (canvasHeight < childHeight)
            {
                var childTop = Canvas.GetTop(child) + e.Delta.Translation.Y;
                if (childTop <= 0 && childTop > canvasHeight - childHeight)
                {
                    Canvas.SetTop(child, Canvas.GetTop(child) + e.Delta.Translation.Y);
                }
            }
        }

        private void SwipeScrollCanvas_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (Math.Abs(e.Velocities.Linear.X) > 0.5 && Math.Abs(e.Cumulative.Translation.X) > this.ActualWidth / 2)
            {
                var toTheRight = Math.Sign(e.Cumulative.Translation.X) == -1;

                ChangeCanvasContent(toTheRight);
            }
        }

        public bool ChangeCanvasContent(bool toTheRight)
        {
            var changed = toTheRight ? DataSource.Next() : DataSource.Previous();

            if (changed)
            {
                ManipulationDelta -= SwipeScrollCanvas_ManipulationDelta;
                ManipulationCompleted -= SwipeScrollCanvas_ManipulationCompleted;
                ChangeCanvasContent(DataSource.Current, toTheRight);
            }
            return changed;
        }

        private void ChangeCanvasContent(object content, bool toTheRight)
        {
            var child = Child;

            FrameworkElement element = Content.LoadContent() as FrameworkElement;
            element.DataContext = content;
            Children.Add(element);
            var left = ActualWidth * (toTheRight ? 1 : -1);
            Canvas.SetLeft(element, left);
            Canvas.SetTop(element, 0);

            //Play animation
            //Animation callback

            AnimateContentChange(new DependencyObject[] { child, element }, -left, 
                () =>
                {
                    if (child != null)
                    {
                        Children.Remove(child);
                        ManipulationDelta += SwipeScrollCanvas_ManipulationDelta;
                        ManipulationCompleted += SwipeScrollCanvas_ManipulationCompleted;
                    }
                });
        }

        private void AnimateContentChange(IEnumerable<DependencyObject> objects, double by, Action callback = null)
        {
            Storyboard storyboard = new Storyboard();

            storyboard.Completed += (s, e) =>
            {
                if(callback != null)
                {
                    callback();
                }
            };

            foreach (var obj in objects.Where(item => item != null))
            {
                DoubleAnimation animation = new DoubleAnimation();
                animation.EasingFunction = new ExponentialEase();
                animation.EasingFunction.EasingMode = EasingMode.EaseInOut;
                animation.Duration = TimeSpan.FromMilliseconds(MotionDuration);
                animation.EnableDependentAnimation = true;
                animation.By = by;
                Storyboard.SetTarget(animation, obj);
                Storyboard.SetTargetProperty(animation, "(Canvas.Left)");
                storyboard.Children.Add(animation);
            }

            storyboard.Begin();
        }
    }
}
