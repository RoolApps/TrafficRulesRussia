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
using XAMLMarkup;


namespace XAMLMarkup
{
    public sealed partial class FlippingCanvas : UserControl
    {
        #region Variable
        private enum Moved : int { NoWhere, ToPrevious, ToNext } ;
        private const string animationProperty = "(Canvas.Left)";
        private const int animationDuration = 1500;

        private double X_position = 0.0;
        private int animated_objects = 0;
        private static int completed_animations = 0;
        private Moved direction;
        
        //private const int window_h = 768;
        //private const int window_w = 1366;
        private double window_h = Window.Current.Bounds.Height;
        private double window_w = Window.Current.Bounds.Width;

        private Point last_p;
        private Point initial_p;

        private Boolean isPressed = false;

        DispatcherTimer timer;

        Storyboard storyboard;

        List<UIElement> elements;

        PagedCanvas paged_canvas = null;
        #endregion

        public static readonly DependencyProperty ContentProperty = 
            DependencyProperty.Register("CanvasContent", typeof(object), typeof(FlippingCanvas), null);

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
            if (CanvasContent as UIElement != null) {
                canvas.Children.Add(CanvasContent as UIElement);
            }

            if (CanvasContent is PagedCanvas) {
                paged_canvas = this.CanvasContent as PagedCanvas;
            }
        }

        private void tick(object sender, object e)
        {
            //ystem.Diagnostics.Debug.WriteLine("width: " + window_w);
            //System.Diagnostics.Debug.WriteLine("height: " + window_h);
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
            completed_animations++;
            if (completed_animations == animated_objects) {
                if (this.direction == Moved.ToNext) {
                    System.Diagnostics.Debug.WriteLine("paged_canvas.LoadNext();");
                    paged_canvas.LoadNext();
                } else if (this.direction == Moved.ToPrevious) {
                    System.Diagnostics.Debug.WriteLine("paged_canvas.LoadPrevious();");
                    paged_canvas.LoadPrevious();
                }
                completed_animations = 0;
            }
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
            //System.Diagnostics.Debug.WriteLine("X_POS: " + X_position.ToString());
            this.animated_objects = canvas.Children.Count;

            if (X_position != 0.0){
                this.direction = Moved.NoWhere;
                if(X_position < 0.0){
                    SlideForward();
                } else if (X_position > 0.0) {
                    SlideBack();
                }
            }
        }

        private void SlideBack()
        {
            double to = 0.0;
            double px = PxToMove();
            //System.Diagnostics.Debug.WriteLine("px to move forward: " + px.ToString());
            // Если расстояние слишком большое для успешного перехода на следующее окно, остаемся в текущем окне
            if (px > window_w / 2) {
                to = -X_position;
            } else {
                to = window_w - X_position;
                this.direction = Moved.ToPrevious;
            }

            foreach (var obj in canvas.Children.ToList()) {
                AddAnimation(obj, animationProperty, GetXPosition(obj), GetXPosition(obj) + to, animationDuration, new ExponentialEase(), completed);
            }
        }

        private void SlideForward()
        {
            double to = 0.0;
            double px = PxToMove();
            //System.Diagnostics.Debug.WriteLine("px to move back: " + px.ToString());
            // Если расстояние слишком большое для успешного перехода на предыдущее окно, остаемся в текущем окне
            if (px > -window_w / 2) {
                to = -(window_w + X_position);
                this.direction = Moved.ToNext;
            } else {
                to = Math.Abs(X_position);
            }

            foreach (var obj in canvas.Children.ToList()) {
                AddAnimation(obj, animationProperty, GetXPosition(obj), GetXPosition(obj) + to, animationDuration, new ExponentialEase(), completed);
            }
        }

        private void MoveObjectsOnCanvas(double px)
        {
            var objects = canvas.Children.ToList();
            foreach (var obj in objects) {
                SetXPosition(obj, GetXPosition(obj) + px);
            }
        }

        private double PxToMove()
        {
            // Возвращает расстояние до следующего окна
            return (X_position < 0) ? -(window_w + X_position) : (window_w - X_position);
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

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.window_h = e.NewSize.Height;
            this.window_w = e.NewSize.Width;
        }
    }
}
