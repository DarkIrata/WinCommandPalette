using System.Windows.Controls;
using WinCommandPalette.Plugin.CommandBase;
using WinCommandPalette.Plugin.CreateCommand;

namespace BaseCommandsPlugin.OpenFile
{
    public partial class CreateOpenFileCommand : UserControl, ICreateCommand
    {
        public string CommandTypeName => "OpenFile";

        private CreateOpenFileCommandModel viewModel;

        public CreateOpenFileCommand()
        {
            this.InitializeComponent();
            this.viewModel = new CreateOpenFileCommandModel();
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