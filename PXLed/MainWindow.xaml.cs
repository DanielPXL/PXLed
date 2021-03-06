using PXLed.Devices;
using PXLed.Effects;
using PXLed.Controls;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Threading;
using System;

namespace PXLed
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Load settings
            SettingsData settingsData = App.Config.GetData<SettingsData>();

            fpsCounter = new();

            // Create device
            if (settingsData.UseWiFi)
                device = new UdpDevice(settingsData.DeviceIP, settingsData.DevicePort);
            else
                device = new ArduinoDevice(settingsData.ArduinoPortName, settingsData.ArduinoBaudRate);
            
            ledManager = new(settingsData.NumLeds, device, ledPreview, fpsCounter);
            StateChanged += MainWindow_StateChanged;

            brightnessSlider.ValueChanged += (s, e) => ledManager.brightness = (float)e.NewValue;
            brightnessSlider.Value = settingsData.Brightness;

            // Get all available effects and make buttons that select them for each of them
            effects = GetEffects();
            MakeEffectButtons();

            SetCurrentEffect(effects[settingsData.CurrentEffectIndex]);

            StartFPSTimer();

            // TODO: Make it live in the system tray
            Closing += (s, e) => Application.Current.Shutdown();
        }

        FPSCounter fpsCounter;
        DispatcherTimer fpsDisplayTimer;

        ILEDDevice device;
        LEDManager ledManager;

        LEDEffectData[] effects;
        LEDEffectData? currentEffect;

        LEDEffectData[] GetEffects()
        {
            // SettingsEffect is added manually to ensure that it is placed first in the button list
            SettingsEffect settingsEffect = new();
            settingsEffect.applyButton.Click += (s, e) => RestartArduino();
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
            ledManager.StopEffect();

            // Stop gracefully with saving to config
            if (currentEffect != null)
            {
                currentEffect.Effect.OnStop();
                App.Config.Save();
            }
        }

        public void RestartArduino()
        {
            // Stop effect
            StopCurrentEffect();

            // Get Settings
            SettingsData settingsData = App.Config.GetData<SettingsData>();

            device.Dispose();
            
            // Create new device
            if (settingsData.UseWiFi)
                device = new UdpDevice(settingsData.DeviceIP, settingsData.DevicePort);
            else
                device = new ArduinoDevice(settingsData.ArduinoPortName, settingsData.ArduinoBaudRate);

            ledManager = new(settingsData.NumLeds, device, ledPreview, fpsCounter);

            // Restart effect
            SetCurrentEffect(currentEffect!);
        }

        public int GetCurrentEffectIndex()
        {
            return Array.IndexOf(effects, currentEffect);
        }

        private void MainWindow_StateChanged(object? sender, EventArgs e)
        {
            // Skip drawing the preview if the window is not visible to save on resources
            if (WindowState == WindowState.Minimized)
                ledManager.skipPreview = true;
            else
                ledManager.skipPreview = false;
        }

        void StartFPSTimer()
        {
            fpsDisplayTimer = new DispatcherTimer();
            fpsDisplayTimer.Tick += (s, e) => DisplayFPS();
            fpsDisplayTimer.Interval = new TimeSpan(0, 0, 0, 0, 250);
            fpsDisplayTimer.Start();
        }

        void DisplayFPS()
        {
            if (currentEffect == null)
                fpsLabel.Content = "FPS: 00.0 / Max: 00.0";
            else
                fpsLabel.Content = $"FPS: {fpsCounter.FPS:00.0} / Max: {currentEffect.MaxFPS:00.0}";
        }
    }
}
