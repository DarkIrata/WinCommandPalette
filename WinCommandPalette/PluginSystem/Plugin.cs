using System.Collections.Generic;
using System.Reflection;
using WinCommandPalette.Plugin;
using WinCommandPalette.Plugin.CommandBase;
using WinCommandPalette.Plugin.CreateCommand;

namespace WinCommandPalette.PluginSystem
{
    public struct Plugin
    {
        public Assembly Assembly { get; }

        public WCPPlugin WCPPlugin { get; }

        public Dictionary<string, ICreateCommand> Commands { get; }

        public Plugin(Assembly pluginAssembly, WCPPlugin wcpPlugin, Dictionary<string, ICreateCommand> commands)
        {
            this.Assembly = pluginAssembly;
            this.WCPPlugin = wcpPlugin;
            this.Commands = commands;
        }
    }
}
