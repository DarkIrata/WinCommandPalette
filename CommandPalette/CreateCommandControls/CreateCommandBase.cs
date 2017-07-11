using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using CommandPalette.Commands;

namespace CommandPalette.CreateCommandControls
{
    public class CreateCommandBase : UserControl
    {
        public virtual string CommandTypeName => throw new ArgumentException($"'{nameof(CommandTypeName)}' was not overridden");

        public virtual ICommand CreateCommand()
        {
            return null;
        }
    }
}
