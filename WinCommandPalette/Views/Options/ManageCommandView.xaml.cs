using System;
using System.Windows.Controls;
using WinCommandPalette.ViewModels.Options;

namespace WinCommandPalette.Views.Options
{
    /// <summary>
    /// Interaktionslogik für ManageCommandView.xaml
    /// </summary>
    public partial class ManageCommandView : UserControl
    {
        private ManageCommandViewModel viewModel;
        private Config config;

        public ManageCommandView(Config config)
        {
            this.config = config ??
                throw new ArgumentNullException(nameof(config));

            this.viewModel = new ManageCommandViewModel(this.config);

            this.InitializeComponent();
            this.DataContext = this.viewModel;
            this.btnSaveChanges.Click += this.viewModel.SaveChanges;
            this.btnCancel.Click += this.viewModel.Reset;
            this.btnDelete.Click += this.viewModel.Delete;
        }
    }
}
