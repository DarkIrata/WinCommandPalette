namespace CommandPalette.Commands
{
    public interface ICommand
    {
        string Name { get; }

        string Description { get; }

        bool RunInUIThread { get; }

        void Execute();
    }
}
