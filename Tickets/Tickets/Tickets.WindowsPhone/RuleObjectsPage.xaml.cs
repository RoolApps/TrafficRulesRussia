using SQLiteShared.Models;
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
using Utils.Extensions;
using Tickets.CommonUI;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Tickets
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RuleObjectsPage : Page
    {
        public RuleObjectsPage()
        {
            this.NavigationCacheMode = NavigationCacheMode.Required;
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode != NavigationMode.Back)
            {
                Canvas.SetZIndex(this.rulesCanvas, -1);

                this.rulesCanvas.DataSource = null;
                this.cmbSections.DataContext = null;
                this.cmbSections.DataContext = AppLogic.Static.PreloadedContent.RuleChapters.Data;
                this.cmbSections.SelectedItem = AppLogic.Static.PreloadedContent.RuleChapters.Data.First();
            }
        }

        private void rich_onBlockTapped(object sender, HLContent e)
        {
            this.Frame.Navigate(typeof(SignMarkPage), GetCommonObject(e.Type, e.Data));
        }

        private CommonObject GetCommonObject(String type, String num)
        {
            switch (type)
            {
                case "signs":
                    return new CommonObject(AppLogic.Static.PreloadedContent.Signs.Data.Single(sign => sign.num == num));
                case "marks":
                    return new CommonObject(AppLogic.Static.PreloadedContent.Marks.Data.Single(mark => mark.num == num));
                default:
                    return null;
            }
        }

        private void cmbSections_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var rich = rulesCanvas.FindChildrenOfType<ExtendedRichTextBlock>().FirstOrDefault();
            if (rich != null)
            {
                rich.onBlockTapped -= rich_onBlockTapped;
            }
            rulesCanvas.DataSource = new VirtualLinkedList<Chapters>(e.AddedItems.OfType<Chapters>(),
                (dataSource, current) =>
                {
                    return dataSource.FirstOrDefault();
                },
                (dataSource, current) =>
                {
                    
                    this.Frame.Navigate(typeof(MainPage));
                    return null;
                }
            );

            rich = rulesCanvas.FindChildrenOfType<ExtendedRichTextBlock>().LastOrDefault();
            if (rich != null)
            {
                rich.onBlockTapped += rich_onBlockTapped;
            }
        }
    }
}
