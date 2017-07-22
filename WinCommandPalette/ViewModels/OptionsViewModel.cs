using System;
using System.Collections.Generic;
using System.Linq;
using WinCommandPalette.Controls;
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
            new MenuItem("G E N E R A L", new GeneralView()),
            new MenuItem("C O M M A N D S", null,
                new SubMenuItem("CREATE NEW", new CreateNewCommandView()),
                new SubMenuItem("MANAGE", new GeneralView())
                ),
            new MenuItem("D E B U G", null),
        };

        public List<MenuItem> MenuItems => this.menuItems;

        private System.Windows.Controls.UserControl activePage;

        public System.Windows.Controls.UserControl ActivePage
        {
            get => this.activePage;
            set
            {
                this.activePage = value;
                this.NotifyPropertyChanged(nameof(this.ActivePage));
            }
        }

        private MenuItem selectedMenuItem;

        public MenuItem SelectedMenuItem
        {
            get => this.selectedMenuItem;

            set
            {
                if (this.SelectedSubMenuItem != null)
                {
                    this.SelectedSubMenuItem = null;
                }

                if (this.SelectedMenuItem != null)
                {
                    this.SelectedMenuItem.IsActive = false;
                }

                this.selectedMenuItem = value;
                this.SelectedMenuItem.IsActive = true;

                if (this.SelectedMenuItem.Page != null)
                {
                    this.ActivePage = this.SelectedMenuItem.Page;
                }
                else
                {
                    if (this.SelectedMenuItem.HasSubMenuItems)
                    {
                        this.SelectedSubMenuItem = this.SelectedMenuItem.SubMenuItems[0];
                    }
                    else
                    {
                        this.ActivePage = null;
                    }
                }

                this.NotifyPropertyChanged(nameof(this.SelectedMenuItem));
            }
        }

        private SubMenuItem selectedSubMenuItem;

        public SubMenuItem SelectedSubMenuItem
        {
            get => this.selectedSubMenuItem;

            set
            {
                if (this.SelectedSubMenuItem != null)
                {
                    this.SelectedSubMenuItem.IsActive = false;
                }

                if (value != null)
                {
                    if (this.SelectedMenuItem != value.MenuItem)
                    {
                        this.SelectedMenuItem = value.MenuItem;
                        this.SelectedSubMenuItem.IsActive = false;
                    }

                    this.selectedSubMenuItem = value;
                    this.SelectedSubMenuItem.IsActive = true;

                    this.ActivePage = this.SelectedSubMenuItem.Page;
                }

                this.NotifyPropertyChanged(nameof(this.SelectedSubMenuItem));
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