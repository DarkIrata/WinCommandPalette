using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using CommandPalette.Helper;
using CommandPalette.PluginSystem;

namespace CommandPalette
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        private Config config;

        public App()
        {
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
            var mainWindow = new MainWindow(this.config);
            mainWindow.Top = ScreenHelper.GetPrimaryScreen().Bounds.Height * 0.40;

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
