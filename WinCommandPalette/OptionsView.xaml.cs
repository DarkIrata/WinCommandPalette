using System;
using System.Windows;
using WinCommandPalette.Helper;

namespace WinCommandPalette
{
    /// <summary>
    /// Interaktionslogik für OptionsView.xaml
    /// </summary>
    public partial class OptionsView : Window
    {
        private Config config;
        private OptionsViewModel viewModel;

        public OptionsView(Config config)
        {
            this.config = config ??
                throw new ArgumentNullException(nameof(config));

            this.viewModel = new OptionsViewModel(this.config);

            this.InitializeComponent();
            this.DataContext = this.viewModel;

            this.Closed += this.OptionsView_Closed;
            this.KeyBox.PreviewKeyDown += this.viewModel.KeyBox_PreviewKeyDown;

            this.btnSaveNewCommand.Click += this.viewModel.BtnSaveNewCommand_Click;

            this.btnSave.Click += this.BtnSave_Click;
            this.btnCancel.Click += this.BtnCancel_Click;

            Win32Helper.UnregisterHotKey();
        }

        private void OptionsView_Closed(object sender, EventArgs e)
        {
            Win32Helper.RegisterHotKey((uint)this.config.ModifierKey, this.config.KeyCode);
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            this.viewModel.Save();
            this.Close();
        }
    }
}