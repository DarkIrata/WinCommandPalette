using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WinCommandPalette.Helper;

namespace WinCommandPalette.Controls
{
    /// <summary>
    /// Interaktionslogik für SubMenuItem.xaml
    /// </summary>
    public partial class SubMenuItem : UserControl
    {
        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(
            "Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SubMenuItem));

        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(SubMenuItem), new PropertyMetadata(string.Empty));


        public bool IsActive
        {
            get => (bool)GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }

        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(SubMenuItem), new PropertyMetadata(false));

        public UserControl Page { get; internal set; }

        public MenuItem MenuItem{ get; internal set; }

        public SubMenuItem(string text, UserControl page)
            : this()
        {
            this.Text = text;
            this.Page = page;
        }
        
        private SubMenuItem()
        {
            this.InitializeComponent();
            this.SubMenuText.MouseUp += (s, e) => RaiseEvent(new RoutedEventArgs(ClickEvent));
        }
    }
}
