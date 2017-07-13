using CommandPalette.PluginSystem;
using System.Windows.Controls;

namespace CSharpScriptCommandsPlugin
{
    /// <summary>
    /// Interaction logic for CreateCShartScriptCommand.xaml
    /// </summary>
    public partial class CreateCShartScriptCommand : UserControl, ICreateCommand
    {
        private const string DefaultCode = @"using System.Windows.Forms;

MessageBox.Show(""Hello World!"");";

        public CreateCShartScriptCommand()
        {
            this.InitializeComponent();

            this.ClearAll();
        }

        public string CommandTypeName => "C#-Script";

        public void ClearAll()
        {
            this.tbName.Text = "New C#-Script command";
            this.tbDescription.Text = "This is a new C#-Script command with the default description";
            this.tbCode.Text = DefaultCode;
        }

        public ICommand GetCommand()
        {
            return new CSharpScriptCommand()
            {
                Name = this.tbName.Text,
                Description = this.tbDescription.Text,
                Code = this.tbCode.Text
            };
        }
    }
}
