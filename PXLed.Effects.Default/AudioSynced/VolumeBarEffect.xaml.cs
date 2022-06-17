using System;
using System.Windows.Controls;
using NAudio;
using NAudio.CoreAudioApi;

namespace PXLed.Effects.AudioSynced
{
    [LEDEffect("Volume Bar", 90f)]
    public partial class VolumeBarEffect : UserControl, ILEDEffect
    {
        public VolumeBarEffect()
        {
            InitializeComponent();

            volumeSlider.ValueChanged += (s, e) => volumeMultiplier = (float)e.NewValue;
            gravitySlider.ValueChanged += (s, e) => gravity = (float)e.NewValue;
        }

        MMDevice? device;
        float lastVolume;

        float volumeMultiplier;
        float gravity;

        public void Update(ref Color24[] leds, float deltaTime)
        {
            // Device can't be selected in the constructor because of threading issues
            if (device == null)
            {
                // Get default audio device
                // TODO: Make audio device selectable
                MMDeviceEnumerator enumarator = new();
                device = enumarator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
            }

            // Get current volume
            float volume = device.AudioMeterInformation.MasterPeakValue * volumeMultiplier;
            // Make it slowly go back to zero by taking the last volume if it's bigger than current volume
            lastVolume = Math.Max(lastVolume, volume);

            float middle = leds.Length / 2f;
            for (int i = 0; i < leds.Length; i++)
            {
                // Center the volume bar
                float distance = Math.Abs(i - middle);
                float normalizedDistance = 1f / lastVolume * distance;

                leds[i] = gradientPicker.Gradient.Evaluate(normalizedDistance);
            }

            // Decrease last volume until it is zero
            if (lastVolume > 0f)
                lastVolume -= gravity;
            if (lastVolume < 0f)
                lastVolume = 0f;
        }

        public void OnStart()
        {
            // Get data from config
            VolumeBarEffectData data = App.Config.GetData<VolumeBarEffectData>();

            gradientPicker.Gradient = data.Gradient;
            volumeMultiplier = data.Volume;
            gravity = data.Gravity;

            volumeSlider.Value = volumeMultiplier;
            gravitySlider.Value = gravity;
        }

        public void OnStop()
        {
            // Save data to config
            VolumeBarEffectData data = new() { Gradient = gradientPicker.Gradient, Volume = volumeMultiplier, Gravity = gravity };
            App.Config.SetData(data);
        }
    }

    public struct VolumeBarEffectData
    {
        public VolumeBarEffectData(Gradient gradient, float volume, float gravity)
        {
            Gradient = gradient;
            Volume = volume;
            Gravity = gravity;
        }

        public Gradient Gradient { get; set; } = new Gradient();
        public float Volume { get; set; } = 400f;
        public float Gravity { get; set; } = 1f;
    }
}
