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
using XAMLMarkup.Enums;
using XAMLMarkup.EventHandlers;

namespace XAMLMarkup
{
    public sealed class FlippingCanvas : Canvas
    {
        #region Private Members
        private Ellipse corrector;
        private Boolean LKMIsPressed = false;
        private Boolean onSliding = false;
        private static int completedAnimations = 0;
        private int currentScreen = 0;
        private int remainSlidingToPrevious = 0;
        private int remainSlidingToNext = 0;
        private double winHeight = Window.Current.Bounds.Height;
        private double winWidth = Window.Current.Bounds.Width;
        private MoveDirection direction;
        private Point lastPoint;
        private Point initialPoint;
        Storyboard storyboard;
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

        #region Public Methods
        public void SlideCanvas(MoveDirection direction) {
            if ( Canvas.GetLeft(corrector) == 0 && remainSlidingToNext > 0) {
                onSliding = true;
                RestartStoryboard();
                this.direction = direction;
                double delta = this.direction == MoveDirection.ToNext ? -ActualWidth : ActualWidth;
                foreach ( var obj in Children.ToList() ) {
                    AddMotionAnimation(obj, delta);
                }
                storyboard.Begin();
            }
        }
        #endregion

        #region Event Handlers

        public event EventHandler<OnFlipCompleted> OnCompleted;

        private void canvas_Loaded(object sender, RoutedEventArgs e) {
            RemainSliding();
        }

        private void canvas_PointerPressed(object sender, PointerRoutedEventArgs e) {
            LKMIsPressed = true;
            lastPoint = initialPoint = e.GetCurrentPoint(this).Position;
            RestartStoryboard();
        }

        private void canvas_PointerReleased(object sender, PointerRoutedEventArgs e) {
            if ( LKMIsPressed && !onSliding ) {
                LKMIsPressed = false;
                foreach ( var obj in Children.ToList() ) {
                    AddMotionAnimation(obj, PxToMove());
                }
                storyboard.Begin();
            }
        }

        private void canvas_PointerMoved(object sender, PointerRoutedEventArgs e) {
            double px = 0;
            if ( LKMIsPressed ) {
                Point current_point = e.GetCurrentPoint(this).Position;
                px = current_point.X - lastPoint.X;
                MoveObjectsOnCanvas(px);
                lastPoint = current_point;
            }
        }

        private void canvas_SizeChanged(object sender, SizeChangedEventArgs e) {
            winHeight = e.NewSize.Height;
            winWidth = e.NewSize.Width;
        }
        #endregion

        #region Private Methods
        private void RestartStoryboard() {
            if ( storyboard != null ) {
                storyboard.Pause();
            }
            storyboard = new Storyboard();
        }

        private void MoveObjectsOnCanvas(double px) {
            var objects = Children.ToList();
            foreach (var obj in objects) {
                Canvas.SetLeft(obj, Canvas.GetLeft(obj) + px);
            }
        }

        private void Completed(object sender, object e) {
            completedAnimations++;
            if (completedAnimations == Children.Count) {
                if ( OnCompleted != null ) {
                    OnCompleted(this, new OnFlipCompleted(direction));
                }
                if ( direction == MoveDirection.ToNext ) {
                    currentScreen++;
                } else if ( direction == MoveDirection.ToPrevious ) {
                    currentScreen--;
                }
                completedAnimations = 0;
                Canvas.SetLeft(corrector, 0.0);
                onSliding = false;
                RemainSliding();
            }
        }

        private void RemainSliding() {
            int toPrev = 0;
            int toNext = 0;
            foreach ( var child in this.Children.ToList() ) {
                if ( child is Panel ) {
                    foreach ( var item in (child as Panel).Children.ToList() ) {
                        if ( (int)(Canvas.GetLeft(item) / ActualWidth) < currentScreen ) {
                            toPrev++;
                        } else if ( (int)(Canvas.GetLeft(item) / ActualWidth) > currentScreen ) {
                            toNext++;
                        }
                    }
                }
            }
            remainSlidingToPrevious = toPrev;
            remainSlidingToNext = toNext;
        }


        private double PxToMove() {
            double delta = 0;
            double currentPosition = Canvas.GetLeft(corrector);
            if ((currentPosition > -winWidth / 2 && currentPosition < winWidth / 2) ||
                (currentPosition > winWidth / 2 && remainSlidingToPrevious == 0) ||
                (currentPosition < -winWidth / 2 && remainSlidingToNext == 0)){
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

        private void AddMotionAnimation(DependencyObject obj, double delta) {
            DoubleAnimation animation = new DoubleAnimation();
            animation.EasingFunction = new ExponentialEase();
            animation.EasingFunction.EasingMode = EasingMode.EaseInOut;
            animation.Duration = MotionDuration > 0 ? TimeSpan.FromMilliseconds(MotionDuration) : TimeSpan.FromMilliseconds(1500);
            animation.EnableDependentAnimation = true;
            animation.By = delta;
            Storyboard.SetTarget(animation, obj);
            Storyboard.SetTargetProperty(animation, "(Canvas.Left)");
            storyboard.Children.Add(animation);
            storyboard.Completed += Completed;
        }
        #endregion

        #region Constructor
        public FlippingCanvas() {
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
