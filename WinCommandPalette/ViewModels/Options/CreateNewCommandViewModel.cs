using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using WinCommandPalette.Plugin.CreateCommand;
using WinCommandPalette.PluginSystem;

namespace WinCommandPalette.ViewModels.Options
{
    public class CreateNewCommandViewModel : ViewModelBase
    {
        private Config config;

        public List<ICreateCommand> AvailableCommandCreators => this.GetAvailableCommandCreators();

        private ICreateCommand selectedItem;

        public ICreateCommand SelectedItem
        {
            get => this.selectedItem;

            set
            {
                this.selectedItem = value;
                this.SelectedItem?.ClearAll();
                this.SelectedPlugin = PluginHelper.GetPlugins().Where(p => p.Commands.ContainsValue(this.SelectedItem)).FirstOrDefault();
                this.NotifyPropertyChanged(nameof(this.SelectedItem));
                this.NotifyPropertyChanged(nameof(this.CanManage));
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

        public bool CanManage => this.SelectedItem != null;

        private PluginSystem.Plugin selectedPlugin;

        public PluginSystem.Plugin SelectedPlugin
        {
            get => this.selectedPlugin;

            set
            {
                this.selectedPlugin = value;
                this.NotifyPropertyChanged(nameof(this.SelectedPlugin));
            }
        }

        public CreateNewCommandViewModel(Config config)
        {
            this.config = config ??
                throw new ArgumentNullException(nameof(config));
        }

        internal void Refresh()
        {
            if (this.AvailableCommandCreators.Count > 0)
            {
                this.SelectedItem = null;
                this.SelectedIndex = 0;
            }

            this.SelectedItem?.ClearAll();
            this.NotifyPropertyChanged(nameof(this.SelectedItem));
        }

        internal void AddCommand(object sender, RoutedEventArgs e)
        {
            var command = this.SelectedItem?.GetCommand();

            if (command != null)
            {
                if (string.IsNullOrEmpty(command.Name))
                {
                    MessageBox.Show($"Command Name cant be empty", "WinCommand Palette - Options", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                this.config.Commands.Add(command);
                this.SelectedItem.ClearAll();
                MessageBox.Show($"Command '{command.Name}' added!", "WinCommand Palette - Options", MessageBoxButton.OK, MessageBoxImage.None);
            }
        }

        internal void Reset(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want reset all changes to this new command?", "WinCommand Palette - Options", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                this.SelectedItem?.ClearAll();
            }
        }

        private List<ICreateCommand> GetAvailableCommandCreators()
        {
            var commandCreators = new List<ICreateCommand>();
            commandCreators.AddRange(this.GetAllCreateCommandViews());

            return commandCreators;
        }

        private List<ICreateCommand> GetAllCreateCommandViews()
        {
            var createCommandViews = new List<ICreateCommand>();

            foreach (var plugin in PluginHelper.GetPlugins())
            {
                if (plugin.Commands?.Values != null)
                {
                    createCommandViews.AddRange(plugin.Commands.Values.Where(v => v != null));
                }
            }

            return createCommandViews;
        }
    }
}
