using System;
using System.Collections.Generic;
using System.Windows.Controls;
using WinCommandPalette.Commands;
using WinCommandPalette.CreateCommandControls.ViewModel;
using WinCommandPalette.PluginSystem;

namespace WinCommandPalette.CreateCommandControls.View
{
    /// <summary>
    /// Interaktionslogik für CreateOpenFileCommand.xaml
    /// </summary>
    public partial class CreateOpenFileCommandView : UserControl, ICreateCommand
    {
        public string CommandTypeName => "OpenFile";

        private CreateOpenFileCommandViewModel viewModel;

        public CreateOpenFileCommandView()
        {
            this.InitializeComponent();
            this.viewModel = new CreateOpenFileCommandViewModel();
            this.DataContext = this.viewModel;
        }

        public ICommand GetCommand()
        {
            return this.viewModel.GetCommand();
        }

        public void ClearAll()
        {
            this.viewModel.ClearAll();
        }
    }
}
