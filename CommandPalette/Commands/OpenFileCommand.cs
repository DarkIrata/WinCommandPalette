using System.Diagnostics;
using CommandPalette.PluginSystem;

namespace CommandPalette.Commands
{
    public class OpenFileCommand : ICommand
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string FileName { get; set; }

        public string Arguments { get; set; }

        public string WorkingDirectory { get; set; }

        public bool RunAsAdmin { get; set; }

        public bool RunInUIThread { get; set; }

        public OpenFileCommand()
        {
            this.FileName = string.Empty;
            this.Arguments = string.Empty;
            this.WorkingDirectory = string.Empty;
            this.RunAsAdmin = false;
            this.RunInUIThread = true;
        }

        public void Execute()
        {
            var processStartInfo = new ProcessStartInfo(this.FileName, this.Arguments);
            if (this.RunAsAdmin)
            {
                processStartInfo.Verb = "runas";
            }

            if (!string.IsNullOrEmpty(this.WorkingDirectory))
            {
                processStartInfo.WorkingDirectory = this.WorkingDirectory;
            }

            var process = new Process()
            {
                StartInfo = processStartInfo
            };

            process.Start();
        }
    }
}
