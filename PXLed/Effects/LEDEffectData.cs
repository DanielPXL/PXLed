using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PXLed.Effects
{
    public class LEDEffectData
    {
        public LEDEffectData(string displayName, float maxFPS, ILEDEffect effect)
        {
            DisplayName = displayName;
            MaxFPS = maxFPS;
            Effect = effect;
        }

        public string DisplayName { get; }
        public float MaxFPS { get; }
        public ILEDEffect Effect { get; }
    }
}
