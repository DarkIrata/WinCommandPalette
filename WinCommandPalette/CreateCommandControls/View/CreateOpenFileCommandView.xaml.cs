using System;
using System.Collections.Generic;
using System.Windows.Controls;
using WinCommandPalette.Commands;
using WinCommandPalette.CreateCommandControls.ViewModel;
using WinCommandPalette.Plugin.CommandBase;
using WinCommandPalette.Plugin.CreateCommand;
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

        public ICommandBase GetCommand()
        {
            return this.viewModel.GetCommand();
        }

        public void ShowCommand(ICommandBase command)
        {
            if (command is OpenFileCommand ofcommand)
            {
                this.viewModel.ShowCommand(ofcommand);
            }
        }

        public void ClearAll()
        {
            this.viewModel.ClearAll();
        }
    }
}
