using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinCommandPalette.Plugin;
using WinCommandPalette.Plugin.CommandBase;

namespace BaseCommandsPlugin
{
    public class BaseCommandsPlugin : WCPPlugin
    {
        public override PluginMeta PluginMeta => new PluginMeta()
        {
            Author = "DarkIrata",
            Description = "Plugin pack containing all default plugins for WCP."
        };

        public override void OnLoad()
        {
        }

        public override List<ICommandBase> AutoRegisterCommands => null;
    }
}
