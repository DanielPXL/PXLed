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

            useWiFiCheckBox.Click += (s, e) => UpdateControlsIfUsingWifi();
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
            useWiFiCheckBox.IsChecked = data.UseWiFi;
            ipBox.Text = data.DeviceIP;
            portBox.Value = data.DevicePort;

            UpdateControlsIfUsingWifi();
        }

        public void OnStop()
        {
            SettingsData data = new()
            {
                NumLeds = numLedUpDown.Value,
                ArduinoPortName = portNameBox.Text,
                ArduinoBaudRate = baudRateBox.Value,
                UseWiFi = useWiFiCheckBox.IsChecked ?? false,
                DeviceIP = ipBox.Text,
                DevicePort = portBox.Value
            };

            App.Config.SetData(data);
        }

        void UpdateControlsIfUsingWifi()
        {
            // If using wifi, enable ip and port boxes and disable port name box and port box
            if (useWiFiCheckBox.IsChecked ?? false)
            {
                ipBox.IsEnabled = true;
                portBox.IsEnabled = true;

                portNameBox.IsEnabled = false;
                baudRateBox.IsEnabled = false;
            }
            else
            {
                ipBox.IsEnabled = false;
                portBox.IsEnabled = false;

                portNameBox.IsEnabled = true;
                baudRateBox.IsEnabled = true;
            }
        }
    }

    public struct SettingsData
    {
        public SettingsData(int currentEffectIndex, double brightness, int numLeds, string arduinoPortName, int arduinoBaudRate, bool useWiFi, string deviceIp, int devicePort)
        {
            CurrentEffectIndex = currentEffectIndex;
            Brightness = brightness;
            NumLeds = numLeds;
            ArduinoPortName = arduinoPortName;
            ArduinoBaudRate = arduinoBaudRate;
            UseWiFi = useWiFi;
            DeviceIP = deviceIp;
            DevicePort = devicePort;
        }

        public int CurrentEffectIndex { get; set; } = 0;
        public double Brightness { get; set; } = 0.4d;
        public int NumLeds { get; set; } = 100;
        public string ArduinoPortName { get; set; } = "COM3";
        public int ArduinoBaudRate { get; set; } = 500000;
        public bool UseWiFi { get; set; } = false;
        public string DeviceIP { get; set; } = "";
        public int DevicePort { get; set; } = 12241;
    }
}
