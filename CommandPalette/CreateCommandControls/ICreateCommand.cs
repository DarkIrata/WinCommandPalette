using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using CommandPalette.Commands;

namespace CommandPalette.CreateCommandControls
{
    public interface ICreateCommand
    {
        string CommandTypeName { get; }

        ICommand CreateCommand();

        void ClearAll();
    }
}
