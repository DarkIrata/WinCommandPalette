using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;

namespace CommandPalette.PluginSystem
{
    public static class PluginHelper
    {
        public readonly static string PluginDirectoryPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Plugins");

        public static string PluginFileType = ".dll";

        public static Dictionary<string, Assembly> PluginAssemblies = new Dictionary<string, Assembly>();

        public static int Load()
        {
            if (!Directory.Exists(PluginDirectoryPath))
            {
                Directory.CreateDirectory(PluginDirectoryPath);
            }

            var pluginFolders = Directory.GetDirectories(PluginDirectoryPath);
            foreach (var pluginFolder in pluginFolders)
            {
                var pluginFiles = Directory.GetFiles(Path.Combine(PluginDirectoryPath, pluginFolder), $"*{PluginFileType}", SearchOption.TopDirectoryOnly);
                foreach (var pluginFile in pluginFiles)
                {
                    try
                    {
                        var pluginAssembly = Assembly.LoadFrom(pluginFile);
                        PluginAssemblies.Add(pluginAssembly.GetName().Name, pluginAssembly);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show($"Error loading Plugin '{Path.GetFileName(pluginFile)}'.");
                    }
                }
            }

            return PluginAssemblies.Count;
        }
    }
}
