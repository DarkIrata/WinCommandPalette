using System.Drawing;

namespace WinCommandPalette.Plugin.CommandBase
{
    public interface ICommandBase
    {
        string Name { get; }

        string Description { get; }

        Image Icon { get; }

        bool RunInUIThread { get; }

        void Execute();
    }
}
