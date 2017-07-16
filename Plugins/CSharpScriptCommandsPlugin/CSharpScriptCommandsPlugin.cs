using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandPalette.PluginSystem;

namespace CSharpScriptCommandsPlugin
{
    public class CSharpScriptCommandsPlugin : IInstantCommands
    {
        public List<ICommand> GetCommands()
        {
            return null;
        }

        public CSharpScriptCommandsPlugin()
        {
            Task.Run(() =>
            {
                // Running the compiler atleast one time will make him faster for further calls
                var cmd = new CSharpScriptCommand()
                {
                    Code = "using System;"
                };

                cmd.Execute();
            });
        }
    }
}