using System;
using System.Windows;

namespace CommandPalette
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

            this.KeyBox.PreviewKeyDown += this.viewModel.KeyBox_PreviewKeyDown;
            this.btnSave.Click += this.BtnSave_Click;
            this.btnCancel.Click += this.BtnCancel_Click;
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
