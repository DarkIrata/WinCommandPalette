using System.Collections.Generic;

namespace CommandPalette.PluginSystem
{
    public interface IInstantCommands
    {
        List<ICommand> GetCommands();
    }
}
