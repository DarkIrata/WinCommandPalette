using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WinCommandPalette.CreateCommandControls.View;
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
                this.SelectedPlugin = PluginHelper.GetPlugins().Where(p => p.Commands.ContainsValue(this.SelectedItem)).FirstOrDefault();
                this.NotifyPropertyChanged(nameof(this.SelectedItem));
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
                this.NotifyPropertyChanged(nameof(this.CanManage));
                this.NotifyPropertyChanged(nameof(this.SelectedItem));
            }
        }

        public bool CanManage => this.SelectedIndex > -1;

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
            if (this.SelectedIndex == -1 && 
                this.AvailableCommandCreators.Count > 0)
            {
                this.SelectedIndex = 0;
            }
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

            // Plugins
            commandCreators.AddRange(this.GetAllCreateCommandViews());

            // Internal
            commandCreators.Add(new CreateOpenFileCommandView());

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
