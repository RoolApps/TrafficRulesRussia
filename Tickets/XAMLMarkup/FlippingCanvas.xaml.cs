using System;
using System.Collections.Generic;
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
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Input;
using Windows.UI.Xaml.Media.Animation;
using System.Threading;
using System.Collections;


namespace XAMLMarkup
{
    public sealed partial class FlippingCanvas : UserControl
    {
        #region Variable
        private const string animationProperty = "(Canvas.Left)";
        private const int animationDuration = 1500;

        private double X_position = 0.0;

        private const int window_h = 768;
        private const int window_w = 1366;

        private Point last_p;
        private Point initial_p;

        private Boolean isPressed = false;

        DispatcherTimer timer;

        Storyboard storyboard;

        List<UIElement> elements;
        #endregion

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("CanvasContent", typeof(object), typeof(FlippingCanvas), null);
        public object CanvasContent
        {
            get
            {
                return GetValue(ContentProperty);
            }
            set
            {
                SetValue(ContentProperty, value);
            }
        }

        public FlippingCanvas()
        {
            this.InitializeComponent();

            canvas.PointerPressed += canvas_PointerPressed;
            canvas.PointerReleased += canvas_PointerReleased;
            canvas.PointerMoved += canvas_PointerMoved;
            canvas.Loaded += canvas_Loaded;

            timer = new DispatcherTimer();
            timer.Tick += tick;
            timer.Interval = TimeSpan.FromMilliseconds(200);
            timer.Start();
        }

        void canvas_Loaded(object sender, RoutedEventArgs e)
        {
            canvas.Children.Add(CanvasContent as UIElement);
        }

        private void tick(object sender, object e)
        {

        }

        private void canvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            isPressed = true;
            last_p = initial_p = e.GetCurrentPoint(this).Position;   //(this) = (sender as Canvas)

            if (storyboard != null) {
                storyboard.Pause();
            }

            storyboard = new Storyboard();
        }

        private void canvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            isPressed = false;

            SelectPosition();
            storyboard.Begin();
        }

        private void canvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            double px = 0;

            if (isPressed) {
                Point current_point = e.GetCurrentPoint(this).Position;
                px = Distance(last_p, current_point);
                MoveObjectsOnCanvas(px);
                last_p = current_point;
            }
        }

        private void completed()
        {
            SetXPosition(corrector, 0.0);
        }

        private void AddAnimation(DependencyObject obj, String property, double from, double to, int duration, EasingFunctionBase easing = null, Action completed = null)
        {
            DoubleAnimation animation = new DoubleAnimation();

            if (easing != null) {
                animation.EasingFunction = easing;
                easing.EasingMode = EasingMode.EaseInOut;
            }

            animation.Duration = TimeSpan.FromMilliseconds(duration);
            animation.EnableDependentAnimation = true;
            animation.From = from;
            animation.To = to;

            Storyboard.SetTarget(animation, obj);
            Storyboard.SetTargetProperty(animation, property);

            storyboard.Children.Add(animation);

            if (completed != null) {
                storyboard.Completed += (s, e) => completed();
            }
        }

        private void SelectPosition()
        {
            X_position = GetXPosition(corrector);
            elements = canvas.Children.ToList();

            foreach (var element in elements) {
                if (X_position != 0.0) {
                    if (X_position < 0.0) {
                        if (X_position >= -(window_w / 2)) {
                            AddAnimation(element, animationProperty, GetXPosition(element), (GetXPosition(element) + Math.Abs(X_position)), animationDuration, new ExponentialEase(), completed);
                        } else if (X_position < -(window_w / 2)) {
                            AddAnimation(element, animationProperty, GetXPosition(element), GetXPosition(element) - (window_w - Math.Abs(X_position)), animationDuration, new ExponentialEase(), completed);
                        }
                    } else if (X_position > 0.0) {
                        if (X_position <= (window_w / 2)) {
                            AddAnimation(element, animationProperty, GetXPosition(element), (GetXPosition(element) - Math.Abs(X_position)), animationDuration, new ExponentialEase(), completed);
                        } else if (X_position > (window_w / 2)) {
                            AddAnimation(element, animationProperty, GetXPosition(element), GetXPosition(element) + (window_w - Math.Abs(X_position)), animationDuration, new ExponentialEase(), completed);
                        }
                    }
                }
            }
        }

        private void MoveObjectsOnCanvas(double px)
        {
            var objects = canvas.Children.ToList();
            foreach (var obj in objects) {
                SetXPosition(obj, GetXPosition(obj) + px);
            }
        }

        private void SetXPosition(UIElement obj, double X)
        {
            Canvas.SetLeft(obj, X);
        }

        private void SetYPosition(UIElement obj, double Y)
        {
            Canvas.SetTop(obj, Y);
        }

        private double GetXPosition(UIElement obj)
        {
            return (Canvas.GetLeft(obj));
        }

        private double GetYPosition(UIElement obj)
        {
            return (Canvas.GetTop(obj));
        }

        private double Distance(Point first_point, Point second_point)
        {
            return (double)(second_point.X - first_point.X);
        }
    }
}
