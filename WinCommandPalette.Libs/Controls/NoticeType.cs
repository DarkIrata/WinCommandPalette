using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace WinCommandPalette.Libs.Controls
{
    public enum NoticeType
    {
        None,
        Info,
        Success,
        Error,
        Warning
    }

    public static class NoticeTypeHelper
    {
        public static void SetColorByNoticeType(this Border border, NoticeType noticeType)
        {
            switch (noticeType)
            {
                default:
                case NoticeType.Info:
                    border.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xFC, 0xFC, 0xFC));
                    border.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xAA, 0xB8, 0xC6));
                    break;
                case NoticeType.Success:
                    border.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xF3, 0xF9, 0xF4));
                    border.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x91, 0xC8, 0x9C));
                    break;
                case NoticeType.Error:
                    border.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xF8, 0xF7));
                    border.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xD0, 0x44, 0x37));
                    break;
                case NoticeType.Warning:
                    border.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFD, 0xF6));
                    border.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xEA, 0xAE));
                    break;
            }
        }
    }
}
