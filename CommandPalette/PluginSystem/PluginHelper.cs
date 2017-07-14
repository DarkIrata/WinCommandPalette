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

        static PluginHelper()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (o, args) =>
            {
                if (args.RequestingAssembly != null)
                {
                    var path = Path.Combine(Path.GetDirectoryName(args.RequestingAssembly.Location), args.Name.Split(',')[0] + ".dll");
                    if (File.Exists(path))
                    {
                        return Assembly.LoadFile(path);
                    }
                }

                return null;
            };
        }

        public static int Load()
        {
            if (!Directory.Exists(PluginDirectoryPath))
            {
                Directory.CreateDirectory(PluginDirectoryPath);
            }

            var pluginFolders = Directory.GetDirectories(PluginDirectoryPath);
            foreach (var pluginFolder in pluginFolders)
            {
                var pluginFile = Path.Combine(pluginFolder, Path.GetFileName(pluginFolder) + PluginFileType);
                if (File.Exists(pluginFile))
                {
                    try
                    {
                        var pluginAssembly = Assembly.LoadFile(pluginFile);
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
