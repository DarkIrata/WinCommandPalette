using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WinCommandPalette.Plugin;

namespace CSharpScriptCommandsPlugin
{
    public class CSharpScriptCommandsPlugin : WCPPlugin
    {
        public override PluginMeta PluginMeta => throw new NotImplementedException();

        public override void OnCreation()
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
            base.OnCreation();
        }
    }
}