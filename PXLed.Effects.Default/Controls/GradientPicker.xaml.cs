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

            Gradient = new Gradient();
            Gradient.AddKey(Color24.FromHSV(144, 1, 1), 0.2f);

            gradientImage.ColorFunc = DrawGradient;
            gradientImage.Draw();

            gradientImage.MouseDown += GradientImage_MouseDown;
        }

        GradientPickerWindow? window;

        public Gradient Gradient { get; set; }

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

        private void Window_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<Gradient> e)
        {
            Gradient = e.NewValue;
            gradientImage.Draw();
        }

        Color24 DrawGradient(int x, int y)
        {
            return Gradient.Evaluate((float)x / gradientImage.PixelWidth);
        }
    }
}
