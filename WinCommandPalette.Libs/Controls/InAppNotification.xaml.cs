using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace WinCommandPalette.Libs.Controls
{
    [ContentProperty("Text")]
    public partial class InAppNotification : UserControl
    {
        public enum NotificationType
        {
            None,
            Info,
            Success,
            Error,
            Warning
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(InAppNotification), new PropertyMetadata("Missing Infotext"));

        public string Text
        {
            get => (string)this.GetValue(TextProperty);
            set => this.SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty FullyRoundedProperty =
            DependencyProperty.Register("FullyRounded", typeof(bool), typeof(InAppNotification), new PropertyMetadata(false));

        public bool FullyRounded
        {
            get => (bool)this.GetValue(FullyRoundedProperty);
            set => this.SetValue(FullyRoundedProperty, value);
        }

        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register("Type", typeof(NotificationType), typeof(InAppNotification), new PropertyMetadata(NotificationType.None, new PropertyChangedCallback(OnTypeChanged)));

        private static void OnTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is InAppNotification notification)
            {
                notification.SetBaseControlColor();
            }
        }

        public NotificationType Type
        {
            get => (NotificationType)this.GetValue(TypeProperty);
            set
            {
                this.SetValue(TypeProperty, value);
            }
        }

        public InAppNotification()
        {
            this.InitializeComponent();
        }

        private void SetBaseControlColor()
        {
            switch (this.Type)
            {
                default:
                case NotificationType.Info:
                    this.BaseControl.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xFC, 0xFC, 0xFC));
                    this.BaseControl.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xAA, 0xB8, 0xC6));
                    break;
                case NotificationType.Success:
                    this.BaseControl.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xF3, 0xF9, 0xF4));
                    this.BaseControl.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x91, 0xC8, 0x9C));
                    break;
                case NotificationType.Error:
                    this.BaseControl.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xF8, 0xF7));
                    this.BaseControl.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xD0, 0x44, 0x37));
                    break;
                case NotificationType.Warning:
                    this.BaseControl.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFD, 0xF6));
                    this.BaseControl.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xEA, 0xAE));
                    break;
            }
        }
    }
}
