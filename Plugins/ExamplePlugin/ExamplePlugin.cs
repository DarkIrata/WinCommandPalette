using System;
using System.Collections.Generic;
using System.Windows;
using WinCommandPalette.Plugin;
using WinCommandPalette.Plugin.CommandBase;

namespace ExamplePlugin
{
    public class ExamplePlugin : WCPPlugin
    {
        public override PluginMeta PluginMeta => new PluginMeta()
        {
            Author = "DarkIrata",
            Description = "A simple example how to create a plugin"
        };

        public override void OnLoad()
        {
            MessageBox.Show("CREATED X");
        }

        public override List<ICommandBase> AutoRegisterCommands => new List<ICommandBase>()
        {
            new ExampleCommand()
            {
                Name = "[Example] Add by GetAutoRegisterCommands",
                Text = "Hardcoded Commands are not saved in the Config"
            }
        };
    }
}
