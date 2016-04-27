using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

namespace XAMLMarkup
{
    public sealed class FlippingCanvas : Canvas
    {
        Ellipse corrector;

        #region Private Members
        private Boolean LKMIsPressed = false;
        private static int completedAnimations = 0;
        private double winHeight = Window.Current.Bounds.Height;
        private double winWidth = Window.Current.Bounds.Width;
        private enum MoveDirection : int { NoWhere, ToPrevious, ToNext };
        private MoveDirection direction;
        private Point lastPoint;
        private Point initialPoint;
        Storyboard storyboard;
        PagedCanvas pagedCanvas = null;
        #endregion

        #region Public Properties
        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("MotionDuration", typeof(double), typeof(FlippingCanvas), null);

        public double MotionDuration
        {
            get
            {
                return (double)GetValue(DurationProperty);
            }
            set
            {
                SetValue(DurationProperty, value);
            }
        }
        #endregion

        #region Private Methods
        private void canvas_Loaded(object sender, RoutedEventArgs e)
        {
            pagedCanvas = this.Children.OfType<PagedCanvas>().SingleOrDefault();
        }

        private void canvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            LKMIsPressed = true;
            lastPoint = initialPoint = e.GetCurrentPoint(this).Position;
            if (storyboard != null) {
                storyboard.Pause();
            }
            storyboard = new Storyboard();
        }

        private void canvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            LKMIsPressed = false;
            foreach (var obj in Children.ToList()) {
                AddMotionAnimation(obj, PxToMove(), completed);
            }
            storyboard.Begin();
        }

        private void canvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            double px = 0;
            if (LKMIsPressed) {
                Point current_point = e.GetCurrentPoint(this).Position;
                px = current_point.X - lastPoint.X;
                MoveObjectsOnCanvas(px);
                lastPoint = current_point;
            }
        }

        private void canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            winHeight = e.NewSize.Height;
            winWidth = e.NewSize.Width;
        }

        private void MoveObjectsOnCanvas(double px)
        {
            var objects = Children.ToList();
            foreach (var obj in objects) {
                Canvas.SetLeft(obj, Canvas.GetLeft(obj) + px);
            }
        }

        private void completed()
        {
            completedAnimations++;
            if (completedAnimations == Children.Count) {
                if (pagedCanvas != null) {
                    if (direction == MoveDirection.ToNext)
                    {
                        pagedCanvas.LoadNext();
                    }
                    else if (direction == MoveDirection.ToPrevious)
                    {
                        pagedCanvas.LoadPrevious();
                    }
                }
                completedAnimations = 0;
                Canvas.SetLeft(corrector, 0.0);
            }
        }

        private double PxToMove()
        {
            double delta = 0;
            double currentPosition = Canvas.GetLeft(corrector);
            if (currentPosition > -winWidth / 2 && currentPosition < winWidth / 2) {
                delta = -currentPosition;
                direction = MoveDirection.NoWhere;
            } else {
                if (currentPosition < 0) {
                    direction = MoveDirection.ToNext;
                    delta = -(winWidth - Math.Abs(currentPosition));
                } else {
                    direction = MoveDirection.ToPrevious;
                    delta = winWidth - currentPosition;
                }
            }
            return delta;
        }

        private void AddMotionAnimation(DependencyObject obj, double delta, Action completed = null)
        {
            DoubleAnimation animation = new DoubleAnimation();
            animation.EasingFunction = new ExponentialEase();
            animation.EasingFunction.EasingMode = EasingMode.EaseInOut;
            animation.Duration = MotionDuration > 0 ? TimeSpan.FromMilliseconds(MotionDuration) : TimeSpan.FromMilliseconds(1500);
            animation.EnableDependentAnimation = true;
            animation.By = delta;
            Storyboard.SetTarget(animation, obj);
            Storyboard.SetTargetProperty(animation, "(Canvas.Left)");
            storyboard.Children.Add(animation);
            if (completed != null) {
                storyboard.Completed += (s, e) => completed();
            }
        }
        #endregion

        #region Constructor
        public FlippingCanvas()
        {
            corrector = new Ellipse();
            Children.Add(corrector);

            Loaded += canvas_Loaded;
            SizeChanged += canvas_SizeChanged;
            PointerPressed += canvas_PointerPressed;
            PointerReleased += canvas_PointerReleased;
            PointerMoved += canvas_PointerMoved;
        }
        #endregion
    }
}
