using CommandPalette.Commands;
using CommandPalette.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CommandPalette
{
    public class MainWindowViewModel : ViewModelBase
    {
        private Config config;
        private List<ICommand> InstantCommands = new List<ICommand>();
        private string lastSearchText = string.Empty;

        private ObservableCollection<ICommand> filteredCommandList;

        public ObservableCollection<ICommand> FilteredCommandList
        {
            get
            {
                return this.filteredCommandList;
            }

            set
            {
                this.filteredCommandList = value;
            }
        }
        
        private int selectedIndex = -1;

        public int SelectedIndex
        {
            get
            {
                return this.selectedIndex;
            }

            set
            {
                this.selectedIndex = value;
                this.NotifyPropertyChanged(nameof(this.SelectedIndex));
            }
        }

        private object selectedItem;

        public object SelectedItem
        {
            get
            {
                return this.selectedItem;
            }

            set
            {
                this.selectedItem = value;
                this.NotifyPropertyChanged(nameof(this.SelectedItem));
            }
        }

        public MainWindowViewModel(Config config)
        {
            this.config = config ??
                throw new ArgumentNullException(nameof(config));

            this.FilteredCommandList = new ObservableCollection<ICommand>();
            this.SetupInstantCommands();
        }

        public void ShowAllCommands()
        {
            var commands = this.config.Commands.ToList();
            commands.AddRange(this.InstantCommands);
            this.FilteredCommandList = new ObservableCollection<ICommand>(commands.OrderBy(k => k.Name).ToList());
            this.NotifyPropertyChanged(nameof(this.FilteredCommandList));
        }

        internal void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = (TextBox)sender;
            var searchText = tb.Text.Trim();
            
            if (searchText == this.lastSearchText)
            {
                return;
            }

            if (searchText.Length == 0)
            {
                this.ShowAllCommands();
                this.lastSearchText = string.Empty;
                return;
            }

            searchText = this.GetSearchString(searchText);
            this.FilteredCommandList.Clear();

            var diffedCommands = new Dictionary<ICommand, int>();
            foreach (var command in this.config.Commands)
            {
                var commandName = this.GetSearchString(command.Name);
                var dif = LevenshteinDistance.Compute(searchText, commandName);
                if (dif > (commandName.Length - searchText.Length))
                {
                    continue;
                }

                diffedCommands.Add(command, dif);
            }

            foreach (var command in this.InstantCommands)
            {
                var commandName = this.GetSearchString(command.Name);
                var dif = LevenshteinDistance.Compute(searchText, commandName);
                if (dif > (commandName.Length - searchText.Length))
                {
                    continue;
                }

                diffedCommands.Add(command, dif);
            }

            var sortedDiffedCommands = diffedCommands.OrderBy(c => c.Value);
            foreach (var item in sortedDiffedCommands)
            {
                this.FilteredCommandList.Add(item.Key);
            }

            this.SelectedIndex = -1;
            if (this.FilteredCommandList.Count != 0)
            {
                this.SelectedIndex = 0;
            }

            this.lastSearchText = searchText;
        }

        private string GetSearchString(string input)
        {
            return input.Trim().Replace(" ", "").ToLower();
        }

        internal bool Execute()
        {
            if (this.SelectedItem is ICommand command)
            {
                if (command.RunInUIThread)
                {
                    command.Execute();
                }
                else
                {
                    // Fixes a dumbass bug where (for example) "OpenFileCommand" Commands with RunAsAdmin = true start 2 times..
                    Task.Run(() => command.Execute());
                }

                return true;
            }

            return false;
        }

        private void SetupInstantCommands()
        {
            this.InstantCommands.Add(new ShowOptionsCommand(this.config));
            this.InstantCommands.Add(new QuitCommand());

#if DEBUG
            this.InstantCommands.Add(new OpenFileCommand()
            {
                Name = "CMD Admin",
                Description = "Kommandozeile",
                FileName = "cmd.exe",
                RunAsAdmin = true,
                RunInUIThread = false
            });
#endif
        }
    }
}