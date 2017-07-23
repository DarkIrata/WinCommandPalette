using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using WinCommandPalette.Enums;
using wsh = IWshRuntimeLibrary;
using wf = System.Windows.Forms;


namespace WinCommandPalette.ViewModels.Options
{
    public class GeneralViewModel : ViewModelBase
    {
        public string HotKey => this.GetHotkeyString();

        private string shortcutFilePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "WinCommandPalette.lnk");
        public bool RunWinWindows
        {
            get => File.Exists(this.shortcutFilePath);
            set
            {
                if (value)
                {
                    this.CreateShortcut();
                }
                else
                {
                    if (this.RunWinWindows)
                    {
                        try
                        {
                            File.Delete(this.shortcutFilePath);
                        }
                        catch { }
                    }
                }

                this.NotifyPropertyChanged(nameof(this.RunWinWindows));
            }
        }

        private void CreateShortcut()
        {
            try
            {
                var shell = new wsh.WshShell();
                var shortcut = (wsh.IWshShortcut)shell.CreateShortcut(this.shortcutFilePath);
                shortcut.TargetPath = Assembly.GetExecutingAssembly().Location;

                shortcut.Save();
            }
            catch (Exception)
            {
                MessageBox.Show($"Error adding to startup.", "WinCommand Palette PluginLoader", MessageBoxButton.OK, MessageBoxImage.Error);
                this.RunWinWindows = false;
            }
        }

        private Config config;
        private wf.KeysConverter keyConverter = new wf.KeysConverter();

        public GeneralViewModel(Config config)
        {
            this.config = config ??
                throw new ArgumentNullException(nameof(config));
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
            if (Keyboard.IsKeyDown(Key.LeftShift))
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

            this.config.ModifierKey = modifier;
            if (Enum.TryParse(e.Key.ToString(), true, out wf.Keys keyCode))
            {
                this.config.KeyCode = (uint)keyCode;
            }

            this.NotifyPropertyChanged(nameof(this.HotKey));
            e.Handled = true;
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

                if (this.config.ModifierKey.HasFlag(value))
                {
                    hotkey += value + " + ";
                }
            }

            if (Enum.TryParse(this.config.KeyCode.ToString(), true, out wf.Keys keyCode))
            {
                var keyCodeString = this.KeyCodeToUnicode(keyCode);
                hotkey += keyCodeString.ToUpper();
            }

            if (hotkey.EndsWith(" + "))
            {
                hotkey = hotkey.Substring(0, hotkey.Length - 3);
            }

            return hotkey;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetKeyboardLayout(uint idThread);

        [DllImport("user32.dll")]
        public static extern int ToUnicodeEx(uint virtualKeyCode, uint scanCode, byte[] keyboardState, StringBuilder receivingBuffer, int bufferSize, uint flags, IntPtr dwhkl);

        public string KeyCodeToUnicode(wf.Keys key)
        {
            StringBuilder result = new StringBuilder();
            ToUnicodeEx((uint)key, 0, new byte[256], result, 5, 0, GetKeyboardLayout(0));
            return result.ToString();
        }
    }
}
