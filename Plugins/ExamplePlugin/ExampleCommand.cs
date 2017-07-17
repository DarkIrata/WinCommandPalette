using System;
using System.Windows.Forms;
using WinCommandPalette.PluginSystem;

namespace ExamplePlugin
{
    public class ExampleCommand : ICommand
    {
        public string Name { get; set; }

        public string Description => "Anyone there?";

        public bool RunInUIThread => false;

        public string Text { get; set; }

        public ExampleCommand()
        {
            this.Name = "[Example] Name isn't set";
        }

        public void Execute()
        {
            MessageBox.Show(this.Text);
        }
    }
}
