using System;
using System.Windows.Controls;
using CommandPalette.Commands;

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

        public ICommand CreateCommand()
        {
            return new OpenFileCommand()
            {
                Name = "A",
                Description = "Not used",
                FileName = "",
                WorkingDirectory = "",
                Arguments = "",
                RunAsAdmin = false,
                RunInUIThread = true
            };
        }

        public void ClearAll()
        {
        }
    }
}
