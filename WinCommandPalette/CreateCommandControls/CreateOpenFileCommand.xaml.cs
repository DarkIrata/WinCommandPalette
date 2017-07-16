using System.Windows.Controls;
using WinCommandPalette.Commands;
using WinCommandPalette.PluginSystem;

namespace WinCommandPalette.CreateCommandControls
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

        public ICommand GetCommand()
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
