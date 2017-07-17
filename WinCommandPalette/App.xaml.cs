using System.Diagnostics;
using System.IO;
using System.Windows;
using WinCommandPalette.Helper;
using WinCommandPalette.PluginSystem;

namespace WinCommandPalette
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        private Config config;

        public App()
        {

            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
            {
                MessageBox.Show("An instance of WinCommand Palette is already running.", "WinCommand Palette", MessageBoxButton.OK, MessageBoxImage.Information);
                Current.Shutdown(1);
            }

            PluginHelper.Load();

            this.config = new Config();
            if (File.Exists("config.xml"))
            {
                this.config = Config.Load("config.xml");
            }
            else
            {
                new OptionsView(this.config).Show();
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var mainWindow = new MainWindow(this.config)
            {
                Top = ScreenHelper.GetPrimaryScreen().Bounds.Height * 0.40
            };

            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            this.config.Save("config.xml");
            Win32Helper.UnregisterHotKey();
            base.OnExit(e);
        }
    }
}
