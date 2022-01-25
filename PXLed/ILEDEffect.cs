using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PXLed
{
    public interface ILEDEffect
    {
        public string DisplayName { get; }
        public float MaxFPS { get; }

        public void Update(ref Color24[] leds, float deltaTime);

        // Save data for config
        public void SetData(object data);
        public object GetData();
    }
}
