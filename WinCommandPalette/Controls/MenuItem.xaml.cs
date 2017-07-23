using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace WinCommandPalette.Controls
{
    /// <summary>
    /// Interaktionslogik für MenuItem.xaml
    /// </summary>
    public partial class MenuItem : UserControl
    {
        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(
            "Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MenuItem));

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
            DependencyProperty.Register("Text", typeof(string), typeof(MenuItem), new PropertyMetadata(string.Empty));

        public bool IsActive
        {
            get => (bool)GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }

        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(MenuItem), new PropertyMetadata(false));

        public bool HasSubMenuItems => this.SubMenuItems?.Count > 0;

        private List<SubMenuItem> subMenuItems = new List<SubMenuItem>();
        public List<SubMenuItem> SubMenuItems
        {
            get => this.subMenuItems;
            set => this.subMenuItems = value;
        }
        
        public UserControl Page { get; set; }

        private MenuItem()
        {
            this.InitializeComponent();
            this.MenuText.MouseUp += (s, e) => RaiseEvent(new RoutedEventArgs(ClickEvent));
        }

        public MenuItem(string text)
            : this(text, null, null)
        {
        }

        public MenuItem(string text, UserControl page, params SubMenuItem[] subMenuItems)
            : this()
        {
            this.Text = text;
            this.Page = page;

            foreach (var subMenuItem in subMenuItems)
            {
                subMenuItem.MenuItem = this;
                this.SubMenuItems.Add(subMenuItem);
            }
        }
    }
}
