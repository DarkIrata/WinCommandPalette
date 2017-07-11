using System.Windows.Forms;
using CommandPalette.PluginSystem;

namespace ExamplePlugin
{
    public class ExampleCommand : ICommand
    {
        public string Name => "Example Command via Plugin";

        public string Description => "Anyone there?";

        public bool RunInUIThread => false;

        public string Text { get; set; }

        public void Execute()
        {
            MessageBox.Show(this.Text);
        }
    }
}
