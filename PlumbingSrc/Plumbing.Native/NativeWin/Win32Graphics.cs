#if NET452
using Plisky.Win32;
using System;
using System.Drawing;

namespace Plisky.Native {

    public class Win32Graphics {

        public static Color GetPixelFromScreen(Point position) {
            Color result;
            IntPtr hdc = NativeMethods.GetDC(IntPtr.Zero);
            try {
                uint pix = NativeMethods.GetPixel(hdc, position.X, position.Y);
                int redValue = (int)(pix & 0x000000FF);
                int greenValue = (int)(pix >> 8 & 0x000000FF);
                int blueValue = (int)(pix >> 16 & 0x000000FF);

                result = Color.FromArgb(redValue, greenValue, blueValue);
            } finally {
                NativeMethods.ReleaseDC(IntPtr.Zero, hdc);
            }
            return result;
        }
    }
}
#endif