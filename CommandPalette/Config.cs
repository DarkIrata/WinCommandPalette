using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using CommandPalette.Commands;

namespace CommandPalette
{
    public class Config : ICloneable
    {
        public event EventHandler<EventArgs> ConfigUpdated;

        public ModifierKey ModifierKey { get; set; }

        public uint KeyCode { get; set; }

        [XmlIgnore]
        public List<ICommand> Commands { get; set; }

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
            configXML.Root.Add(XElement.Parse($"<Commands>{this.SerializeCommands(ns, settings)}</Commands>"));

            File.WriteAllText(path, configXML.ToString());
        }

        private string SerializeCommands(XmlSerializerNamespaces ns, XmlWriterSettings settings)
        {
            var commandsSB = new StringBuilder(4096);

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
                commandElement.Add(new XAttribute("Type", command.GetType()));
                commandsSB.AppendLine(commandElement.ToString());
            }


            return commandsSB.ToString();
        }

        internal void UpdateConfig(Config newConfig)
        {
            this.KeyCode = newConfig.KeyCode;
            this.ModifierKey = newConfig.ModifierKey;
            this.Commands = newConfig.Commands;

            this.ConfigUpdated?.Invoke(this, new EventArgs());
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
            config.Commands = DeserializeCommands(configXML.Root.Element("Commands"));
            return config;
        }

        private static List<ICommand> DeserializeCommands(XElement commandsElement)
        {
            var commands = new List<ICommand>();
            if (commandsElement == null)
            {
                return commands;
            }

            Assembly assembly = typeof(ICommand).Assembly;
            foreach (var commandElement in commandsElement.Elements())
            {
                Type type = assembly.GetType(commandElement.Attribute("Type").Value);
                var cmdSerializer = new XmlSerializer(type);
                using (var fs = new StringReader(commandElement.ToString()))
                {
                    commands.Add((ICommand)cmdSerializer.Deserialize(fs));
                }

            }

            return commands;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
