using System;
using System.Windows.Controls;
using WinCommandPalette.ViewModels.Options;

namespace WinCommandPalette.Views.Options
{
    public partial class CreateNewCommandView : UserControl, IOptionsPage
    {
        private CreateNewCommandViewModel viewModel;
        private Config config;

        public CreateNewCommandView(Config config)
        {
            this.config = config ??
                throw new ArgumentNullException(nameof(config));

            this.viewModel = new CreateNewCommandViewModel(this.config);

            this.InitializeComponent();
            this.DataContext = this.viewModel;
            this.btnAddCommand.Click += this.viewModel.AddCommand;
            this.btnReset.Click += this.viewModel.Reset;
        }

        public void Refresh()
        {
            this.viewModel.Refresh();
        }
    }
}
