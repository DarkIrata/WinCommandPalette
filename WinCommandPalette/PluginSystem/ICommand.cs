using System.Windows.Controls;

namespace WinCommandPalette.PluginSystem
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
