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
            "Click", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(MenuItem));

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

        public UserControl Page { get; set; }

        public List<MenuItem> SubMenuItems { get; set; }

        public MenuItem()
        {
            this.InitializeComponent();
            this.SubMenuItems = new List<MenuItem>();
            this.MouseUp += (s, e) => RaiseEvent(new RoutedEventArgs(ClickEvent));
        }

        public MenuItem(string text)
            : this(text, null, null)
        {
        }

        public MenuItem(string text, UserControl page, params MenuItem[] menuItems)
            : this()
        {
            this.Text = text;
            this.Page = page;

            foreach (var menuItem in menuItems)
            {
                menuItem.Click += (s, e) => RaiseEvent(new RoutedEventArgs(ClickEvent));
                this.SubMenuItems.Add(menuItem);
            }
        }
    }
}
