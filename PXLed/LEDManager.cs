using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PXLed
{
    public class LEDManager
    {
        public LEDManager(int ledCount, ArduinoConnection arduino, LEDPreviewControl preview)
        {
            colors = new Color24[ledCount];
            this.arduino = arduino;
            this.preview = preview;
        }

        public ILEDEffect? CurrentEffect { get; private set; }

        public Color24[] colors;

        private readonly ArduinoConnection arduino;
        private readonly LEDPreviewControl preview;
        private Thread? renderThread;

        public void StartEffect(ILEDEffect effect)
        {
            // Set current effect so that the render thread switches to new effect
            CurrentEffect = effect;

            // If render thread has not started yet or was stopped for some reason, start new render thread
            if (renderThread == null || !renderThread.IsAlive)
            {
                renderThread = new Thread(new ThreadStart(EffectLoop));
                renderThread.Start();
            }
        }

        void EffectLoop()
        {
            int now = Environment.TickCount;
            int last;

            while (renderThread!.IsAlive && CurrentEffect != null)
            {
                // Record last frame's time and current frame's time for delta time
                last = now;
                now = Environment.TickCount;

                // Wait until next frame has to be rendered (determined by MaxFPS property on effect)
                int timeToWait = (int)Math.Round((1000f / CurrentEffect.MaxFPS) - (now - last));
                if (timeToWait > 0)
                {
                    Thread.Sleep(timeToWait);
                }

                // Execute effect
                CurrentEffect.Update(ref colors, (now - last) / 1000f);

                // Send new colors to LEDs
                arduino.SendColorArray(ref colors, 0.4f);

                try
                {
                    // Send new colors to the preview
                    // Execute on UI thread in order to prevent an exception
                    preview.Dispatcher.Invoke(() => preview.SetColors(ref colors));
                } catch (TaskCanceledException)
                {
                    // Don't really know why but MainWindow closing causes this exception to throw
                    // Because the MainWindow is closing and the process is being stopped, break so that the thread exits cleanly
                    break;
                }
            }
        }
    }
}
