using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WinCommandPalette.Controls;
using WinCommandPalette.CreateCommandControls.View;
using WinCommandPalette.Enums;
using WinCommandPalette.Plugin.CreateCommand;
using WinCommandPalette.PluginSystem;
using wf = System.Windows.Forms;
using WinCommandPalette.Views.Options;

namespace WinCommandPalette.ViewModels
{
    public class OptionsViewModel : ViewModelBase
    {
        private Config config;
        private Config newConfig;
        private wf.KeysConverter keyConverter = new wf.KeysConverter();

        private List<MenuItem> menuItems = new List<MenuItem>()
        {
            { new MenuItem("G E N E R A L", new GeneralView())},
            { new MenuItem("C O M M A N D S", new CreateNewCommandView()) },
        };

        public List<MenuItem> MenuItems => this.menuItems;

        private MenuItem selectedMenuItem;

        public MenuItem SelectedMenuItem
        {
            get
            {
                return this.selectedMenuItem;
            }

            set
            {
                if (this.selectedMenuItem != null)
                {
                    this.selectedMenuItem.IsActive = false;
                }

                this.selectedMenuItem = value;
                this.selectedMenuItem.IsActive = true;
                this.NotifyPropertyChanged(nameof(this.SelectedMenuItem));
            }
        }

        // public string HotKey => this.GetHotkeyString();

        public OptionsViewModel(Config config)
        {
            this.config = config ??
                throw new ArgumentNullException(nameof(config));

            this.newConfig = (Config)config.Clone();

            this.SelectedMenuItem = this.MenuItems.FirstOrDefault();
        }


        //public List<ICreateCommand> AvailableCommandCreators => this.GetAvailableCommandCreators();
        //private List<ICreateCommand> GetAvailableCommandCreators()
        //{
        //    var commandCreators = new List<ICreateCommand>();
        //    // Internal
        //    commandCreators.Add(new CreateOpenFileCommandView());

        //    // Plugins
        //    commandCreators.AddRange(PluginHelper.GetAllCreateCommandViews());

        //    return commandCreators;
        //}

        //internal void KeyBox_PreviewKeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.LeftShift || 
        //        e.Key == Key.LeftCtrl ||
        //        e.Key == Key.LeftAlt ||
        //        e.Key == Key.LWin)
        //    {
        //        e.Handled = true;
        //        return;
        //    }

        //    var modifier = ModifierKey.None;
        //    if(Keyboard.IsKeyDown(Key.LeftShift))
        //    {
        //        modifier |= ModifierKey.LeftShift;
        //    }

        //    if (Keyboard.IsKeyDown(Key.LeftCtrl))
        //    {
        //        modifier |= ModifierKey.LeftCTRL;
        //    }

        //    if (Keyboard.IsKeyDown(Key.LeftAlt))
        //    {
        //        modifier |= ModifierKey.ALT;
        //    }

        //    if (Keyboard.IsKeyDown(Key.LWin))
        //    {
        //        modifier |= ModifierKey.Win;
        //    }

        //    this.newConfig.ModifierKey = modifier;
        //    if (Enum.TryParse(e.Key.ToString(), true, out wf.Keys keyCode))
        //    {
        //        this.newConfig.KeyCode = (uint)keyCode;
        //    }

        //    this.NotifyPropertyChanged(nameof(this.HotKey));
        //    e.Handled = true;
        //}

        internal void Save()
        {
            this.config.UpdateConfig(this.newConfig);
        }

        //private string GetHotkeyString()
        //{
        //    var hotkey = string.Empty;

        //    foreach (var value in Enum.GetValues(typeof(ModifierKey)).Cast<ModifierKey>())
        //    {
        //        if (value == ModifierKey.None)
        //        {
        //            continue;
        //        }

        //        if (this.newConfig.ModifierKey.HasFlag(value))
        //        {
        //            hotkey += value + " + ";
        //        }
        //    }

        //    if (Enum.TryParse(this.newConfig.KeyCode.ToString(), true, out wf.Keys keyCode))
        //    {
        //        hotkey += keyCode.ToString();
        //    }

        //    if (hotkey.EndsWith(" + "))
        //    {
        //        hotkey = hotkey.Substring(0, hotkey.Length - 3);
        //    }

        //    return hotkey;
        //}
    }
}