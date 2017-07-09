using System;
using System.Windows;
using System.Windows.Input;
using wf = System.Windows.Forms;

namespace CommandPalette
{
    public class OptionsViewModel : ViewModelBase
    {
        private Config config;
        private Config newConfig;
        private wf.KeysConverter keyConverter = new wf.KeysConverter();

        public string HotKey
        {
            get => this.GetHotkeyString();
        }

        public bool CanSave
        {
            get => this.IsValid();
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
            string hotkey = string.Empty;

            foreach (ModifierKey value in Enum.GetValues(typeof(ModifierKey)))
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

        public OptionsViewModel(Config config)
        {
            this.config = config ??
                throw new ArgumentNullException(nameof(config));

            this.newConfig = (Config)config.Clone();
        }
    }
}
