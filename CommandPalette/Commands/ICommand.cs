using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandPalette.Commands
{
    public interface ICommand
    {
        string Name { get; }

        string Description { get; }

        bool RunInUIThread { get; }

        void Execute();
    }
}
