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
using XAMLMarkup;
using Utils;
using AppLogic;
using AppLogic.Enums;
using AppLogic.Interfaces;
using Tickets;

namespace Tickets
{
    public sealed partial class QuestionsContentPage : Page
    {
        public QuestionsContentPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ISession session = e.Parameter as ISession;
            PagedCanvas paged_canvas = flipping_canvas.CanvasContent as PagedCanvas;
            PagedCollection<IQuestion> paged_col = new PagedCollection<IQuestion>(2);
            paged_col.DataSource = session.Questions;
            paged_canvas.DataSource = paged_col;
        }
        
    }
}
