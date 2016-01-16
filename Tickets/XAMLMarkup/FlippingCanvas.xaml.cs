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
        private string animationProperty = "(Canvas.Left)";

        private double X_position = 0.0;

        private const int window_h = 768;
        private const int window_w = 1366;

        private Point last_p;
        private Point initial_p;

        private Boolean isPressed;

        private List<Storyboard> animations;

        DispatcherTimer timer;
        #endregion

        public FlippingCanvas()
        {
            this.InitializeComponent();

            /*
            timer = new DispatcherTimer();
            timer.Tick += tick;
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Start();
            */

            isPressed = false;
            animations = new List<Storyboard>();
        }

        /*
        private void tick(object sender, object e)
        {

        }
        */

        private void SelectPosition()
        {
            X_position = GetXPosition(corrector);

            foreach (var obj in canvas.Children.ToList()) {
                if (X_position != 0.0) {
                    if (X_position < 0.0) {
                        if (X_position >= -(window_w / 2)) {
                            AnimateObject(obj, Math.Abs(X_position));
                        } else if (X_position < -(window_w / 2)) {
                            AnimateObject(obj, -(window_w - Math.Abs(X_position)));
                        }
                    } else if (X_position > 0.0) {
                        if (X_position <= (window_w / 2)) {
                            AnimateObject(obj, -(Math.Abs(X_position)));
                        } else if (X_position > (window_w / 2)) {
                            AnimateObject(obj, (window_w - Math.Abs(X_position)));
                        }
                    }
                }
            }
        }

        private void grid_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            isPressed = true;
            last_p = initial_p = e.GetCurrentPoint(this).Position;   //(this) = (sender as canvas)

            if (animations.Count != 0) {
                foreach (Storyboard sb in animations) {
                    sb.Pause();
                }

                animations.Clear();
            }
        }

        private void grid_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            isPressed = false;
            SelectPosition();
        }

        private void grid_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            double px = 0;

            if (isPressed) {
                Point current_point = e.GetCurrentPoint(this).Position;
                px = Distance(last_p, current_point);
                MoveObjectsOnCanvas(px);
                last_p = current_point;
            }
        }

        private void AnimateObject(DependencyObject obj, double dist)
        {
            Storyboard storyboard = new Storyboard();

            ExponentialEase easingFunction = new ExponentialEase();
            easingFunction.EasingMode = EasingMode.EaseInOut;

            DoubleAnimation doubleanimation = new DoubleAnimation() {
                Duration = new Duration(TimeSpan.FromMilliseconds(1000)),
                EnableDependentAnimation = true,
                By = dist,
                EasingFunction = easingFunction
            };

            Storyboard.SetTarget(doubleanimation, obj);
            Storyboard.SetTargetProperty(doubleanimation, animationProperty);

            storyboard.Children.Add(doubleanimation);
            storyboard.Completed += storyboard_Completed;

            animations.Add(storyboard);
            storyboard.Begin();
        }

        private void storyboard_Completed(object sender, object e)
        {
            animations.Remove(sender as Storyboard);
            SetXPosition(corrector, 0);
        }

        private void MoveObjectsOnCanvas(double px)
        {
            var objects = canvas.Children.ToList();
            foreach (var obj in objects) {
                Canvas.SetLeft(obj, GetXPosition(obj) + px);
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

        private void Ellipse_Loaded(object sender, RoutedEventArgs e)
        {
            //AnimateObject(sender as Ellipse, 500);
        }

        private void canvas_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var obj in canvas.Children.ToList()) {
                AnimateObject(obj, 500);
            }
        }
    }
}
