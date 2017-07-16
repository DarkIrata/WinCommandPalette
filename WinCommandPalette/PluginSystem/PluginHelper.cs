using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace WinCommandPalette.PluginSystem
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

        public static List<T> GetAll<T>()
        {
            var list = new List<T>();
            foreach (var pluginAssembly in PluginAssemblies)
            {
                var resultFromAssembly = GetFromAssembly<T>(pluginAssembly.Value);
                if (resultFromAssembly != null && resultFromAssembly.Count > 0)
                {
                    list.AddRange(resultFromAssembly);
                }
            }

            return list;
        }

        public static List<T> GetFromAssembly<T>(Assembly assembly)
        {
            var list = new List<T>();
            var baseType = typeof(T);
            var types = assembly.GetTypes()
                      ?.Where(p => baseType.IsAssignableFrom(p) && !p.IsInterface);

            if (types == null)
            {
                return null;
            }

            foreach (var type in types)
            {
                list.Add((T)Activator.CreateInstance(type));
            }

            return list;
        }
    }
}
