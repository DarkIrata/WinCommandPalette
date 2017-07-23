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
        private Config BackupConfig;

        private List<MenuItem> menuItems = null;

        public List<MenuItem> MenuItems
        {
            get => this.menuItems;
            set => this.menuItems = value;
        }

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

        public OptionsViewModel(Config config)
        {
            this.config = config ??
                throw new ArgumentNullException(nameof(config));

            this.BackupConfig = (Config)config.Clone();

            this.MenuItems = new List<MenuItem>()
            {
                new MenuItem("G E N E R A L", new GeneralView(config)),
                new MenuItem("C O M M A N D S", null,
                    new SubMenuItem("CREATE NEW", new CreateNewCommandView()),
                    new SubMenuItem("MANAGE", null)
                    )
            };

            this.SelectedMenuItem = this.MenuItems.FirstOrDefault();
        }

        internal void Save()
        {
        }
    }
}