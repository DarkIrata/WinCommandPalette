using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using CommandPalette.Attributes;
using CommandPalette.Commands;
using wf = System.Windows.Forms;

namespace CommandPalette
{
    public class OptionsViewModel : ViewModelBase
    {
        private Config config;
        private Config newConfig;
        private wf.KeysConverter keyConverter = new wf.KeysConverter();

        public string HotKey => this.GetHotkeyString();

        public List<Type> AvailableCommandTypes => this.GetAvailableCommandTypes();

        public bool CanSave => this.IsValid();

        private Type selectedItem;

        public Type SelectedItem
        {
            get
            {
                return this.selectedItem;
            }

            set
            {
                this.selectedItem = value;
                this.CreateControl = this.GetControlFromSelectedType();
                this.NotifyPropertyChanged(nameof(this.SelectedItem));
            }
        }

        private UserControl createControl;

        public UserControl CreateControl
        {
            get
            {
                return this.createControl;
            }

            set
            {
                this.createControl = value;
                this.NotifyPropertyChanged(nameof(this.CreateControl));
            }
        }

        public OptionsViewModel(Config config)
        {
            this.config = config ??
                throw new ArgumentNullException(nameof(config));

            this.newConfig = (Config)config.Clone();
        }

        private List<Type> GetAvailableCommandTypes()
        {
            var interfaceType = typeof(Commands.ICommand);
            var commandTypes = interfaceType.Assembly.GetTypes()
                .Where(p => interfaceType.IsAssignableFrom(p) && !p.IsInterface && p.CustomAttributes
                    .Any(x => x.AttributeType == typeof(CreateCommandControl)))
                .ToList();

            if (commandTypes == null)
            {
                return new List<Type>();
            }

            return commandTypes;
        }

        private UserControl GetControlFromSelectedType()
        {
            var type = this.SelectedItem;
            if (type == null)
            {
                return new UserControl();
            }

            var attributes = type.GetCustomAttributes(true);
            var viewTypeAttribute = attributes.FirstOrDefault(a => a is CreateCommandControl);
            if (viewTypeAttribute == null)
            {
                throw new ArgumentNullException(nameof(viewTypeAttribute));
            }

            var assembly = type.Assembly;
            var viewType = assembly.GetType(((CreateCommandControl)viewTypeAttribute).ControlType.FullName);
            if (viewType == null)
            {
                throw new ArgumentNullException(nameof(viewType));
            }

            return (UserControl)Activator.CreateInstance(viewType);
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
