using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using WinCommandPalette.Helper;
using WinCommandPalette.PluginSystem;
using WinCommandPalette.Plugin.CommandBase;
using WinCommandPalette.Enums;
using System.Linq;
using System.Reflection;
using WinCommandPalette.Libs.Helper;

namespace WinCommandPalette
{
    public class Config : IEquatable<Config>
    {
        private const string COMMANDS_TAG_NAME = "Commands";
        private const string PLUGIN_TYPE_ATTRIBUTE_NAME = "Type";
        private const string PLUGIN_ASSEMBLY_ATTRIBUTE_NAME = "Plugin";
        
        public ModifierKey ModifierKey { get; set; }

        public uint KeyCode { get; set; }

        public bool RunWithWindows { get; set; }

        public bool BlurryWindow { get; set; }

        [XmlIgnore]
        public List<ICommandBase> Commands { get; set; }

        internal List<XElement> UndeserializableCommands = new List<XElement>();

        public Config()
        {
            this.ModifierKey = ModifierKey.LeftCTRL | ModifierKey.LeftShift;
            this.KeyCode = 80; // P
            this.Commands = new List<ICommandBase>();
        }

        internal void Save(string path)
        {
            var ns = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var settings = new XmlWriterSettings()
            {
                Indent = true,
                OmitXmlDeclaration = true
            };

            var configSB = new StringBuilder(64);
            var serializer = new XmlSerializer(typeof(Config));
            using (var stream = new StringWriter(configSB))
            using (var w = XmlWriter.Create(stream, settings))
            {
                serializer.Serialize(w, this, ns);
            }

            var configXML = XDocument.Parse(configSB.ToString());
            configXML.Root.Add(XElement.Parse($"<{COMMANDS_TAG_NAME}>{this.SerializeCommands(ns, settings)}</{COMMANDS_TAG_NAME}>"));
            if (this.UndeserializableCommands.Count > 0)
            {
                foreach (var command in this.UndeserializableCommands)
                {
                    configXML.Root.Element(COMMANDS_TAG_NAME).Add(command);
                }
            }

            File.WriteAllText(path, configXML.ToString());
        }

        private string SerializeCommands(XmlSerializerNamespaces ns, XmlWriterSettings settings)
        {
            var commandsSB = new StringBuilder(4096);

            var baseAssembly = typeof(Config).Assembly;
            foreach (var command in this.Commands)
            {
                var sb = new StringBuilder(128);
                var cmdSerializer = new XmlSerializer(command.GetType());
                using (var stream = new StringWriter(sb))
                using (var w = XmlWriter.Create(stream, settings))
                {
                    cmdSerializer.Serialize(w, command, ns);
                }

                var commandElement = XElement.Parse(sb.ToString());
                commandElement.Add(new XAttribute(PLUGIN_TYPE_ATTRIBUTE_NAME, command.GetType()));

                var assembly = command.GetType().Assembly;
                if (assembly != baseAssembly)
                {
                    commandElement.Add(new XAttribute(PLUGIN_ASSEMBLY_ATTRIBUTE_NAME, assembly.GetName().Name));
                }

                commandsSB.AppendLine(commandElement.ToString());
            }

            return commandsSB.ToString();
        }

        public static Config Load(string path)
        {
            var config = new Config();
            var serializer = new XmlSerializer(typeof(Config));

            var configFile = File.ReadAllText(path);
            using (var fs = new StringReader(configFile))
            {
                config = (Config)serializer.Deserialize(fs);
            }

            var configXML = XDocument.Parse(configFile);
            DeserializeCommands(config, configXML.Root.Element(COMMANDS_TAG_NAME));
            return config;
        }

        private static void DeserializeCommands(Config config, XElement commandsElement)
        {
            config.Commands = new List<ICommandBase>();

            if (commandsElement == null)
            {
                return;
            }

            var baseAssembly = typeof(Config).Assembly;
            var baseAssemblyName = baseAssembly.GetName().Name;
            foreach (var commandElement in commandsElement.Elements())
            {
                var assembly = baseAssembly;
                var pluginName = commandElement.Attribute(PLUGIN_ASSEMBLY_ATTRIBUTE_NAME)?.Value;

                if (!string.IsNullOrEmpty(pluginName) &&
                    pluginName != baseAssemblyName)
                {
                    var plugin = PluginHelper.GetPlugin(pluginName);
                    if (plugin == null)
                    {
                        Console.WriteLine($"Plugin '{pluginName}' is unkown");
                        config.UndeserializableCommands.Add(commandElement);
                        continue;
                    }

                    assembly = ((PluginSystem.Plugin)plugin).Assembly;
                }

                var type = assembly.GetType(commandElement.Attribute(PLUGIN_TYPE_ATTRIBUTE_NAME)?.Value);
                if (type == null)
                {
                    Console.WriteLine($"Plugin '{pluginName}' doesn't contain command type '{type}'");
                    config.UndeserializableCommands.Add(commandElement);
                    continue;
                }

                var cmdSerializer = new XmlSerializer(type);
                using (var fs = new StringReader(commandElement.ToString()))
                {
                    var command = (ICommandBase)cmdSerializer.Deserialize(fs);
                    if (string.IsNullOrEmpty(command.Name))
                    {
                        Console.WriteLine($"A Command from Plugin '{pluginName}' was found without a name");
                        continue;
                    }
                    config.Commands.Add(command);
                }
            }
        }

        public void Update(Config other)
        {
            var configProperties = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in configProperties)
            {
                var newValue = other.GetType().GetProperty(property.Name)?.GetValue(other);
                if (newValue != null)
                {
                    property.SetValue(this, newValue);
                }
            }
        }

        public Config DeepCopy()
        {
            return this.DeepCopyByExpressionTree();
        }

        public bool Equals(Config other)
        {
            var baseProperties = true;
            var configProperties = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in configProperties)
            {
                if (property.Name == nameof(this.Commands))
                {
                    continue;
                }

                var value = property.GetValue(this);
                var newValue = other.GetType().GetProperty(property.Name)?.GetValue(other);

                if (newValue?.GetHashCode() != value?.GetHashCode())
                {
                    baseProperties = false;
                    break;
                }
            }

            var commands = false;
            if (this.Commands.Count == 0 && other.Commands.Count == 0)
            {
                commands = true;
            }
            else
            {
                commands = this.Commands.Count == other.Commands.Count &&
                           this.Commands.Except(other.Commands).Any();
            }

            return baseProperties && commands;
        }
    }
}
