using System;
using System.Windows.Controls;
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
    }
}
