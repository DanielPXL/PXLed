using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;

namespace PXLed.Controls
{
    public partial class GradientPickerWindow : Window
    {
        public GradientPickerWindow()
        {
            InitializeComponent();

            Gradient = new();

            keyListImage.ColorFunc = DrawKeyListImage;
            keyListImage.Draw();

            gradientImage.ColorFunc = DrawGradientImage;
            gradientImage.Draw();

            keyListImage.MouseDown += KeyListImage_MouseDown;
            timeSlider.ValueChanged += TimeSlider_ValueChanged;
            colorPicker.ValueChanged += ColorPicker_ValueChanged;
        }

        public Gradient Gradient { get; set; }
        public event RoutedPropertyChangedEventHandler<Gradient>? ValueChanged;

        int selectedKey;

        public void Draw()
        {
            keyListImage.Draw();
            gradientImage.Draw();
        }

        void KeyListImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            float click = (float)(e.GetPosition(keyListImage).X / keyListImage.ActualWidth);
            List<ColorKey> keys = Gradient.GetKeys();

            for (int i = 0; i < keys.Count; i++)
            {
                if (Math.Abs(click - keys[i].time) < 0.02f)
                {
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        SelectKey(i);
                    } else if (e.RightButton == MouseButtonState.Pressed)
                    {
                        Gradient.RemoveKey(i);
                        SelectKey(0);
                        ValueChanged?.Invoke(this, new(Gradient, Gradient));
                    }

                    return;
                }
            }

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                int newIndex = Gradient.AddKey(Gradient.Evaluate(click), click);
                SelectKey(newIndex);
                ValueChanged?.Invoke(this, new(Gradient, Gradient));
            }
        }

        void TimeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            selectedKey = Gradient.UpdateKeyTime(selectedKey, (float)e.NewValue);

            Draw();

            ValueChanged?.Invoke(this, new(Gradient, Gradient));
        }

        void ColorPicker_ValueChanged(object sender, RoutedPropertyChangedEventArgs<Color24> e)
        {
            Gradient.UpdateKeyColor(selectedKey, e.NewValue);

            Draw();

            ValueChanged?.Invoke(this, new(Gradient, Gradient));
        }

        void SelectKey(int index)
        {
            selectedKey = index;

            List<ColorKey> keys = Gradient.GetKeys();
            timeSlider.Value = keys[index].time;
            colorPicker.Value = keys[index].color;

            Draw();
        }

        Color24 DrawKeyListImage(int x, int y)
        {
            List<ColorKey> keys = Gradient.GetKeys();

            for (int i = 0; i < keys.Count; i++)
            {
                if (Math.Abs((float)x / keyListImage.PixelWidth - keys[i].time) < 0.02f)
                {
                    return keys[i].color;
                }
            }

            return Color24.FromRGB(150, 150, 150);
        }

        Color24 DrawGradientImage(int x, int y)
        {
            return Gradient.Evaluate((float)x / gradientImage.PixelWidth);
        }
    }
}
