using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using WinCommandPalette.Plugin.CommandBase;
using WinCommandPalette.Plugin.CreateCommand;

namespace CSharpScriptCommandsPlugin
{
    /// <summary>
    /// Interaction logic for CreateCSharpScriptCommand.xaml
    /// </summary>
    public partial class CreateCSharpScriptCommand : UserControl, ICreateCommand
    {
        private const string DefaultCode = @"return $""The current time is: {System.DateTime.Now}"";";

        private readonly ITextMarkerService textMarkerService;
        private readonly ObservableCollection<string> references = new ObservableCollection<string>();

        private ToolTip editorToolTip;

        public CreateCSharpScriptCommand()
        {
            this.InitializeComponent();

            var textMarkerService = new TextMarkerService(this.tbCode.Document);
            this.tbCode.TextArea.TextView.BackgroundRenderers.Add(textMarkerService);
            this.tbCode.TextArea.TextView.LineTransformers.Add(textMarkerService);
            var services = (IServiceContainer)this.tbCode.Document.ServiceProvider.GetService(typeof(IServiceContainer));
            if (services != null)
            {
                services.AddService(typeof(ITextMarkerService), textMarkerService);
            }

            this.textMarkerService = textMarkerService;

            this.tbCode.Document.TextChanged += this.Document_TextChanged;
            this.tbCode.TextArea.TextView.MouseHover += this.TextView_MouseHover;
            this.tbCode.TextArea.TextView.MouseHoverStopped += this.TextView_MouseHoverStopped;
            this.lbReferences.ItemsSource = this.references;

            this.ClearAll();
        }

        private void Document_TextChanged(object sender, System.EventArgs e)
        {
            if (this.editorToolTip != null)
            {
                this.editorToolTip.IsOpen = false;
            }

            var script = CSharpScript.Create<string>(this.tbCode.Text, ScriptOptions.Default.WithReferences(this.references));
            var diagnostics = script.Compile();

            this.textMarkerService.RemoveAll(m => true);

            foreach (var diag in diagnostics)
            {
                var marker = this.textMarkerService.Create(diag.Location.SourceSpan.Start, diag.Location.SourceSpan.Length);
                marker.MarkerTypes = TextMarkerTypes.SquigglyUnderline;
                marker.MarkerColor = Colors.Red;

                marker.ToolTip = diag.GetMessage();
            }
        }

        private void TextView_MouseHover(object sender, MouseEventArgs e)
        {
            var pos = this.tbCode.TextArea.TextView.GetPositionFloor(e.GetPosition(this.tbCode.TextArea.TextView) + this.tbCode.TextArea.TextView.ScrollOffset);
            var inDocument = pos.HasValue;
            if (inDocument)
            {
                var logicalPosition = pos.Value.Location;
                var offset = this.tbCode.Document.GetOffset(logicalPosition);

                var markersAtOffset = this.textMarkerService.GetMarkersAtOffset(offset);
                var markerWithToolTip = markersAtOffset.FirstOrDefault(marker => marker.ToolTip != null);

                if (markerWithToolTip != null)
                {
                    if (this.editorToolTip == null)
                    {
                        this.editorToolTip = new ToolTip()
                        {
                            PlacementTarget = this,
                            Content = new TextBlock
                            {
                                Text = (string)markerWithToolTip.ToolTip,
                                TextWrapping = TextWrapping.Wrap
                            },
                            IsOpen = true
                        };
                        this.editorToolTip.Closed += this.ToolTip_Closed;

                        e.Handled = true;
                    }
                }
            }
        }

        private void TextView_MouseHoverStopped(object sender, MouseEventArgs e)
        {
            if (this.editorToolTip != null)
            {
                this.editorToolTip.IsOpen = false;
                e.Handled = true;
            }
        }

        private void ToolTip_Closed(object sender, RoutedEventArgs e)
        {
            this.editorToolTip.Closed -= this.ToolTip_Closed;
            this.editorToolTip = null;
        }

        public string CommandTypeName => "C#-Script";

        public string CommandDescription => "Execute a C# - Script";

        public void ClearAll()
        {
            this.tbName.Text = "New C#-Script command";
            this.tbDescription.Text = "This is a new C#-Script command with the default description";
            this.tbCode.Text = DefaultCode;
            this.references.Clear();
            this.references.Add("System");
        }

        public ICommandBase GetCommand()
        {
            return new CSharpScriptCommand()
            {
                Name = this.tbName.Text,
                Description = this.tbDescription.Text,
                Code = this.tbCode.Text,
                References = this.references.ToList()
            };
        }

        public void ShowCommand(ICommandBase command)
        {
            if (command is CSharpScriptCommand csscommand)
            {
                this.references.Clear();
                foreach (var item in csscommand.References)
                {
                    this.references.Add(item);
                }
                this.SortReferences();

                this.tbName.Text = csscommand.Name;
                this.tbDescription.Text = csscommand.Description;
                this.tbCode.Text = csscommand.Code;
            }
        }

        private void BtnAddReference_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.tbReference.Text) && !this.references.Contains(this.tbReference.Text))
            {
                this.references.Add(this.tbReference.Text);

                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)this.SortReferences);

                this.tbReference.Clear();
            }
        }

        private void SortReferences()
        {
            var unsorted = this.references.ToList();

            this.references.Clear();
            unsorted.Sort();

            foreach (var reference in unsorted)
            {
                this.references.Add(reference);
            }
        }

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe)
            {
                if (fe.DataContext is string reference)
                {
                    this.references.Remove(reference);
                }
            }
        }
    }
}
