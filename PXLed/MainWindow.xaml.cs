using PXLed.Devices;
using PXLed.Effects;
using System.Windows;

namespace PXLed
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            FPSCounter fpsCounter = new FPSCounter((float fps) => fpsLabel.Dispatcher.Invoke(() => DisplayFPS(fps)));

            // Initialize LED Manager
            LEDManager ledManager = new(91, new ArduinoDevice("COM3", 230400), ledPreview, fpsCounter);

            // Start SettingsEffect for testing
            // TODO: Implement actual effect switching and plugin loading
            ledManager.StartEffect(new SettingsEffect());
        }

        void DisplayFPS(float fps)
        {
            // TODO: Display max fps for current effect
            fpsLabel.Content = $"FPS: {fps:00.0} / Max: {60}";
        }
    }
}
