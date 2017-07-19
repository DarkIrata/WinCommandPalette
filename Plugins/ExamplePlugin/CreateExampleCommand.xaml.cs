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
            this.tbText.Text = string.Empty;
        }

        public ICommandBase GetCommand()
        {
            return new ExampleCommand()
            {
                Text = this.tbText.Text
            };
        }
    }
}
