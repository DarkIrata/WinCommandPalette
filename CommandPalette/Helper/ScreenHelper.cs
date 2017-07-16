using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommandPalette.Helper
{
    public static class ScreenHelper
    {
        public static Screen GetScreenFromCursor()
        {
            return Screen.FromPoint(Control.MousePosition);
        }

        public static Screen GetPrimaryScreen()
        {
            return Screen.PrimaryScreen;
        }

        public static double GetAppCenterScreenWidth(double appWidth)
        {
            var activeScreen = GetScreenFromCursor();
            return (activeScreen.Bounds.Location.X + (activeScreen.WorkingArea.Width - appWidth) / 2);
        }
    }
}
