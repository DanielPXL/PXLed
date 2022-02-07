using System.Windows.Controls;
using PXLed.Effects;

namespace PXLed.Effects.Default
{
    [LEDEffect("Rainbow", 60f)]
    public partial class RainbowEffect : UserControl, ILEDEffect
    {
        public RainbowEffect()
        {
            InitializeComponent();
            zoomSlider.ValueChanged += (s, e) => zoom = (float)zoomSlider.Value;
            speedSlider.ValueChanged += (s, e) => speed = (float)speedSlider.Value;
        }

        float time;
        float zoom;
        float speed;

        public void Update(ref Color24[] leds, float deltaTime)
        {
            for (int i = 0; i < leds.Length; i++)
            {
                leds[i] = Color24.FromHSV(i * zoom + time * speed, 1f, 1f);
            }

            time += deltaTime;
        }

        public void OnStart()
        {
            // Load data from config
            RainbowEffectData data = App.Config.GetData<RainbowEffectData>();

            zoom = data.Zoom;
            speed = data.Speed;

            zoomSlider.Value = zoom;
            speedSlider.Value = speed;
        }

        public void OnStop()
        {
            // Save data to config
            RainbowEffectData data = new() { Speed = speed, Zoom = zoom };
            App.Config.SetData(data);
        }
    }

    public struct RainbowEffectData
    {
        public float Zoom { get; set; } = 10f;
        public float Speed { get; set; } = 50f;
    }
}
