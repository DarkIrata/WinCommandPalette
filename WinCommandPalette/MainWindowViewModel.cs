using WinCommandPalette.Commands;
using WinCommandPalette.Helper;
using WinCommandPalette.PluginSystem;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace WinCommandPalette
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
            tb.CaretIndex++;
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

            // Setting SelectedIndex only one time doesn't execute probably, that's why it gets set 2 times
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
                    this.ExecuteCommand(command);
                }
                else
                {
                    // Fixs a dumbass bug where (for example) "OpenFileCommand" Commands with RunAsAdmin = true start its given path 2 times..
                    Task.Run(() => this.ExecuteCommand(command));
                }

                return true;
            }

            return false;
        }

        private void ExecuteCommand(ICommand command)
        {
            try
            {
                command.Execute();
            }
            catch (Exception)
            {
                MessageBox.Show($"Unkown error while executing command '{command.Name}'", "WinCommand Palette", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetupInstantCommands()
        {
            this.InstantCommands.Add(new ShowOptionsCommand(this.config));
            this.InstantCommands.Add(new QuitCommand());

            var pluginInstantCommands = PluginHelper.GetAll<IInstantCommands>();
            foreach (var pluginInstantCommand in pluginInstantCommands)
            {
                var commands = pluginInstantCommand.GetCommands();
                if (commands != null)
                {
                    this.InstantCommands.AddRange(commands);
                }
            }
        }
    }
}