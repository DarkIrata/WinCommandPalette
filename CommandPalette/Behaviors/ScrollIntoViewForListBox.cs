using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace CommandPalette.Behaviors
{
    // Code by Ankesh
    // https://stackoverflow.com/a/8830961/6300299

    public class ScrollIntoViewForListBox : Behavior<ListBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.SelectionChanged += this.AssociatedObject_SelectionChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.SelectionChanged -= this.AssociatedObject_SelectionChanged;

        }

        private void AssociatedObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
