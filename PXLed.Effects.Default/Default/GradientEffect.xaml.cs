using System.Windows.Controls;

namespace PXLed.Effects.Default
{
    [LEDEffect("Gradient", 60f)]
    public partial class GradientEffect : UserControl, ILEDEffect
    {
        public GradientEffect()
        {
            InitializeComponent();
            zoomSlider.ValueChanged += (s, e) => zoom = (float)zoomSlider.Value;
            speedSlider.ValueChanged += (s, e) => speed = (float)speedSlider.Value;
        }

        float time;
        float speed;
        float zoom;

        public void Update(ref Color24[] leds, float deltaTime)
        {
            for (int i = 0; i < leds.Length; i++)
            {
                leds[i] = gradientPicker.Gradient.Evaluate(((float)i / (leds.Length - 1) * zoom + time * speed) % 1f);
            }

            time += deltaTime;
        }

        public void OnStart()
        {
            // Get data from config
            GradientEffectData data = App.Config.GetData<GradientEffectData>();

            gradientPicker.Gradient = data.Gradient;

            zoom = data.Zoom;
            speed = data.Speed;

            zoomSlider.Value = zoom;
            speedSlider.Value = speed;
        }

        public void OnStop()
        {
            // Save data to config
            GradientEffectData data = new() { Gradient = gradientPicker.Gradient, Speed = speed, Zoom = zoom };
            App.Config.SetData(data);
        }
    }

    public struct GradientEffectData
    {
        public GradientEffectData(Gradient gradient, float speed, float zoom)
        {
            Gradient = gradient;
            Speed = speed;
            Zoom = zoom;
        }

        public Gradient Gradient { get; set; } = new Gradient();
        public float Speed { get; set; } = 2f;
        public float Zoom { get; set; } = 1f;
    }
}
