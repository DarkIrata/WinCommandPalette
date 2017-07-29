using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using WinCommandPalette.Helper;
using WinCommandPalette.PluginSystem;
using WinCommandPalette.Views;

namespace WinCommandPalette
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        private const string CONFIG_FILE_NAME = "config.xml";
        private readonly string configDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "IrataProjects", "WinCommandPalette");
        private readonly string configFilePath;

        private Config config;

        public App()
        {
#if DEBUG
            this.configFilePath = Path.Combine(this.configDir, "debug", CONFIG_FILE_NAME);
#else
            this.configFilePath = Path.Combine(this.configDir, CONFIG_FILE_NAME);
#endif

            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
            {
                MessageBox.Show("An instance of WinCommand Palette is already running.", "WinCommand Palette", MessageBoxButton.OK, MessageBoxImage.Information);
                Current.Shutdown(1);
            }

            Directory.CreateDirectory(Path.GetDirectoryName(this.configFilePath));
            PluginHelper.Load();

            this.config = new Config();
            if (File.Exists(this.configFilePath))
            {
                this.config = Config.Load(this.configFilePath);
            }
            else
            {
                new OptionsView(this.config).Show();
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var mainWindow = new MainWindow(this.config);
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            this.config.Save(this.configFilePath);
            HotKeyHelper.UnregisterHotKey();
            base.OnExit(e);
        }
    }
}
