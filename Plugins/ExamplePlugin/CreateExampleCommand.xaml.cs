using System.Windows.Controls;
using WinCommandPalette.Plugin.CommandBase;
using WinCommandPalette.Plugin.CreateCommand;

namespace ExamplePlugin
{
    /// <summary>
    /// Interaction logic for CreateExampleCommand.xaml
    /// </summary>
    public partial class CreateExampleCommand : UserControl, ICreateCommand
    {
        public string CommandTypeName => "Example Plugin";

        public CreateExampleCommand()
        {
            this.InitializeComponent();
        }

        public void ClearAll()
        {
            this.tbName.Text = string.Empty;
            this.tbText.Text = string.Empty;
        }

        public ICommandBase GetCommand()
        {
            return new ExampleCommand()
            {
                Text = this.tbText.Text,
                Name = this.tbName.Text
            };
        }

        public void ShowCommand(ICommandBase command)
        {
            if (command is ExampleCommand ecommand)
            {
                this.tbName.Text = ecommand.Name;
                this.tbText.Text = ecommand.Text;
            }
        }
    }
}
