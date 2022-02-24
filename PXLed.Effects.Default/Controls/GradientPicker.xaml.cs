using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace PXLed.Controls
{
    public partial class GradientPicker : UserControl
    {
        public GradientPicker()
        {
            InitializeComponent();

            gradientImage.ColorFunc = DrawGradient;
            Gradient = new Gradient();

            gradientImage.MouseDown += GradientImage_MouseDown;
        }

        GradientPickerWindow? window;

        public Gradient Gradient
        {
            get
            {
                return gradient;
            }
            set
            {
                gradient = value;
                gradientImage.Draw();
            }
        }

        Gradient gradient;

        private void GradientImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (window == null)
            {
                window = new();
                window.ShowInTaskbar = false;

                window.Gradient = Gradient;
                window.Draw();

                window.Deactivated += (s, e) => window.Hide();
                window.Closing += (s, e) => { window.Hide(); e.Cancel = true; };
                window.ValueChanged += Window_ValueChanged;
            }

            window.Show();
        }

        private void Window_ValueChanged(object sender, RoutedPropertyChangedEventArgs<Gradient> e)
        {
            Gradient = e.NewValue;
        }

        Color24 DrawGradient(int x, int y)
        {
            return Gradient.Evaluate((float)x / gradientImage.PixelWidth);
        }
    }
}
