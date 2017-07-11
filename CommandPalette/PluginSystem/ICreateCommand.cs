namespace CommandPalette.PluginSystem
{
    public interface ICreateCommand
    {
        string CommandTypeName { get; }

        ICommand GetCommand();

        void ClearAll();
    }
}
