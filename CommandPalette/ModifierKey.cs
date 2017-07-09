using System;
using CommandPalette.Helper;

namespace CommandPalette
{
    [Flags]
    public enum ModifierKey
    {
        None = 0,
        ALT = Win32Helper.MOD_ALT,
        LeftCTRL = Win32Helper.MOD_CONTROL,
        LeftShift = Win32Helper.MOD_SHIFT,
        Win = Win32Helper.MOD_WIN
    }
}