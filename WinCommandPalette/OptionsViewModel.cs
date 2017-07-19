using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using WinCommandPalette.CreateCommandControls.View;
using WinCommandPalette.Plugin.CreateCommand;
using WinCommandPalette.PluginSystem;
using wf = System.Windows.Forms;

namespace WinCommandPalette
{
    public class OptionsViewModel : ViewModelBase
    {
        private Config config;
        private Config newConfig;
        private wf.KeysConverter keyConverter = new wf.KeysConverter();

        public string HotKey => this.GetHotkeyString();

        public List<ICreateCommand> AvailableCommandCreators => this.GetAvailableCommandCreators();

        public bool CanSave => this.IsValid();

        private ICreateCommand selectedItem;

        public ICreateCommand SelectedItem
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

        public OptionsViewModel(Config config)
        {
            this.config = config ??
                throw new ArgumentNullException(nameof(config));

            this.newConfig = (Config)config.Clone();
        }

        private List<ICreateCommand> GetAvailableCommandCreators()
        {
            var commandCreators = new List<ICreateCommand>();
            // Internal
            commandCreators.Add(new CreateOpenFileCommandView());

            // Plugins
            commandCreators.AddRange(PluginHelper.GetAllCreateCommandViews());

            return commandCreators;
        }

        private bool IsValid()
        {
            return true;
        }

        internal void KeyBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift || 
                e.Key == Key.LeftCtrl ||
                e.Key == Key.LeftAlt ||
                e.Key == Key.LWin)
            {
                e.Handled = true;
                return;
            }

            var modifier = ModifierKey.None;
            if(Keyboard.IsKeyDown(Key.LeftShift))
            {
                modifier |= ModifierKey.LeftShift;
            }

            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                modifier |= ModifierKey.LeftCTRL;
            }

            if (Keyboard.IsKeyDown(Key.LeftAlt))
            {
                modifier |= ModifierKey.ALT;
            }

            if (Keyboard.IsKeyDown(Key.LWin))
            {
                modifier |= ModifierKey.Win;
            }

            this.newConfig.ModifierKey = modifier;
            if (Enum.TryParse(e.Key.ToString(), true, out wf.Keys keyCode))
            {
                this.newConfig.KeyCode = (uint)keyCode;
            }

            this.NotifyPropertyChanged(nameof(this.HotKey));
            e.Handled = true;
        }

        internal void BtnSaveNewCommand_Click(object sender, RoutedEventArgs e)
        {
            var command = this.SelectedItem?.GetCommand();
            if (command != null)
            {
                this.newConfig.Commands.Add(command);
                this.SelectedItem.ClearAll();
            }
        }

        internal void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            this.selectedItem?.ClearAll();
        }

        internal void Save()
        {
            this.config.UpdateConfig(this.newConfig);
        }

        private string GetHotkeyString()
        {
            var hotkey = string.Empty;

            foreach (var value in Enum.GetValues(typeof(ModifierKey)).Cast<ModifierKey>())
            {
                if (value == ModifierKey.None)
                {
                    continue;
                }

                if (this.newConfig.ModifierKey.HasFlag(value))
                {
                    hotkey += value + " + ";
                }
            }

            if (Enum.TryParse(this.newConfig.KeyCode.ToString(), true, out wf.Keys keyCode))
            {
                hotkey += keyCode.ToString();
            }

            if (hotkey.EndsWith(" + "))
            {
                hotkey = hotkey.Substring(0, hotkey.Length - 3);
            }

            return hotkey;
        }
    }
}