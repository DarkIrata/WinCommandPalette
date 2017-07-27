using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WinCommandPalette.Plugin.CommandBase;
using WinCommandPalette.Plugin.CreateCommand;
using WinCommandPalette.PluginSystem;
using WinCommandPalette.Views.Options;

namespace WinCommandPalette.ViewModels.Options
{
    public class ManageCommandViewModel : ViewModelBase
    {
        private Config config;
        private ICreateCommand noCommandCreateView = new CreateCommandNotFound();

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
                this.NotifyPropertyChanged(nameof(this.SelectedItem));
            }
        }

        public bool CanManage => this.SelectedItem != null && this.CommandCreator != this.noCommandCreateView;

        public bool CanDelete => this.SelectedItem != null;

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
                this.NotifyPropertyChanged(nameof(this.CanManage));
                this.NotifyPropertyChanged(nameof(this.CanDelete));
            }
        }

        private ICreateCommand commandCreator;

        public ICreateCommand CommandCreator
        {
            get => this.commandCreator;

            set
            {
                this.commandCreator = value;

                this.CommandCreator?.ShowCommand(this.SelectedItem);
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
                else
                {
                    this.CommandCreator = this.TryGetFromFromExecutingAssembly();
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
            if (this.Commands.Count > 0)
            {
                this.SelectedItem = null;
                this.CommandCreator = null;
                this.SelectedIndex = 0;
            }

            this.CommandCreator?.ShowCommand(this.SelectedItem);
            this.NotifyPropertyChanged(nameof(this.CommandCreator));
        }

        internal void SaveChanges(object sender, RoutedEventArgs e)
        {
            var command = this.CommandCreator?.GetCommand();
            if (command != null)
            {
                if (string.IsNullOrEmpty(command.Name))
                {
                    MessageBox.Show($"Command Name cant be empty", "WinCommand Palette - Options", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var commandsCopy = this.Commands;
                var index = commandsCopy.IndexOf(this.SelectedItem);
                commandsCopy[index] = command;

                this.Commands = commandsCopy;
                this.SelectedIndex = index;

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

            if (MessageBox.Show($"Are you sure you want to delete '{this.SelectedItem?.Name}'?", "WinCommand Palette - Options", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                return;
            }

            var commandsCopy = this.Commands;
            var index = commandsCopy.IndexOf(this.SelectedItem);
            commandsCopy.RemoveAt(index);
            this.Commands = commandsCopy;

            if (commandsCopy.Count > 0)
            {
                this.SelectedIndex = --index;
            }
        }

        // This Dictionary and Method will be removed if intern commands with a create dialog gets moved to their own plugins
        private Dictionary<string, ICreateCommand> internCreateCommand = new Dictionary<string, ICreateCommand>();
        private ICreateCommand TryGetFromFromExecutingAssembly()
        {
            var commandName = this.SelectedItem.GetType().Name;

            if (this.internCreateCommand.ContainsKey(commandName))
            {
                return this.internCreateCommand[commandName];
            }

            return this.noCommandCreateView;
        }
    }
}
