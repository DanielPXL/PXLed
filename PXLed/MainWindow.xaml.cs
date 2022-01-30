using PXLed.Devices;
using PXLed.Effects;
using PXLed.Controls;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace PXLed
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            fpsCounter = new();
            ledManager = new(91, new ArduinoDevice("COM3", 230400), ledPreview, fpsCounter);

            effects = GetEffects();
            MakeEffectButtons();
        }

        FPSCounter fpsCounter;
        LEDManager ledManager;

        LEDEffectData[] effects;
        int currentEffect;

        LEDEffectData[] GetEffects()
        {
            SettingsEffect settingsEffect = new();
            LEDEffectData settingsEffectData = new(settingsEffect.DisplayName, settingsEffect.MaxFPS, settingsEffect);

            List<LEDEffectData> foundEffects = PluginFinder.FindEffects();
            return foundEffects.Prepend(settingsEffectData).ToArray();
        }

        void MakeEffectButtons()
        {
            for (int i = 0; i < effects.Length; i++)
            {
                Button effectButton = new();
                effectButton.Height = 60;
                effectButton.Content = effects[i].DisplayName;
                effectButton.Click += (s, e) => SetCurrentEffect(i);

                effectButtonPanel.Children.Add(effectButton);
            }
        }

        void SetCurrentEffect(int index)
        {
            effectContentBox.Content = (UserControl)effects[index].Effect;
            effectContentBox.Header = effects[index].DisplayName;
        }

        void DisplayFPS(float fps)
        {
            // TODO: Display max fps for current effect
            fpsLabel.Content = $"FPS: {fps:00.0} / Max: {60}";
        }
    }
}
