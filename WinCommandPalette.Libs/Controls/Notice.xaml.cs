using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WinCommandPalette.Libs.Controls
{
    [ContentProperty("Text")]
    public partial class Notice : UserControl
    {

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(Notice), new PropertyMetadata("Missing Infotext"));

        public string Text
        {
            get => (string)this.GetValue(TextProperty);
            set => this.SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty NoticeTypeProperty =
            DependencyProperty.Register("NoticeType", typeof(NoticeType), typeof(Notice), new PropertyMetadata(NoticeType.None, new PropertyChangedCallback(OnNoticeTypeChanged)));


        public NoticeType NoticeType
        {
            get => (NoticeType)this.GetValue(NoticeTypeProperty);
            set
            {
                this.SetValue(NoticeTypeProperty, value);
            }
        }

        public Notice()
        {
            this.InitializeComponent();
        }

        private static void OnNoticeTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Notice notice)
            {
                notice.BaseControl.SetColorByNoticeType(notice.NoticeType);
            }
        }
    }
}
