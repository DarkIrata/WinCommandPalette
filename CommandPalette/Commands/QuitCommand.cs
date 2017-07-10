using System.Windows;
using CommandPalette.Attributes;

namespace CommandPalette.Commands
{
    public class QuitCommand : ICommand
    {
        public string Name => "Quit";

        public string Description => "Quit WinCommand Palette";

        public bool RunInUIThread => true;

        public QuitCommand()
        {
        }

        public void Execute()
        {
            Application.Current.Shutdown(0);
        }
    }
}
