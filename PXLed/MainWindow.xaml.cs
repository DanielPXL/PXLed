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
            // TODO: Make Arduino device settings changeable through settings
            ledManager = new(91, new ArduinoDevice("COM3", 230400), ledPreview, fpsCounter);

            // Get all available effects and make buttons that select them for each of them
            effects = GetEffects();
            MakeEffectButtons();

            // TODO: Save last used effect in config
            SetCurrentEffect(effects[0]);
        }

        FPSCounter fpsCounter;
        LEDManager ledManager;

        LEDEffectData[] effects;
        LEDEffectData? currentEffect;

        LEDEffectData[] GetEffects()
        {
            // SettingsEffect is added manually to ensure that it is placed first in the button list
            SettingsEffect settingsEffect = new();
            LEDEffectData settingsEffectData = new(settingsEffect.DisplayName, settingsEffect.MaxFPS, settingsEffect);

            List<LEDEffectData> foundEffects = PluginFinder.FindEffects();
            return foundEffects.Prepend(settingsEffectData).ToArray();
        }

        void MakeEffectButtons()
        {
            // Clear buttons if this is not the first time calling the function
            effectButtonPanel.Children.Clear();

            for (int i = 0; i < effects.Length; i++)
            {
                // Make button
                Button effectButton = new();
                effectButton.Height = 60;
                effectButton.Content = effects[i].DisplayName;

                // Make it so that Button calls SetCurrentEffect for its assigned effect
                LEDEffectData effectForButton = effects[i];
                effectButton.Click += (s, e) => SetCurrentEffect(effectForButton);

                // Add buttons to the panel in the scroll view
                effectButtonPanel.Children.Add(effectButton);
            }
        }

        void SetCurrentEffect(LEDEffectData effect)
        {
            // Let effect know that we are stopping it
            StopCurrentEffect();

            currentEffect = effect;

            // Get UI of effect and display it
            effectContentBox.Content = (UserControl)effect.Effect;
            effectContentBox.Header = effect.DisplayName;

            // Let effect know that we are starting it
            currentEffect.Effect.OnStart();

            // Start effect
            ledManager.StartEffect(effect);
        }

        public void StopCurrentEffect()
        {
            // Stop gracefully with saving to config
            if (currentEffect != null)
            {
                currentEffect.Effect.OnStop();
                App.Config.Save();
            }
        }

        // TODO: Make this actually get called
        void DisplayFPS()
        {
            if (currentEffect == null)
                fpsLabel.Content = "FPS: 00.0 / Max: 00.0";
            else
                fpsLabel.Content = $"FPS: {fpsCounter.FPS:00.0} / Max: {currentEffect.MaxFPS:00.0}";
        }
    }
}
