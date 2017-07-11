using System;
using CommandPalette.PluginSystem;

namespace CommandPalette.Commands
{
    public class ShowOptionsCommand : ICommand
    {
        public string Name => "Options";

        public string Description => "Show WinCommand Palette options";


        private Config config;

        public bool RunInUIThread => true;

        public ShowOptionsCommand(Config config)
        {
            this.config = config ??
                throw new ArgumentNullException(nameof(config));
        }

        public void Execute()
        {
            var optionsWindow = new OptionsView(this.config);
            optionsWindow.Show();
        }
    }
}