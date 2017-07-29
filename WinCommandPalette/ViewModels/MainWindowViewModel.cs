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
using WinCommandPalette.Plugin.CommandBase;
using WinCommandPalette.Libs.Helper;

namespace WinCommandPalette.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private Config config;
        private List<ICommandBase> AutoRegisterCommands = new List<ICommandBase>();
        private string lastSearchText = string.Empty;

        private ObservableCollection<ICommandBase> filteredCommandList;

        public ObservableCollection<ICommandBase> FilteredCommandList
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

            this.FilteredCommandList = new ObservableCollection<ICommandBase>();
            this.RegisterAutoRegisterCommands();
        }

        public void ShowAllCommands()
        {
            var commands = this.config.Commands.ToList();
            commands.AddRange(this.AutoRegisterCommands);
            this.FilteredCommandList = new ObservableCollection<ICommandBase>(commands.OrderBy(k => k.Name).ToList());
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

            var diffedCommands = new Dictionary<ICommandBase, int>();
            this.GetCommandsWithDif(diffedCommands, this.config.Commands, searchText);
            this.GetCommandsWithDif(diffedCommands, this.AutoRegisterCommands, searchText);

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

        private void GetCommandsWithDif(Dictionary<ICommandBase, int> diffedCommands, List<ICommandBase> commands, string searchText)
        {
            foreach (var command in commands)
            {
                var commandName = this.GetSearchString(command.Name);
                var dif = LevenshteinDistance.Compute(searchText, commandName);
                if (dif > (commandName.Length - searchText.Length))
                {
                    continue;
                }

                if (commandName.IndexOf(searchText) > -1)
                {
                    dif--;
                }

                diffedCommands.Add(command, dif);
            }
        }

        private string GetSearchString(string input)
        {
            return input.Trim().Replace(" ", "").ToLower();
        }

        internal bool Execute()
        {
            if (this.SelectedItem is ICommandBase command)
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

        private void ExecuteCommand(ICommandBase command)
        {
            try
            {
                command.Execute();
            }
            catch (Exception ex)
            {
                // TODO: LOG EXCEPTION
                Console.WriteLine(ex.Message);
                MessageBox.Show($"Unhandled error while executing command '{command.Name}'.\r\n\r\n{ex.Message}", "WinCommand Palette", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RegisterAutoRegisterCommands()
        {
            // Internal
            this.AutoRegisterCommands.Add(new ShowAboutCommand(this.config));
            this.AutoRegisterCommands.Add(new ShowOptionsCommand(this.config));
            this.AutoRegisterCommands.Add(new QuitCommand());

            // Plugins
            this.AutoRegisterCommands.AddRange(PluginHelper.GetAllAutoRegisterCommands());
        }
    }
}