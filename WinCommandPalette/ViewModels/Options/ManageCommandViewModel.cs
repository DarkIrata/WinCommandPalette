using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WinCommandPalette.Plugin.CommandBase;
using WinCommandPalette.Plugin.CreateCommand;
using WinCommandPalette.PluginSystem;

namespace WinCommandPalette.ViewModels.Options
{
    public class ManageCommandViewModel : ViewModelBase
    {
        private Config config;

        public ObservableCollection<ICommandBase> Commands
        {
            get => new ObservableCollection<ICommandBase>(this.config.Commands);

            set
            {
                this.config.Commands = value.ToList();
                this.NotifyPropertyChanged(nameof(this.Commands));
            }
        }

        private int selectedIndex = -1;

        public int SelectedIndex
        {
            get => this.selectedIndex;

            set
            {
                this.selectedIndex = value;
                this.NotifyPropertyChanged(nameof(this.SelectedIndex));
            }
        }


        private ICommandBase selectedItem;

        public ICommandBase SelectedItem
        {
            get => this.selectedItem;

            set
            {
                this.selectedItem = value;
                if (this.SelectedItem != null)
                {
                    this.SelectedPlugin = PluginHelper.GetPlugins().Where(p => p.Commands.ContainsKey(this.SelectedItem.GetType().FullName)).FirstOrDefault();
                }

                this.NotifyPropertyChanged(nameof(this.SelectedItem));
            }
        }

        private ICreateCommand commandCreator;

        public ICreateCommand CommandCreator
        {
            get => this.commandCreator;

            set
            {
                this.commandCreator = value;
                if (this.CommandCreator != null)
                {
                    this.CommandCreator.ShowCommand(this.SelectedItem);
                }
                this.NotifyPropertyChanged(nameof(this.CommandCreator));
            }
        }

        private PluginSystem.Plugin selectedPlugin;

        public PluginSystem.Plugin SelectedPlugin
        {
            get => this.selectedPlugin;

            set
            {
                this.selectedPlugin = value;
                if (this.SelectedPlugin.Assembly != null)
                {
                    this.CommandCreator = this.SelectedPlugin.Commands[this.SelectedItem.GetType().FullName];
                }
                this.NotifyPropertyChanged(nameof(this.SelectedPlugin));
            }
        }

        public ManageCommandViewModel(Config config)
        {
            this.config = config ??
                throw new ArgumentNullException(nameof(config));
        }

        internal void Refresh()
        {
            this.NotifyPropertyChanged(nameof(this.Commands));
        }

        internal void SaveChanges(object sender, RoutedEventArgs e)
        {
            var command = this.CommandCreator?.GetCommand();
            if (command != null)
            {
                var commandsCopy = this.Commands;
                var index = commandsCopy.IndexOf(this.SelectedItem);
                commandsCopy[index] = command;

                this.Commands = commandsCopy;
                this.selectedIndex = index;

                MessageBox.Show($"Command '{command.Name}' updated!", "WinCommand Palette - Options", MessageBoxButton.OK, MessageBoxImage.None);
            }
        }

        internal void Reset(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want reset all changes to this command?", "WinCommand Palette - Options", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                this.CommandCreator?.ShowCommand(this.SelectedItem);
            }
        }

        internal void Delete(object sender, RoutedEventArgs e)
        {
            var commandsCopy = this.Commands;
            var index = commandsCopy.IndexOf(this.SelectedItem);
            commandsCopy.RemoveAt(index);
            this.Commands = commandsCopy;

            if (commandsCopy.Count > 0)
            {
                this.SelectedIndex = --index;
            }
        }
    }
}
