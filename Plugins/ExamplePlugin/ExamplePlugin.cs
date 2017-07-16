using System;
using System.Collections.Generic;
using WinCommandPalette.PluginSystem;

namespace ExamplePlugin
{
    public class ExamplePlugin : IInstantCommands
    {
        public List<ICommand> GetCommands()
        {
            return new List<ICommand>()
            {
                new ExampleCommand()
                {
                    Name = "[Example] Add by InstantCommand",
                    Text = "Hardcoded Command. Not saved in the Config"
                }
            };
        }
    }
}
