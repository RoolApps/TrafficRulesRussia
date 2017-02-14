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
    public sealed partial class SignMarkPage : Page
    {
        public SignMarkPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var commonObject = e.Parameter as CommonObject;

            var dataItem = new { Image = commonObject.Image, Content = commonObject.Description };
            var data = new[] { dataItem };

            var stack = CreateStack(dataItem);

            var linkedList = VirtualLinkedListFactory.Create(data,
                (dataSource, current) =>
                {
                    return current;
                },
                (dataSource, current) =>
                {
                    var item = stack.Pop();
                    if(item == null)
                    {
                        if(this.Frame.CanGoBack)
                        {
                            this.Frame.GoBack();
                        }
                        else
                        {
                            this.Frame.Navigate(typeof(MainPage));
                        }
                        return null;
                    }
                    else
                    {
                        return item;
                    }
                }
            );

            this.canvas.DataSource = linkedList;
            var rtb = this.canvas.FindChildrenOfType<ExtendedRichTextBlock>().Single();

            EventHandler<CommonUI.HLContent> blockTapped = null;

            blockTapped = (s, ea) =>
            {
                rtb.onBlockTapped -= blockTapped;

                var defaultReturnNext = linkedList.ReturnNext;

                linkedList.ReturnNext = (dataSource, current) =>
                {
                    linkedList.ReturnNext = defaultReturnNext;

                    stack.Push(current);

                    var prmtr = GetCommonObject(ea.Type, ea.Data);

                    var dtItm = new { Image = prmtr.Image, Content = prmtr.Description };

                    return dtItm;
                };

                this.canvas.ChangeCanvasContent(true);

                this.canvas.FindChildrenOfType<ExtendedRichTextBlock>().Last().onBlockTapped += blockTapped;
            };

            rtb.onBlockTapped += blockTapped;
        }

        private void ExtendedRichTextBlock_onBlockTapped(object sender, CommonUI.HLContent e)
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

        private Stack<T> CreateStack<T>(T prototype) where T: class
        {
            var stack = new Stack<T>();
            stack.Push(null);
            return stack;
        }
    }
}
