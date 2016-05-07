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
using System.Collections.ObjectModel;

namespace Tickets {
    public sealed partial class ResultsPage : Page {
        private ISession session;

        #region Constructor
        public ResultsPage() {
            this.InitializeComponent();
        }
        #endregion

        private void loadTickets() {

        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            session = e.Parameter as ISession;
            firstGrid.ItemsSource = session.Tickets;
        }

        private void SemanticZoom_ViewChangeStarted(object sender, SemanticZoomViewChangedEventArgs e) {
            if (e.SourceItem.Item == null)
                return;

            e.DestinationItem = new SemanticZoomLocation {
                Item = e.SourceItem.Item
            };
        }

    }

    public class IsAnsweredQuestion : IValueConverter {
        const String Answered = "Green";
        const String NotAnswered = "Red";

        public object Convert(object value, Type targetType, object parameter, string language) {
            return (bool)value ? Answered : NotAnswered;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }
    }
}
