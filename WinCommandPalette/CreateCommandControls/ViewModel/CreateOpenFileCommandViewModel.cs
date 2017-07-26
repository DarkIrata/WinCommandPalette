using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinCommandPalette.Commands;
using WinCommandPalette.Plugin.CommandBase;
using WinCommandPalette.PluginSystem;

namespace WinCommandPalette.CreateCommandControls.ViewModel
{
    public class CreateOpenFileCommandModel : ViewModelBase
    {
        private OpenFileCommand openFileCommand = new OpenFileCommand();

        public OpenFileCommand OpenFileCommand
        {
            get => this.openFileCommand;

            set
            {
                this.openFileCommand = value;
                this.NotifyPropertyChanged(nameof(this.OpenFileCommand));
            }
        }

        internal ICommandBase GetCommand()
        {
            return this.OpenFileCommand;
        }

        internal void ClearAll()
        {
            this.OpenFileCommand = new OpenFileCommand();
        }

        internal void ShowCommand(OpenFileCommand ofcommand)
        {
            this.OpenFileCommand = ofcommand;
        }
    }
}
