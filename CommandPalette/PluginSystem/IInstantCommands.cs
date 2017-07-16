using System.Collections.Generic;

namespace WinCommandPalette.PluginSystem
{
    public interface IInstantCommands
    {
        List<ICommand> GetCommands();
    }
}
