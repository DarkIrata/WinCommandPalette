using System;
using System.Windows;
using WinCommandPalette.PluginSystem;

namespace WinCommandPalette.Commands
{
    internal class QuitCommand : ICommand
    {
        public string Name => "Quit";

        public string Description => "Quit WinCommand Palette";

        public bool RunInUIThread => true;

        public bool RunAsAdmin => false;

        public void Execute()
        {
            Application.Current.Shutdown(0);
        }
    }
}
