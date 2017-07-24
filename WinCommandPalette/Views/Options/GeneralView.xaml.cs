using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WinCommandPalette.ViewModels.Options;

namespace WinCommandPalette.Views.Options
{
    /// <summary>
    /// Interaktionslogik für GeneralView.xaml
    /// </summary>
    public partial class GeneralView : UserControl, IOptionsPage
    {
        private GeneralViewModel viewModel;
        private Config config;

        public GeneralView(Config config)
        {
            this.config = config ??
                throw new ArgumentNullException(nameof(config));

            this.viewModel = new GeneralViewModel(this.config);

            this.InitializeComponent();
            this.DataContext = this.viewModel;
            this.KeyBox.PreviewKeyDown += this.viewModel.KeyBox_PreviewKeyDown;
        }

        public void Refresh()
        {
        }
    }
}
