using WinCommandPalette.Plugin.CommandBase;

namespace WinCommandPalette.Plugin.CreateCommand
{
    public interface ICreateCommand
    {
        string CommandTypeName { get; }

        string CommandDescription { get; }

        ICommandBase GetCommand();

        void ShowCommand(ICommandBase command);

        void ClearAll();
    }
}
