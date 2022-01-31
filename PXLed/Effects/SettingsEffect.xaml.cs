﻿using System;
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
        public float MaxFPS => 60f;

        float time = 0f;
        int index = 0;

        // just testing
        public void Update(ref Color24[] leds, float deltaTime)
        {
            time += deltaTime;
            index++;
            index %= 2;

            for (int i = 0; i < leds.Length; i++)
            {
                leds[i] = Color24.FromHSV(i * 10d + time * 30f, 1d, 1d);
            }

            //for (int i = 0; i < leds.Length; i++)
            //{
            //    leds[i] = Color24.FromHSV(0d, 0d, index);
            //}
        }

        public void OnStart()
        {
            SettingsData data = App.Config.GetData<SettingsData>();
            numLedUpDown.Value = data.NumLeds;
        }

        public void OnStop()
        {
            SettingsData data = new() { NumLeds = numLedUpDown.Value };
            App.Config.SetData(data);
        }
    }

    public struct SettingsData
    {
        public float Brightness { get; set; } = 0.4f;
        public int NumLeds { get; set; } = 100;
    }
}
