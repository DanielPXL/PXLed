using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PXLed;

namespace PXLed.Controls
{
    /// <summary>
    /// Represents an Image Control that can be drawn completely by a single function
    /// </summary>
    public partial class FuncImage : UserControl
    {
        public FuncImage()
        {
            InitializeComponent();

            bitmap = new(PixelHeight, PixelWidth, PixelHeight, PixelWidth, PixelFormats.Bgr32, null);
            
            image.Source = bitmap;
            image.Stretch = Stretch.Fill;
        }

        /// <summary>
        /// Will get called for each pixel in the bitmap and gets passed the x and y coordinate for that pixel
        /// </summary>
        public Func<int, int, Color24>? ColorFunc { get; set; }

        public int PixelHeight { get; set; } = 100;
        public int PixelWidth { get; set; } = 100;

        WriteableBitmap bitmap;

        public void Draw()
        {
            if (ColorFunc == null)
                throw new NullReferenceException($"{nameof(ColorFunc)} is null");

            // Adjust bitmap size
            if (PixelHeight != bitmap.PixelHeight || PixelWidth != bitmap.PixelWidth)
            {
                bitmap = new(PixelWidth, PixelHeight, PixelWidth, PixelHeight, PixelFormats.Bgr32, null);

                image.Source = bitmap;
                image.Stretch = Stretch.Fill;
            }

            // https://docs.microsoft.com/en-us/dotnet/api/system.windows.media.imaging.writeablebitmap
            try
            {
                bitmap.Lock();

                unsafe
                {
                    IntPtr pBackBuffer = bitmap.BackBuffer;

                    // Go through each pixel in bitmap
                    for (int x = 0; x < bitmap.PixelWidth; x++)
                    {
                        for (int y = 0; y < bitmap.PixelHeight; y++)
                        {
                            // Get color information from the function
                            int color = ColorFunc(x, y).ToInt24();

                            // Set the pixel in the back buffer to the color
                            *(int*)(pBackBuffer + (y * bitmap.BackBufferStride) + (x * 4)) = color;
                        }
                    }
                }

                // Make complete bitmap dirty
                bitmap.AddDirtyRect(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));
            }
            finally
            {
                bitmap.Unlock();
            }
        }
    }
}
