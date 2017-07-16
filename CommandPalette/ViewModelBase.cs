using System.ComponentModel;

namespace WinCommandPalette
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string property) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }
}
