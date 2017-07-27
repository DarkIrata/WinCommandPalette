using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace WinCommandPalette.Controls
{
    /// <summary>
    /// Interaktionslogik für LicenseInfo.xaml
    /// </summary>
    public partial class LicenseInfo : UserControl
    {
        public static readonly DependencyProperty ProjectProperty =
            DependencyProperty.Register("Project", typeof(string), typeof(LicenseInfo), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty CopyrightProperty =
            DependencyProperty.Register("Copyright", typeof(string), typeof(LicenseInfo), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty LicenseProperty =
            DependencyProperty.Register("LicenseType", typeof(string), typeof(LicenseInfo), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty ProjectURLProperty =
            DependencyProperty.Register("ProjectURL", typeof(string), typeof(LicenseInfo), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty LicenseURLProperty =
            DependencyProperty.Register("LicenseURL", typeof(string), typeof(LicenseInfo), new PropertyMetadata(string.Empty));

        public string Project
        {
            get => (string)this.GetValue(ProjectProperty);
            set => this.SetValue(ProjectProperty, value);
        }

        public string Copyright
        {
            get => (string)this.GetValue(CopyrightProperty);
            set => this.SetValue(CopyrightProperty, value);
        }

        public string License
        {
            get => (string)this.GetValue(LicenseProperty);
            set => this.SetValue(LicenseProperty, value);
        }

        public string ProjectURL
        {
            get => (string)this.GetValue(ProjectURLProperty);
            set => this.SetValue(ProjectURLProperty, value);
        }

        public string LicenseURL
        {
            get => (string)this.GetValue(LicenseURLProperty);
            set => this.SetValue(LicenseURLProperty, value);
        }
           
        public LicenseInfo()
        {
            this.InitializeComponent();
            this.ProjectText.MouseUp += this.ProjectText_MouseUp;
            this.LicenseText.MouseUp += this.LicenseText_MouseUp;
        }

        private void LicenseText_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.OpenURL(this.LicenseURL);
        }

        private void ProjectText_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.OpenURL(this.ProjectURL);
        }

        private void OpenURL(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return;
            }

            try
            {
                Process.Start(url);
            }
            catch {}
        }
    }
}
