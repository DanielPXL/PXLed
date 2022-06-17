using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PXLed.Devices
{
    public interface ILEDDevice : IDisposable
    {
        public void SendColors(ref Color24[] colors, float brightness);        
    }
}
