using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using CommandPalette.PluginSystem;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace CSharpScriptCommandsPlugin
{
    public class CSharpScriptCommand : ICommand
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public bool RunInUIThread => false;

        public bool RunAsAdmin { get; set; }

        public string Code { get; set; }

        public void Execute()
        {
            if (string.IsNullOrEmpty(this.Code))
            {
                MessageBox.Show(nameof(this.Code) + " can't be empty");
                return;
            }

            try
            {
                var result = CSharpScript.EvaluateAsync<string>(this.Code)?.Result;
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
                File.WriteAllText(logPath, this.FormatCompileError(compilerError.Message));

                MessageBox.Show($"Error while compiling the code.\r\nLOG: {logPath}", "WinCommand Palette", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string FormatCompileError(string compileError)
        {
            var sb = new StringBuilder(2048);
            sb.AppendLine("==============[ Info ]==============");
            sb.AppendLine($"Command: '{this.Name ?? ">>InstantCommand-Call<<"}'");
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
