using System;
using System.Drawing;
using System.Windows;
using WinCommandPalette.Plugin.CommandBase;
using WinCommandPalette.PluginSystem;

namespace WinCommandPalette.Commands
{
    internal class QuitCommand : ICommandBase
    {
        public string Name => "Quit";

        public string Description => "Quit WinCommand Palette";

        public bool RunInUIThread => true;

        public Image Icon => null;

        public void Execute()
        {
            Application.Current.Shutdown(0);
        }
    }
}
