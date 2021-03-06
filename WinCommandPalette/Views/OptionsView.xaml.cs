﻿using System;
using System.Collections.Generic;
using System.Windows;
using WinCommandPalette.Controls;
using WinCommandPalette.Helper;
using WinCommandPalette.Libs.Helper;
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
            this.SourceInitialized += this.OptionsView_SourceInitialized;
            this.DataContext = this.viewModel;

            this.btnSave.Click += this.BtnSave_Click;
            this.btnCancel.Click += this.BtnCancel_Click;
            this.Closing += this.OptionsView_Closing;
            this.Closed += this.OptionsView_Closed;
            HotKeyHelper.UnregisterHotKey();
        }

        private void OptionsView_SourceInitialized(object sender, EventArgs e)
        {
            WPFWinHelper.DisableTitleBarButtons(this, WPFWinHelper.TitleBarButtons.Maximaze);
        }

        private void OptionsView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.viewModel.IsModified())
            {
                var msgBoxResult = MessageBox.Show("You have unsaved changes.\r\nDo you want to save the changes or discard them.", "WinCommand Palette - Options", MessageBoxButton.YesNoCancel, MessageBoxImage.Information);
                if (msgBoxResult == MessageBoxResult.Yes)
                {
                    this.viewModel.Save();
                }
                else if (msgBoxResult == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }
        }

        private void OptionsView_Closed(object sender, EventArgs e)
        {
            HotKeyHelper.RegisterHotKey((uint)this.config.ModifierKey, this.config.KeyCode);
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