using System;
using System.Windows.Controls;

namespace CommandPalette.CreateCommandControls
{
    /// <summary>
    /// Interaktionslogik für CreateOpenFileCommand.xaml
    /// </summary>
    public partial class CreateOpenFileCommand : UserControl, ICreateCommand
    {
        public string CommandTypeName => "OpenFile";

        public CreateOpenFileCommand()
        {
            this.InitializeComponent();
        }

        public Commands.ICommand CreateCommand()
        {
            return null;
        }

        public void ClearAll()
        {
        }
    }
}
