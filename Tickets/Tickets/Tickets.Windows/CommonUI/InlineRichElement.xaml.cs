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

// Шаблон элемента пользовательского элемента управления задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234236

namespace Tickets.CommonUI {
    public sealed partial class InlineRichElement : UserControl {
       public InlineRichElement() {
            this.Tapped += onParagraphTapped;
            this.InitializeComponent();
        }

        public string Text {
            get { return (String)GetValue(TextBlockTextProperty); }
            set { SetValue(TextBlockTextProperty, value); }
        }

        public static readonly DependencyProperty TextBlockTextProperty =
            DependencyProperty.Register("myText", typeof(String), typeof(InlineRichElement),
            new PropertyMetadata(String.Empty, OnTextBoxTextPropertyChanged));

        private static void OnTextBoxTextPropertyChanged( DependencyObject d, DependencyPropertyChangedEventArgs e ) {
            (d as InlineRichElement).textblock.Text = e.NewValue.ToString();
        }

        public string Type {
            get { return (String)GetValue(TextBlockSignProperty); }
            set { SetValue(TextBlockSignProperty, value); }
        }

        public static readonly DependencyProperty TextBlockSignProperty =
            DependencyProperty.Register("myType", typeof(String), typeof(InlineRichElement),
            null);

        private void onParagraphTapped( object sender, TappedRoutedEventArgs e ) {
            e.Handled = true;
            System.Diagnostics.Debug.WriteLine("tapped, text: {0}, type: {1}", Text, Type);
        }
    }
}
