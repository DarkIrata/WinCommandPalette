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

namespace CommandPalette.CreateCommandControls
{
    /// <summary>
    /// Interaktionslogik für CreateOpenFileCommand.xaml
    /// </summary>
    public partial class CreateOpenFileCommand : CreateCommandBase
    {
        public override string CommandTypeName => "OpenFile";

        public CreateOpenFileCommand()
        {
            this.InitializeComponent();
        }

        public override Commands.ICommand CreateCommand()
        {
            return base.CreateCommand();
        }
    }
}
