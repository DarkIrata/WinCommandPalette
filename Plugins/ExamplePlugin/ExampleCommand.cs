using System;
using System.Windows.Forms;
using System.Drawing;
using WinCommandPalette.Plugin.CommandBase;

namespace ExamplePlugin
{
    public class ExampleCommand : ICommandBase
    {
        public string Name { get; set; }

        public string Description => "What a suprising Description";

        public bool RunInUIThread => false;

        public string Text { get; set; }

        public Image Icon => null;

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
