using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinCommandPalette.PluginSystem;

namespace WinCommandPalette.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        public int PluginsLoadedCount => PluginHelper.PluginsCount();

        public int CommandsLoadedCount => this.config.Commands.Count;

        public int NotCommandsLoadedCount => this.config.UndeserializableCommands.Count;

        private Config config;

        public AboutViewModel(Config config)
        {
            this.config = config ??
                throw new ArgumentNullException(nameof(config));
        }
    }
}
