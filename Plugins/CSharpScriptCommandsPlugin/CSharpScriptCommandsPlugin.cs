using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WinCommandPalette.Plugin;

namespace CSharpScriptCommandsPlugin
{
    public class CSharpScriptCommandsPlugin : WCPPlugin
    {
        public override PluginMeta PluginMeta => null;

        public override void OnLoad()
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
            base.OnLoad();
        }
    }
}