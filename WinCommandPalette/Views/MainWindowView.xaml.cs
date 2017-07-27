using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using WinCommandPalette.Helper;
using WinCommandPalette.ViewModels;

namespace WinCommandPalette.Views
{
    public partial class MainWindow : Window
    {
        private MainWindowViewModel viewModel;
        private Config config;
        private bool focusedListBoxItem = false;
        private bool calledClosing = false;

        private MainWindow()
        {
            this.InitializeComponent();

            this.Loaded += this.HideWindow;
            this.GotKeyboardFocus += this.MainWindow_GotKeyboardFocus;
            this.Deactivated += this.HideWindow;

            this.SearchBox.PreviewKeyDown += this.SearchBox_PreviewKeyDown;
            this.SearchBox.LostKeyboardFocus += this.HideWindow;
        }

        public MainWindow(Config config)
            : this()
        {
            this.config = config ??
                throw new ArgumentNullException(nameof(config));

            this.viewModel = new MainWindowViewModel(this.config);
            this.DataContext = this.viewModel;
            this.SearchBox.TextChanged += this.viewModel.SearchBox_TextChanged;
        }

        private void SearchBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.DisableContent();
            }
            else if (e.Key == Key.Up)
            {
                if (this.viewModel.SelectedIndex > 0)
                {
                    this.viewModel.SelectedIndex--;
                }
            }
            else if (e.Key == Key.Down)
            {
                if (this.viewModel.SelectedIndex < this.viewModel.FilteredCommandList.Count - 1)
                {
                    this.viewModel.SelectedIndex++;
                }
            }
            else if (e.Key == Key.Enter)
            {
                this.ExecuteSelectedCommand();
            }
        }

        private void ListBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(sender is ListBoxItem listBoxItem)
            {
                this.focusedListBoxItem = true;
                listBoxItem.IsSelected = true;
            }
        }

        private void ListBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListBoxItem listBoxItem)
            {
                if (listBoxItem.IsSelected)
                {
                    this.ExecuteSelectedCommand();
                }
            }
        }

        private void HideWindow(object sender, EventArgs e)
        {
            if (sender == this.SearchBox && this.focusedListBoxItem)
            {
                return;
            }

            this.DisableContent();
        }

        private void MainWindow_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            this.SearchBox.Focus();
            Keyboard.Focus(this.SearchBox);
        }

        private void ActivateContent()
        {
            Win32Helper.UnregisterHotKey();
            this.viewModel.ShowAllCommands();
            if (this.viewModel.FilteredCommandList.Count > 0)
            {
                this.SuggestionList.ScrollIntoView(this.viewModel.FilteredCommandList[0]);
            }

            this.Top = ScreenHelper.GetPrimaryScreen().Bounds.Height * 0.20;
            this.Left = ScreenHelper.GetAppCenterScreenWidth(this.Width);
            this.Visibility = Visibility.Visible;
            this.SizeToContent = SizeToContent.Height;

            Win32Helper.SetForegroundWindow(Win32Helper.applicationHandle);
        }

        private void DisableContent()
        {
            if (this.calledClosing)
            {
                return;
            }

            if (!Win32Helper.keyRegistered)
            {
                if (!Win32Helper.RegisterHotKey((uint)this.config.ModifierKey, this.config.KeyCode))
                {
                    MessageBox.Show("Couldn't register configured HotKey. Maybe something is already registered on this combination.\r\nClosing myself.", this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                    this.calledClosing = true;
                    this.Close();
                }
            }
            
            this.focusedListBoxItem = false;
            this.SearchBox.Text = string.Empty;
            this.viewModel.SelectedIndex = 0;

            this.Visibility = Visibility.Collapsed;
            this.SizeToContent = SizeToContent.Manual;
        }
        
        private void ExecuteSelectedCommand()
        {
            if (this.viewModel.Execute())
            {
                this.DisableContent();
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            Win32Helper.applicationHandle = new WindowInteropHelper(this).Handle;
            if (Win32Helper.applicationHandle == IntPtr.Zero)
            {
                throw new Exception("Couldn't create Handle");
            }

            var source = PresentationSource.FromVisual(this) as HwndSource;
            source?.AddHook(this.WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == Win32Helper.WM_HOTKEY)
            {
                this.ActivateContent();
            }

            return IntPtr.Zero;
        }
    }
}
