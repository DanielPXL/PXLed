using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PXLed
{
    public class FPSCounter
    {
        public FPSCounter(Action<float>? callback = null)
        {
            this.callback = callback;
        }

        public float FPS {
            get 
            {
                return fps;                    
            }
        }

        private float fps;
        
        private readonly Action<float>? callback;

        private long startTime;

        public void StartFrame(long ticksNow)
        {
            startTime = ticksNow;
        }

        public void StopFrame(long ticksNow)
        {
            // Calculate delta time in ticks
            long deltaTicks = ticksNow - startTime;
            if (deltaTicks > 0)
            {
                // fps for one tick
                float currentFps = (float)TimeSpan.TicksPerSecond / deltaTicks;

                // Lerp smoothes the value with previous value
                fps = Lerp(fps, currentFps, 0.1f);

                // Invoke callback
                callback?.Invoke(fps);
            }
        }

        static float Lerp(float a, float b, float t)
        {
            return a * (1 - t) + b * t;
        }
    }
}
