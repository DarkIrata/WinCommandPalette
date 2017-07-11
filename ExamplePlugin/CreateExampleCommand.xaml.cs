using System.Windows.Controls;
using CommandPalette.CreateCommandControls;

namespace ExamplePlugin
{
    /// <summary>
    /// Interaction logic for CreateExampleCommand.xaml
    /// </summary>
    public partial class CreateExampleCommand : UserControl, ICreateCommand
    {
        public CreateExampleCommand()
        {
            this.InitializeComponent();
        }

        public string CommandTypeName => "Example Plugin";

        public void ClearAll()
        {
            this.tbText.Text = string.Empty;
        }

        public CommandPalette.Commands.ICommand CreateCommand()
        {
            return new ExampleCommand()
            {
                Text = this.tbText.Text
            };
        }
    }
}
