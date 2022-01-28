using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;

namespace PXLed.Devices
{
    /// <summary>
    /// Connection to an Arduino using SerialPorts
    /// </summary>
    public class ArduinoDevice : ILEDDevice, IDisposable
    {
        public const byte START_BYTE = 254;
        public const byte STOP_BYTE = 255;
        public const byte ACKNOWLEDGE_BYTE = 100;

        public ArduinoDevice(string portName, int baudRate)
        {
            // Read buffer for receiving acknowledgement byte
            readBuffer = new byte[1];

            // Try to open new SerialPort to and connect to Arduino
            try
            {
                port = new SerialPort(portName, baudRate, Parity.None, 8);
                port.Open();
            } catch (Exception ex)
            {
                App.ShowError(ex);
            }
        }

        private readonly SerialPort? port;
        private readonly byte[] readBuffer;

        public void SendColors(ref Color24[] colors, float brightness)
        {
            // Array length = rgb count + start byte + stop byte
            byte[] byteArray = new byte[colors.Length * 3 + 2];

            // Start & Stop bytes
            byteArray[0] = START_BYTE;
            byteArray[byteArray.Length - 1] = STOP_BYTE;

            // Color data
            for (int i = 0; i < colors.Length; i++)
            {
                byteArray[i * 3 + 1] = (byte)Math.Min(253f, (float)colors[i].r * brightness);
                byteArray[i * 3 + 2] = (byte)Math.Min(253f, (float)colors[i].g * brightness);
                byteArray[i * 3 + 3] = (byte)Math.Min(253f, (float)colors[i].b * brightness);
            }

            port!.Write(byteArray, 0, byteArray.Length);

            // Wait for acknowledgment
            int tries = 0;
            while (true)
            {
                if (port!.BytesToRead > 0)
                {                    
                    port.Read(readBuffer, 0, readBuffer.Length);
                    if (readBuffer[0] == ACKNOWLEDGE_BYTE)
                    {
                        break;
                    }
                }

                // Timeout after ten tries
                if (tries > 10)
                {
                    break;
                }

                // Sleep for a bit to make the cpu not work it's ass off as much
                Thread.Sleep(1);
                tries++;
            }
        }

        public void Dispose()
        {
            port?.Close();
            port?.Dispose();
        }
    }
}
