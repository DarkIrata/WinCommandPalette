using System;
using System.Runtime.InteropServices;

namespace CommandPalette.Helper
{
    internal static class Win32Helper
    {
        public const int MOD_ALT = 1;

        public const int MOD_CONTROL = 2;

        public const int MOD_SHIFT = 4;

        public const int MOD_WIN = 8;

        public const int WM_HOTKEY = 786;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hwnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    }
}
