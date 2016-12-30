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
using Tickets.CommonUI;

namespace Tickets {
    public sealed partial class RulesPage : Page {
        public RulesPage() {
            this.InitializeComponent();
        }

        #region Event Handlers
        protected override void OnNavigatedTo( NavigationEventArgs e ) {
            SQLiteDataAccessor sql = new SQLiteDataAccessor();
            var list = sql.CreateQuery<Chapters>();
            Chapters c = list.Where(i=>i.id==6).ToArray()[0];

            myRtb.DataContext = c.content;
            RichTextBlockContent.onBlockTapped += RichTextBlockContent_onBlockTapped;

            base.OnNavigatedTo(e);
        }

        void RichTextBlockContent_onBlockTapped( object sender, HLContent e ) {
            System.Diagnostics.Debug.WriteLine("e.type: {0}, e.data: {1}", e.Type, e.Data);
        }
        #endregion
    }
}
