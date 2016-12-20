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
using SQLiteShared;
using SQLiteShared.Models;

namespace Tickets {
    public sealed partial class RulesPage : Page {
        public RulesPage() {
            this.InitializeComponent();
        }

        #region Event Handlers
        protected override void OnNavigatedTo( NavigationEventArgs e ) {
            SQLiteDataAccessor sql = new SQLiteDataAccessor();
            var list = sql.CreateQuery<Chapters>();
            Chapters c = list.First();

            // < Эта штука, чтобы знать откуда брать элемент InlineRichElement
                commonBlock.Ns = "using:Tickets.CommonUI";
            // >
            commonBlock.Content = c.content;

            base.OnNavigatedTo(e);
        }
        #endregion
    }
}
