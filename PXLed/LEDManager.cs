using PXLed.Controls;
using PXLed.Devices;
using PXLed.Effects;
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
        public LEDManager(int ledCount, ILEDDevice ledDevice, LEDPreviewControl? preview = null, FPSCounter? fpsCounter = null)
        {
            colors = new Color24[ledCount];
            this.ledDevice = ledDevice;
            this.preview = preview;
            this.fpsCounter = fpsCounter;
        }

        public LEDEffectData? CurrentEffect { get; private set; }

        public Color24[] colors;
        public float brightness;

        private readonly ILEDDevice ledDevice;
        private readonly LEDPreviewControl? preview;
        private readonly FPSCounter? fpsCounter;
        private Thread? renderThread;

        public void StartEffect(LEDEffectData effect)
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

        // Very much inspired by https://gamedev.stackexchange.com/a/69769
        void EffectLoop()
        {
            long now;
            long last = DateTime.UtcNow.Ticks;

            while (renderThread!.IsAlive && CurrentEffect != null)
            {
                now = DateTime.UtcNow.Ticks;

                // Do stuff with the fps counter in order to record the fps correctly for displaying
                fpsCounter?.StopFrame(now);
                fpsCounter?.StartFrame(now);

                float optimalTime = 1000f / CurrentEffect.MaxFPS;
             
                long updateLength = now - last;
                float delta = updateLength / (float)TimeSpan.TicksPerSecond;
                last = now;

                // Execute effect
                CurrentEffect.Effect.Update(ref colors, delta);

                // Send new colors to LEDs
                ledDevice.SendColors(ref colors, brightness);

                try
                {
                    // Send new colors to the preview
                    // Execute on UI thread in order to prevent an exception
                    preview?.Dispatcher.Invoke(() => preview.SetColors(ref colors));
                }
                catch (TaskCanceledException)
                {
                    // Don't really know why but MainWindow closing causes this exception to throw

                    // Because the MainWindow is closing and the process is being stopped, break so that the thread exits cleanly
                    break;
                }

                // Wait until enough time has passed for the given frames per second
                float sleepTime = (now - DateTime.UtcNow.Ticks) / (float)TimeSpan.TicksPerMillisecond + optimalTime;
                while (sleepTime > 0f)
                {
                    Thread.Sleep((int)(sleepTime / 2f));
                    sleepTime = (now - DateTime.UtcNow.Ticks) / (float)TimeSpan.TicksPerMillisecond + optimalTime;
                }
            }
        }

        public void StopEffect()
        {
            if (renderThread != null)
                CurrentEffect = null;
        }
    }
}
