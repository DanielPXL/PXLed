using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PXLed.Effects
{
    public interface ILEDEffect
    {
        public void Update(ref Color24[] leds, float deltaTime);

        // Save data for config
        public void SetData(object data);
        public object GetData();
    }
}
