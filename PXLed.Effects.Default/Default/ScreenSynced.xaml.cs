using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Controls;

namespace PXLed.Effects.Default
{
    [LEDEffect("Screen Synced", 60f)]
    public partial class ScreenSynced : UserControl, ILEDEffect
    {
        public ScreenSynced()
        {
            InitializeComponent();

            screenRow = new Bitmap(1920, 1, PixelFormat.Format32bppArgb);
            gdest = Graphics.FromImage(screenRow);
            gsrc = Graphics.FromHwnd(IntPtr.Zero);
        }

        ~ScreenSynced()
        {
            screenRow.Dispose();
            gdest.Dispose();
            gsrc.Dispose();
        }

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);

        Bitmap screenRow;
        Graphics gdest;
        Graphics gsrc;

        public void Update(ref Color24[] leds, float deltaTime)
        {
            GetRow(400);
            
            for (int i = 0; i < leds.Length; i++)
            {
                Color c = screenRow.GetPixel((int)((float)i / leds.Length * screenRow.Width), 0);
                leds[i] = Color24.FromRGB(c.R, c.G, c.B);
            }
        }

        // https://stackoverflow.com/a/1483963
        public void GetRow(int y)
        {
            IntPtr hSrcDC = gsrc.GetHdc();
            IntPtr hDC = gdest.GetHdc();
            int retval = BitBlt(hDC, 0, 0, screenRow.Width, 1, hSrcDC, 0, y, (int)CopyPixelOperation.SourceCopy);
            gdest.ReleaseHdc();
            gsrc.ReleaseHdc();
        }

        public void OnStart()
        {
            
        }

        public void OnStop()
        {
            
        }
    }
}
