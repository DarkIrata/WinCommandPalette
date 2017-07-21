using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Xml.Serialization;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using WinCommandPalette.Plugin.CommandBase;

namespace CSharpScriptCommandsPlugin
{
    public class CSharpScriptCommand : ICommandBase
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public bool RunInUIThread => false;

        public string Code { get; set; }

        public Image Icon => null;
        
        [XmlArrayItem("Reference")]
        public List<string> References { get; set; } = new List<string>()
        {
            "System"
        };

        public void Execute()
        {
            if (string.IsNullOrEmpty(this.Code))
            {
                MessageBox.Show(nameof(this.Code) + " can't be empty");
                return;
            }

            try
            {
                var result = CSharpScript.EvaluateAsync<string>(this.Code, ScriptOptions.Default.WithReferences(this.References))?.Result;
                if (!string.IsNullOrEmpty(result))
                {
                    MessageBox.Show(result);
                }
            }
            catch (CompilationErrorException compilerError)
            {
                var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Log");
                var logPath = Path.Combine(path, $"CompileError_{DateTime.Now.ToString("dd-MM-yy__HH_mm")}.log");

                Directory.CreateDirectory(path);
                var message = this.FormatCompileError(compilerError.Message);
                File.WriteAllText(logPath, message);

                MessageBox.Show($"Error while compiling the code.\r\nLOG: {logPath}{Environment.NewLine}{Environment.NewLine}{message}", "WinCommand Palette", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string FormatCompileError(string compileError)
        {
            var sb = new StringBuilder(2048);
            sb.AppendLine("==============[ Info ]==============");
            sb.AppendLine($"Command: '{this.Name ?? ">>AutoRegister Command<<"}'");
            sb.AppendLine(string.Empty);
            sb.AppendLine("==============[ C# Code ]==============");
            sb.AppendLine(this.Code);
            sb.AppendLine(string.Empty);
            sb.AppendLine("==============[ Compiler Error ]==============");
            sb.AppendLine(compileError);

            return sb.ToString();
        }
    }
}
