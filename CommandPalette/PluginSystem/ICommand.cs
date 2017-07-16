using System.Windows.Controls;

namespace CommandPalette.PluginSystem
{
    public interface ICommand
    {
        string Name { get; }

        string Description { get; }

        bool RunInUIThread { get; }

        bool RunAsAdmin { get; }

        void Execute();
    }
}
