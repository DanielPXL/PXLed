using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PXLed.Effects
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class LEDEffectAttribute : Attribute
    {
        public LEDEffectAttribute(string displayName, float maxFps)
        {
            DisplayName = displayName;
            MaxFPS = maxFps;
        }

        public string DisplayName { get; }
        public float MaxFPS { get;  }
    }
}
