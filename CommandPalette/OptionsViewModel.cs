using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using CommandPalette.Commands;
using CommandPalette.CreateCommandControls;
using wf = System.Windows.Forms;

namespace CommandPalette
{
    public class OptionsViewModel : ViewModelBase
    {
        private Config config;
        private Config newConfig;
        private wf.KeysConverter keyConverter = new wf.KeysConverter();

        public string HotKey => this.GetHotkeyString();

        public List<CreateCommandBase> AvailableCommandCreators => this.GetAvailableCommandCreators();

        public bool CanSave => this.IsValid();

        private CreateCommandBase selectedItem;

        public CreateCommandBase SelectedItem
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

        private List<CreateCommandBase> GetAvailableCommandCreators()
        {
            var commandCreators = new List<CreateCommandBase>();

            var baseType = typeof(CreateCommandBase);
            var commandCreatorsTypes = baseType.Assembly.GetTypes()
                .Where(p => baseType.IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract & p != baseType)
                ?.ToList();

            if (commandCreatorsTypes == null)
            {
                return new List<CreateCommandBase>();
            }

            foreach (var type in commandCreatorsTypes)
            {
                commandCreators.Add((CreateCommandBase)Activator.CreateInstance(type));
            }

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
