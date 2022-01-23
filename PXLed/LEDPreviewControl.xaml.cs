using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PXLed
{
    /// <summary>
    /// Displays a horizontal preview of LEDs using colored rectangles.
    /// </summary>
    public partial class LEDPreviewControl : UserControl
    {
        public LEDPreviewControl()
        {
            InitializeComponent();
            SizeChanged += (object sender, SizeChangedEventArgs args) => ResizeContent(ActualWidth, ActualHeight);
        }

        Rectangle[]? ledRects;

        public int LEDCount
        {
            get
            {
                return ledRects?.Length ?? 0;
            }

            set
            {
                // Reconstruct rects when changing number of displayed leds
                // Remove old rects
                panel.Children.Clear();                

                // Add new rects
                ledRects = new Rectangle[value];
                Random r = new Random();
                for (int i = 0; i < value; i++)
                {
                    ledRects[i] = new Rectangle();
                    panel.Children.Add(ledRects[i]);

                    // Make rect color random for debug
                    ledRects[i].Fill = new SolidColorBrush(Color24.Random());
                }

                // Resize new rects
                ResizeContent(ActualWidth, ActualHeight);
            }
        }

        public void SetColors(ref Color24[] colors)
        {
            // If colors in array is not equal to number of rects, reconstruct the rects
            if (colors.Length != ledRects?.Length)
                LEDCount = colors.Length;

            if (ledRects != null)
            {
                for (int i = 0; i < colors.Length; i++)
                {
                    ledRects[i].Fill = new SolidColorBrush(colors[i]);
                }
            }
        }

        void ResizeContent(double width, double height)
        {
            for (int i = 0; i < ledRects?.Length; i++)
            {
                ledRects[i].Width = width / ledRects.Length;
                ledRects[i].Height = height;
            }
        }
    }
}
