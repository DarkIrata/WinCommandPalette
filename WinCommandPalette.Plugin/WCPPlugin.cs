using System.Collections.Generic;
using WinCommandPalette.Plugin.CommandBase;

namespace WinCommandPalette.Plugin
{
    public abstract class WCPPlugin
    {
        public abstract PluginMeta PluginMeta { get; }

        public virtual void OnLoad()
        {
        }

        public virtual List<ICommandBase> AutoRegisterCommands => null;
    }
}