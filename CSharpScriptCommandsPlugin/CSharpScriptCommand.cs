using CommandPalette.PluginSystem;
using Microsoft.CodeAnalysis.CSharp.Scripting;

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
            var result = CSharpScript.EvaluateAsync<string>(this.Code).Result;
            if (!string.IsNullOrEmpty(result))
            {
                System.Windows.MessageBox.Show(result);
            }
        }
    }
}
