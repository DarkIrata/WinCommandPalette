using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace WinCommandPalette.Libs.Controls
{
    [ContentProperty("Text")]
    public partial class InfoToolTip : UserControl
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(InfoToolTip), new PropertyMetadata("Missing Infotext"));
        public string Text
        {
            get => (string)this.GetValue(TextProperty);
            set => this.SetValue(TextProperty, value);
        }

        public InfoToolTip()
        {
            this.InitializeComponent();
        }
    }
}
