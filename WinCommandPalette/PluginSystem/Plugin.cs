using System.Collections.Generic;
using System.Reflection;
using WinCommandPalette.Plugin;
using WinCommandPalette.Plugin.CommandBase;
using WinCommandPalette.Plugin.CreateCommand;

namespace WinCommandPalette.PluginSystem
{
    internal class Plugin
    {
        internal Assembly Assembly { get; }

        internal WCPPlugin WCPPlugin { get; }

        internal Dictionary<string, ICreateCommand> Commands { get; }

        public Plugin(Assembly pluginAssembly, WCPPlugin wcpPlugin, Dictionary<string, ICreateCommand> commands)
        {
            this.Assembly = pluginAssembly;
            this.WCPPlugin = wcpPlugin;
            this.Commands = commands;
        }
    }
}
