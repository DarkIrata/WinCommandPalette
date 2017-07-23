using System;
using System.Collections.Generic;
using System.Windows;
using WinCommandPalette.Controls;
using WinCommandPalette.Helper;
using WinCommandPalette.ViewModels;

namespace WinCommandPalette.Views
{
    public partial class OptionsView : Window
    {
        private Config config;
        private OptionsViewModel viewModel;

        public OptionsView(Config config)
        {
            this.config = config ??
                throw new ArgumentNullException(nameof(config));

            this.viewModel = new OptionsViewModel(this.config);

            this.InitializeComponent();
            this.DataContext = this.viewModel;

            this.Closed += this.OptionsView_Closed;
            Win32Helper.UnregisterHotKey();
        }

        private void OptionsView_Closed(object sender, EventArgs e)
        {
            Win32Helper.RegisterHotKey((uint)this.config.ModifierKey, this.config.KeyCode);
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            this.viewModel.Save();
            this.Close();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is MenuItem menuItem)
            {
                this.viewModel.SelectedMenuItem = menuItem;
            }
            else if (e.OriginalSource is SubMenuItem subMenuItem)
            {
                this.viewModel.SelectedSubMenuItem = subMenuItem;
            }
        }
    }
}