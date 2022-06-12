using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PXLed.Controls
{
    public partial class ColorPicker : UserControl
    {
        public ColorPicker()
        {
            InitializeComponent();

            // Initialize the images
            satValImage.ColorFunc = GetSatValPixel;
            satValImage.Draw();
            
            hueImage.ColorFunc = GetHuePixel;
            hueImage.Draw();

            // Subscribe to relevent events
            satValImage.MouseMove += SatValImage_MouseMove;
            hueImage.MouseMove += HueImage_MouseMove;

            // Each input method has it's own update method in order not to do weird stuff to a control while the user is changing its value
            redSlider.ValueChanged += (s, e) => UpdateFromSliders(Color24.FromRGB((byte)e.NewValue, Value.g, Value.b));
            greenSlider.ValueChanged += (s, e) => UpdateFromSliders(Color24.FromRGB(Value.r, (byte)e.NewValue, Value.b));
            blueSlider.ValueChanged += (s, e) => UpdateFromSliders(Color24.FromRGB(Value.r, Value.g, (byte)e.NewValue));

            redUpDown.TextChanged += (s, e) => UpdateFromUpDowns(Color24.FromRGB((byte)redUpDown.Value, Value.g, Value.b));
            greenUpDown.TextChanged += (s, e) => UpdateFromUpDowns(Color24.FromRGB(Value.r, (byte)greenUpDown.Value, Value.b));
            blueUpDown.TextChanged += (s, e) => UpdateFromUpDowns(Color24.FromRGB(Value.r, Value.g, (byte)blueUpDown.Value));

            hexTextBox.TextChanged += (s, e) => UpdateFromHexBox();
        }

        public Color24 Value
        {
            get
            {
                return colorValue;
            }
            set
            {
                Color24 oldValue = colorValue;

                // Set internal color to value and update all the inputs
                colorValue = value;
                UpdateImages();
                UpdateSliders();
                UpdateUpDowns();
                UpdateHexBox();

                ValueChanged?.Invoke(this, new RoutedPropertyChangedEventArgs<Color24>(oldValue, value));
            }
        }

        public event RoutedPropertyChangedEventHandler<Color24> ValueChanged;

        void UpdateFromSliders(Color24 value)
        {
            Color24 oldValue = colorValue;

            // Update everything except Sliders
            colorValue = value;
            UpdateImages();
            UpdateUpDowns();
            UpdateHexBox();

            ValueChanged?.Invoke(this, new RoutedPropertyChangedEventArgs<Color24>(oldValue, value));
        }

        void UpdateFromUpDowns(Color24 value)
        {
            Color24 oldValue = colorValue;

            // Update everything except UpDowns
            colorValue = value;
            UpdateImages();
            UpdateSliders();
            UpdateHexBox();
            
            ValueChanged?.Invoke(this, new RoutedPropertyChangedEventArgs<Color24>(oldValue, value));
        }

        void UpdateFromHexBox()
        {
            // If text matches the required format
            if (Regex.Match(hexTextBox.Text, @"^#[a-fA-F0-9]{6}$").Success)
            {
                Color24 oldValue = colorValue;

                // Update everything except the hex text box
                colorValue = Color24.FromHex(hexTextBox.Text);
                UpdateImages();
                UpdateSliders();
                UpdateUpDowns();
            
                ValueChanged?.Invoke(this, new RoutedPropertyChangedEventArgs<Color24>(oldValue, colorValue));
            }
        }

        // Remember the last drawn color in order to know wether the images need to be redrawn
        Color24 colorValue = Color24.FromHSV(0d, 1d, 1d);
        Color24 lastDrawn = Color24.FromHSV(0d, 1d, 1d);

        void UpdateImages()
        {
            // Only update the images if the color isn't already drawn
            if (lastDrawn != Value)
            {
                satValImage.Draw();
                hueImage.Draw();

                lastDrawn = Value;
            }
        }

        void UpdateSliders()
        {
            redSlider.Value = Value.r;
            greenSlider.Value = Value.g;
            blueSlider.Value = Value.b;
        }

        void UpdateUpDowns()
        {
            redUpDown.Value = Value.r;
            greenUpDown.Value = Value.g;
            blueUpDown.Value = Value.b;
        }

        void UpdateHexBox()
        {
            hexTextBox.Text = colorValue.ToString();
        }

        Color24 GetSatValPixel(int x, int y)
        {
            // Calculate saturation and value at pixel position
            double pixelSat = (double)x / satValImage.PixelWidth;
            double pixelVal = (double)y / satValImage.PixelHeight * -1 + 1;

            Color24 baseColor = Color24.FromHSV(Value.Hue, pixelSat, pixelVal);

            // If the current selected color is nearby to the pixel being calculated, make it darker so that there's a marker for the current selection
            if (Math.Abs(pixelSat - Value.Saturation) < 0.03d && Math.Abs(pixelVal - Value.Value) < 0.03d)
                return Color24.LerpRGB(baseColor, Color24.FromRGB(0, 0, 0), 0.5f);
            else
                return baseColor;
        }

        Color24 GetHuePixel(int x, int y)
        {
            // Calculate hue at pixel height
            double hueAtHeight = (double)y / hueImage.PixelHeight * 360d;
            Color24 baseColor = Color24.FromHSV(hueAtHeight, 1d, 1d);

            // If the current selected color is nearby to the pixel being calculated, make it darker so that there's a marker for the current selection
            if (Math.Abs(hueAtHeight - Value.Hue) < 20d)
                return Color24.LerpRGB(baseColor, Color24.FromRGB(0, 0, 0), 0.5f);
            else
                return baseColor;
        }

        private void SatValImage_MouseMove(object sender, MouseEventArgs e)
        {
            // If dragging and clicking
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point clickPoint = e.GetPosition(satValImage);
                double normalizedX = clickPoint.X / satValImage.ActualWidth;
                double normalizedY = clickPoint.Y / satValImage.ActualHeight * -1 + 1;

                Value = Color24.FromHSV(Value.Hue, normalizedX, normalizedY);
            }
        }

        void HueImage_MouseMove(object sender, MouseEventArgs e)
        {
            // If dragging and clicking
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point clickPoint = e.GetPosition(hueImage);
                double normalizedHeight = clickPoint.Y / hueImage.ActualHeight;
                Value = Color24.FromHSV(normalizedHeight * 360d, Value.Saturation, Value.Value);
            }
        }
    }
}
