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

        public ILEDEffect? CurrentEffect { get; private set; }

        public Color24[] colors;

        private readonly ILEDDevice ledDevice;
        private readonly LEDPreviewControl? preview;
        private readonly FPSCounter? fpsCounter;
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

        // Very much inspired by https://gamedev.stackexchange.com/a/69769
        void EffectLoop()
        {
            int now;
            int last = Environment.TickCount;

            while (renderThread!.IsAlive && CurrentEffect != null)
            {
                // Do stuff with the fps counter in order to record the fps correctly for displaying
                fpsCounter?.StopFrame();
                fpsCounter?.StartFrame();

                float optimalTime = 1000f / CurrentEffect.MaxFPS;
             
                now = Environment.TickCount;
                int updateLength = now - last;
                float delta = updateLength / 1000f;
                last = now;

                // Execute effect
                CurrentEffect.Update(ref colors, delta);

                // Send new colors to LEDs
                ledDevice.SendColors(ref colors, 0.4f);

                try
                {
                    // Send new colors to the preview
                    // Execute on UI thread in order to prevent an exception
                    preview?.Dispatcher.Invoke(() => preview.SetColors(ref colors));
                } catch (TaskCanceledException)
                {
                    // Don't really know why but MainWindow closing causes this exception to throw

                    // Because the MainWindow is closing and the process is being stopped, break so that the thread exits cleanly
                    break;
                }

                int sleepTime = last - Environment.TickCount + (int)optimalTime;
                if (sleepTime > 0)
                {
                    Thread.Sleep(sleepTime);
                }
            }
        }
    }
}
