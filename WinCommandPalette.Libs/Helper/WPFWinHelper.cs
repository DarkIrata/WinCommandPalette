using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace WinCommandPalette.Libs.Helper
{
    public static class WPFWinHelper
    {
        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private const int GWL_STYLE = -16;

        private const int WS_MAXIMIZEBOX = 0x10000;
        private const int WS_MINIMIZEBOX = 0x20000;

        [Flags]
        public enum TitleBarButtons
        {
            Minimaze = 1,
            Maximaze = 2
        }

        public static void DisableTitleBarButtons(Window window, TitleBarButtons buttons)
        {
            var windowHelper = new WindowInteropHelper(window);
            DisableTitleBarButtons(windowHelper.Handle, buttons);
        }

        public static void DisableTitleBarButtons(IntPtr handle, TitleBarButtons buttons)
        {
            if (handle == IntPtr.Zero)
            {
                throw new InvalidOperationException("The window has not yet been completely initialized");
            }

            var dwNewLongTitleBarButtons = 0;
            if (buttons.HasFlag(TitleBarButtons.Minimaze))
            {
                dwNewLongTitleBarButtons += WS_MINIMIZEBOX;
            }

            if (buttons.HasFlag(TitleBarButtons.Maximaze))
            {
                dwNewLongTitleBarButtons += WS_MAXIMIZEBOX;
            }

            SetWindowLong(handle, GWL_STYLE, GetWindowLong(handle, GWL_STYLE) & ~dwNewLongTitleBarButtons);
        }
    }
}
