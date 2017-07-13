using CommandPalette.PluginSystem;

namespace CSharpScriptCommandsPlugin
{
    public class CSharpScriptCommand : ICommand
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public bool RunInUIThread => false;

        public string Code { get; set; }

        public void Execute()
        {
        }
    }
}
