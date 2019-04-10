using System;

namespace Artorio
{
    static class Extra
    {
        public static System.Windows.Media.Color ToWPFColor(System.Drawing.Color colorFrom)
        {
            return System.Windows.Media.Color.FromArgb(colorFrom.A, colorFrom.R, colorFrom.G, colorFrom.B);
        }

        public static System.Drawing.Color ToWinFormColor(System.Windows.Media.Color colorFrom)
        {
            return System.Drawing.Color.FromArgb(colorFrom.A, colorFrom.R, colorFrom.G, colorFrom.B);
        }
    }
}
