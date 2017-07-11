using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.IO;
using System.Windows.Media.Imaging;
using System.Drawing;
using wf = System.Windows.Forms;
using CommandPalette.Helper;

namespace CommandPalette
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel viewModel;
        private Config config;
        private bool focusedListBoxItem = false;

        public MainWindow()
        {
            PluginLoader.Load();

            this.config = new Config();
            if (File.Exists("config.xml"))
            {
                this.config = Config.Load("config.xml");
            }
            
            this.viewModel = new MainWindowViewModel(this.config);
            this.InitializeComponent();
            this.DataContext = this.viewModel;

            this.Loaded += this.MainWindow_Loaded;
            this.Closing += this.MainWindow_Closing;
            this.GotKeyboardFocus += this.MainWindow_GotKeyboardFocus;
            this.Deactivated += this.MainWindow_Deactivated;

            this.SearchBox.PreviewKeyDown += this.SearchBox_PreviewKeyDown;
            this.SearchBox.TextChanged += this.viewModel.SearchBox_TextChanged;
            this.SearchBox.LostKeyboardFocus += this.SearchBox_LostKeyboardFocus;
        }

        private void SearchBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.DisableContent();
                this.SearchBox.Text = string.Empty;
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

        private void MainWindow_Deactivated(object sender, EventArgs e)
        {
            this.DisableContent();
        }

        private void SearchBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!this.focusedListBoxItem)
            {
                this.DisableContent();
            }
        }

        private void MainWindow_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            this.SearchBox.Focus();
            Keyboard.Focus(this.SearchBox);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var rect = wf.Screen.FromHandle(Win32Helper.applicationHandle).Bounds;
            this.Top = rect.Height * 0.20;

            this.DisableContent();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.config.Save("config.xml");
            Win32Helper.UnregisterHotKey();
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
                Win32Helper.UnregisterHotKey();

                this.Visibility = Visibility.Visible;
                this.SizeToContent = SizeToContent.Height;

                var activeScreen = wf.Screen.FromPoint(wf.Control.MousePosition);
                this.Left = (activeScreen.Bounds.Location.X + (activeScreen.WorkingArea.Width - this.Width) / 2);

                this.viewModel.SelectedIndex = 0;
                this.focusedListBoxItem = false;
                Win32Helper.SetForegroundWindow(Win32Helper.applicationHandle);
            }

            return IntPtr.Zero;
        }

        private void DisableContent()
        {
            if (!Win32Helper.keyRegistered)
            {
                if (!Win32Helper.RegisterHotKey((uint)this.config.ModifierKey, this.config.KeyCode))
                {
                    MessageBox.Show("Error register HotKey. Killing myself", "Command Pattern", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                }
            }

            this.Visibility = Visibility.Collapsed;
            this.SizeToContent = SizeToContent.Manual;
        }
        
        private void ExecuteSelectedCommand()
        {
            if (this.viewModel.Execute())
            {
                this.SearchBox.Text = string.Empty;
                this.DisableContent();
            }
        }
    }
}
