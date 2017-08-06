using System;
using System.Windows.Controls;
using WinCommandPalette.Libs.Controls;
using WinCommandPalette.ViewModels.Options;

namespace WinCommandPalette.Views.Options
{
    /// <summary>
    /// Interaktionslogik für DebugView.xaml
    /// </summary>
    public partial class DebugView : UserControl, IOptionsPage
    {
        public DebugView()
        {
            InitializeComponent();
        }

        public void Refresh()
        {
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //this.NotifyBar.ShowNotification(NoticeType.Error, "Ohhh noo BOOOIII", 30, new System.Windows.Duration(new TimeSpan(0, 0, 5)));
        }
    }
}
