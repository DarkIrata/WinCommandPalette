using WinCommandPalette.Helper;
using System;

namespace WinCommandPalette.Enums
{
    [Flags]
    public enum ModifierKey
    {
        None = 0,
        ALT = HotKeyHelper.MOD_ALT,
        LeftCTRL = HotKeyHelper.MOD_CONTROL,
        LeftShift = HotKeyHelper.MOD_SHIFT,
        Win = HotKeyHelper.MOD_WIN
    }
}