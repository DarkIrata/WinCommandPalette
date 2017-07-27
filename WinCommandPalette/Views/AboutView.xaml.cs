using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using WinCommandPalette.Helper;
using WinCommandPalette.ViewModels;

namespace WinCommandPalette.Views
{
    /// <summary>
    /// Interaktionslogik für AboutView.xaml
    /// </summary>
    public partial class AboutView : Window
    {
        private Config config;
        private AboutViewModel viewModel;

        public AboutView(Config config)
        {
            this.config = config ??
                throw new ArgumentNullException(nameof(config));

            this.viewModel = new AboutViewModel(this.config);

            this.InitializeComponent();
            this.DataContext = this.viewModel;
            Win32Helper.UnregisterHotKey();

            this.Closing += this.AboutView_Closing;
            this.topPanel.MouseDown += this.TopPanel_MouseDown;
            this.btnClose.Click += this.BtnClose_Click;

            this.mMaier.MouseUp += this.MMaier_MouseUp;
            this.gCookie.MouseUp += this.GCookie_MouseUp;

            this.gitHubLogo.MouseUp += this.GitHubLogo_MouseUp;
            this.ipLogo.MouseUp += this.IpLogo_MouseUp;
        }

        private void MMaier_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://github.com/maurice-maier");
        }

        private void GCookie_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://github.com/g0dsCookie");
        }

        private void IpLogo_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://ipmix.de/");
        }

        private void GitHubLogo_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://github.com/DarkIrata/WinCommandPalette");
        }

        private void AboutView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Win32Helper.RegisterHotKey((uint)this.config.ModifierKey, this.config.KeyCode);
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TopPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}
