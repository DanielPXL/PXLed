using System.Windows.Controls;

namespace PXLed.Effects.Default
{
    [LEDEffect("Solid Color", 5f)]
    public partial class SolidColorEffect : UserControl, ILEDEffect
    {
        public SolidColorEffect()
        {
            InitializeComponent();
        }

        public void Update(ref Color24[] leds, float deltaTime)
        {
            for (int i = 0; i < leds.Length; i++)
            {
                leds[i] = colorPicker.Value;
            }
        }

        public void OnStart()
        {
            // Get data from config
            SolidColorEffectData data = App.Config.GetData<SolidColorEffectData>();
            colorPicker.Value = data.Color;
        }

        public void OnStop()
        {
            // Save data to config
            SolidColorEffectData data = new() { Color = colorPicker.Value };
            App.Config.SetData(data);
        }
    }

    public struct SolidColorEffectData
    {
        public SolidColorEffectData(Color24 color)
        {
            Color = color;
        }

        public Color24 Color { get; set; } = new Color24();
    }
}
