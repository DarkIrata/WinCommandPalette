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
using CommandPalette.Commands;
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
