using System.Windows;

namespace PXLed
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Initialize LED Manager
            LEDManager ledManager = new(91, new ArduinoConnection("COM3", 230400), ledPreview);

            // Start SettingsEffect for testing
            // TODO: Implement actual effect switching and plugin loading
            ledManager.StartEffect(new SettingsEffect());
        }
    }
}
