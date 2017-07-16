using System;
using System.Windows;
using System.Windows.Controls;

namespace WinCommandPalette.AttachedProperties
{
    public static class ScrollIntoView
    {
        public static readonly DependencyProperty EnabledProperty = DependencyProperty.RegisterAttached(
            "Enabled",
            typeof(bool),
            typeof(ScrollIntoView),
            new PropertyMetadata(InternalEnabledChanged));

        private static void InternalEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListBox lBox)
            {
                if ((bool)e.NewValue)
                {
                    lBox.SelectionChanged += AssociatedObject_SelectionChanged;
                }
                else
                {
                    lBox.SelectionChanged -= AssociatedObject_SelectionChanged;
                }
            }
            else
            {
                throw new NotSupportedException($"{d.GetType().Name} is not supported");
            }
        }

        public static void SetEnabled(DependencyObject d, bool value)
        {
            d.SetValue(EnabledProperty, value);
        }

        public static bool GetEnabled(DependencyObject d)
        {
            return (bool)d.GetValue(EnabledProperty);
        }

        private static void AssociatedObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox)
            {
                if (listBox.SelectedItem != null)
                {
                    listBox.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        listBox.UpdateLayout();
                        if (listBox.SelectedItem != null)
                        {
                            listBox.ScrollIntoView(listBox.SelectedItem);
                        }
                    }));
                }
            }
        }
    }
}
