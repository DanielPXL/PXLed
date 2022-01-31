using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace PXLed.Effects
{
    public partial class SettingsEffect : UserControl, ILEDEffect
    {
        public SettingsEffect()
        {
            InitializeComponent();
        }

        public string DisplayName => "Settings";
        public float MaxFPS => 10f;

        public void Update(ref Color24[] leds, float deltaTime)
        {
            for (int i = 0; i < leds.Length; i++)
            {
                leds[i] = Color24.FromRGB(0, 0, 0);
            }

            // Display left and right border as red on led strip for setup
            leds[0] = Color24.FromRGB(255, 0, 0);
            leds[^1] = Color24.FromRGB(255, 0, 0);

            // Display middle led as blue for setup
            leds[leds.Length / 2] = Color24.FromRGB(0, 0, 255);
        }

        public void OnStart()
        {
            SettingsData data = App.Config.GetData<SettingsData>();
            numLedUpDown.Value = data.NumLeds;
            portNameBox.Text = data.ArduinoPortName;
            baudRateBox.Value = data.ArduinoBaudRate;
        }

        public void OnStop()
        {
            SettingsData data = new()
            {
                NumLeds = numLedUpDown.Value,
                ArduinoPortName = portNameBox.Text,
                ArduinoBaudRate = baudRateBox.Value
            };

            App.Config.SetData(data);
        }
    }

    public struct SettingsData
    {
        public int CurrentEffectIndex { get; set; } = 0;
        public double Brightness { get; set; } = 0.4d;
        public int NumLeds { get; set; } = 100;
        public string ArduinoPortName { get; set; } = "COM3";
        public int ArduinoBaudRate { get; set; } = 230400;
    }
}
