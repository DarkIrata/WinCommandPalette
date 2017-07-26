using System;
using System.Drawing;
using WinCommandPalette.Plugin.CommandBase;
using WinCommandPalette.Views;

namespace WinCommandPalette.Commands
{
    internal class ShowAboutCommand : ICommandBase
    {
        public string Name => "About";

        public string Description => "Show WinCommand Palette about screen";

        public bool RunInUIThread => true;

        public Image Icon => null;

        private Config config;

        public ShowAboutCommand(Config config)
        {
            this.config = config ??
                throw new ArgumentNullException(nameof(config));
        }

        public void Execute()
        {
            var aboutWindow = new AboutView(this.config);
            aboutWindow.Show();
        }
    }
}
