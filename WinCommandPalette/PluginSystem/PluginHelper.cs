using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using WinCommandPalette.Plugin;
using WinCommandPalette.Plugin.CommandBase;
using WinCommandPalette.Plugin.CreateCommand;

namespace WinCommandPalette.PluginSystem
{
    internal static class PluginHelper
    {
        internal readonly static string PluginDirectoryPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Plugins");
        internal static string PluginFileType = ".dll";
        private static Dictionary<string, Plugin> Plugins = new Dictionary<string, Plugin>();

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

        internal static int Load()
        {
            // TODO: Add Logging
            // TODO: Add Stopwatch to lookup plugin load time
            Directory.CreateDirectory(PluginDirectoryPath);

            var pluginFiles = GetPluginFilePaths();
            foreach (var pluginFile in pluginFiles)
            {
                var pluginName = Path.GetFileNameWithoutExtension(pluginFile);

                if (Plugins.ContainsKey(pluginName))
                {
                    ShowErrorLoadingPluginMessage(pluginName, "A plugin with this name is already registered.");
                    continue;
                }

                try
                {
                    var pluginAssembly = Assembly.LoadFile(pluginFile);
                    var wcpPlugin = GetPluginInstance(pluginAssembly);
                    if (wcpPlugin == null)
                    {
                        ShowErrorLoadingPluginMessage(pluginName, "Plugin is missing a WCPPlugin instance.");
                        continue;
                    }
                    Task.Run(() => CallPluginOnLoad(wcpPlugin));

                    var commands = GetPluginCommands(pluginAssembly);
                    if (commands == null ||
                        commands.Count == 0)
                    {
                        ShowErrorLoadingPluginMessage(pluginName, "Plugin doesn't have any commands to load");
                        continue;
                    }

                    Plugins.Add(pluginName, new Plugin(pluginAssembly, wcpPlugin, commands));
                }
                catch (Exception ex)
                {
                    ShowErrorLoadingPluginMessage(pluginName, ex.Message);
                }
            }

            return Plugins.Count;
        }

        private static void CallPluginOnLoad(WCPPlugin wcpPlugin)
        {
            try
            {
                wcpPlugin.OnLoad();
            }
            catch
            {
                // TODO: Should Log
            }
        }

        internal static List<ICommandBase> GetAllAutoRegisterCommands()
        {
            var autoRegisterCommands = new List<ICommandBase>();

            foreach (var plugin in Plugins)
            {
                if (plugin.Value.WCPPlugin.AutoRegisterCommands != null)
                {
                    autoRegisterCommands.AddRange(plugin.Value.WCPPlugin.AutoRegisterCommands);
                }
            }

            return autoRegisterCommands;
        }

        internal static List<Plugin> GetPlugins()
        {
            return Plugins.Values.ToList();
        }

        internal static Plugin? GetPlugin(string pluginName)
        {
            if (Plugins.ContainsKey(pluginName))
            {
                return Plugins[pluginName];
            }

            return null;
        }

        private static void ShowErrorLoadingPluginMessage(string pluginName, string reason)
        {
            Task.Run(() => // Message Box is Threaded so it doesn't destroy the handler for the main Window. Needed to register the HotKey
            {
                MessageBox.Show($"Error loading plugin '{pluginName}'.\r\n\r\nReason: {reason}", "WinCommand Palette PluginLoader", MessageBoxButton.OK, MessageBoxImage.Error);
            });
        }

        private static List<string> GetPluginFilePaths()
        {
            var pluginFiles = new List<string>();
            var pluginDirectories = Directory.GetDirectories(PluginDirectoryPath);

            foreach (var pluginDirectory in pluginDirectories)
            {
                var pluginFile = Path.Combine(pluginDirectory, Path.GetFileName(pluginDirectory) + PluginFileType);
                if (File.Exists(pluginFile))
                {
                    pluginFiles.Add(pluginFile);
                }
            }

            return pluginFiles;
        }

        private static WCPPlugin GetPluginInstance(Assembly assembly)
        {
            var baseType = typeof(WCPPlugin);
            var type = assembly.GetTypes()?.FirstOrDefault(t => baseType.IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface);
            if (type == null)
            {
                return null;
            }

            return (WCPPlugin)Activator.CreateInstance(type);
        }

        private static Dictionary<string, ICreateCommand> GetPluginCommands(Assembly assembly)
        {

            var wpcCommands = new Dictionary<string, ICreateCommand>();
            var commandBaseType = typeof(ICommandBase);

            var commandTypes = assembly.GetTypes()?.Where(p => commandBaseType.IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract);
            if (commandTypes == null)
            {
                return wpcCommands;
            }

            foreach (var commandType in commandTypes)
            {
                var createCommandInstance = GetCreateCommandInstance(assembly, commandType.Name);
                wpcCommands.Add(commandType.FullName, createCommandInstance);
            }

            return wpcCommands;
        }

        private static ICreateCommand GetCreateCommandInstance(Assembly assembly, string commandName)
        {
            var createCommandType = typeof(ICreateCommand);
            var assemblyCreateCommandType = assembly.GetTypes()?.FirstOrDefault(p => createCommandType.IsAssignableFrom(p) &&
                                                                !p.IsInterface && !p.IsAbstract &&
                                                                p.Name == "Create" + commandName);
            if (assemblyCreateCommandType == null)
            {
                return null;
            }

            return (ICreateCommand)Activator.CreateInstance(assemblyCreateCommandType);
        }

        private static List<T> GetFromAssembly<T>(Assembly assembly, string namePart = "")
        {
            var list = new List<T>();
            var baseType = typeof(T);
            var types = assembly.GetTypes()
                      ?.Where(p => baseType.IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract);

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
