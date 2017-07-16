using WinCommandPalette.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using WinCommandPalette.Helper;
using WinCommandPalette.PluginSystem;

namespace WinCommandPalette
{
    public class Config : ICloneable
    {
        private const string COMMANDS_TAG_NAME = "Commands";
        private const string PLUGIN_TYPE_ATTRIBUTE_NAME = "Type";
        private const string PLUGIN_ASSEMBLY_ATTRIBUTE_NAME = "Plugin";

        public ModifierKey ModifierKey { get; set; }

        public uint KeyCode { get; set; }

        [XmlIgnore]
        public List<ICommand> Commands { get; set; }

        private List<XElement> UndeserializableCommands = new List<XElement>();

        public Config()
        {
            this.ModifierKey = ModifierKey.LeftCTRL | ModifierKey.LeftShift;
            this.KeyCode = 80; // P
            this.Commands = new List<ICommand>();
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

            var baseAssembly = typeof(ICommand).Assembly;
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

        internal void UpdateConfig(Config newConfig)
        {
            this.KeyCode = newConfig.KeyCode;
            this.ModifierKey = newConfig.ModifierKey;
            this.Commands = newConfig.Commands;
            this.UndeserializableCommands = newConfig.UndeserializableCommands;
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
            config.Commands = new List<ICommand>();
            if (commandsElement == null)
            {
                return;
            }

            var baseAssembly = typeof(ICommand).Assembly;
            foreach (var commandElement in commandsElement.Elements())
            {
                var assembly = baseAssembly;
                var assemblyName = commandElement.Attribute(PLUGIN_ASSEMBLY_ATTRIBUTE_NAME)?.Value;
                if (!string.IsNullOrEmpty(assemblyName))
                {
                    if (!PluginHelper.PluginAssemblies.ContainsKey(assemblyName))
                    {
                        Console.WriteLine($"Plugin '{assemblyName}' is unkown");
                        config.UndeserializableCommands.Add(commandElement);
                        continue;
                    }

                    assembly = PluginHelper.PluginAssemblies[assemblyName];
                }

                var type = assembly.GetType(commandElement.Attribute(PLUGIN_TYPE_ATTRIBUTE_NAME).Value);
                var cmdSerializer = new XmlSerializer(type);
                using (var fs = new StringReader(commandElement.ToString()))
                {
                    config.Commands.Add((ICommand)cmdSerializer.Deserialize(fs));
                }
            }
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
