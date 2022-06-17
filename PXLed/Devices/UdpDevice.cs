using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PXLed.Devices
{
    /// <summary>
    /// Connection to a device using Udp
    /// </summary>
    public class UdpDevice : ILEDDevice
    {
        public const byte ACKNOWLEDGE_BYTE = 100;

        public UdpDevice(string ipAdress, int port)
        {
            endPoint = new IPEndPoint(IPAddress.Parse(ipAdress), port);

            sendBuffer = new byte[1];

            // Try to open new SerialPort to and connect to Arduino
            try
            {
                client = new UdpClient(port);
            }
            catch (Exception ex)
            {
                App.ShowError(ex);
            }
        }

        private readonly UdpClient? client;
        private IPEndPoint endPoint;
        private byte[] sendBuffer;
        byte[] received;

        public void SendColors(ref Color24[] colors, float brightness)
        {
            // Array length = rgb count
            if (sendBuffer.Length != colors.Length * 3)
                sendBuffer = new byte[colors.Length * 3];

            // Color data
            for (int i = 0; i < colors.Length; i++)
            {
                sendBuffer[i * 3] = (byte)Math.Min(253f, (float)colors[i].r * brightness);
                sendBuffer[i * 3 + 1] = (byte)Math.Min(253f, (float)colors[i].g * brightness);
                sendBuffer[i * 3 + 2] = (byte)Math.Min(253f, (float)colors[i].b * brightness);
            }

            // Port can close at any time so we need to be as sure as possible that it isn't
            if (client == null)
                return;

            client!.Send(sendBuffer, sendBuffer.Length, endPoint);

            //// Wait for acknowledgment
            //int tries = 0;
            //while (true)
            //{
            //    // Port can close at any time so we need to be as sure as possible that it isn't
            //    if (client == null)
            //        return;

            //    if (client!.Available > 0)
            //    {
            //        received = client.Receive(ref endPoint);
            //        if (received[0] == ACKNOWLEDGE_BYTE)
            //        {
            //            break;
            //        }
            //    }

            //    // Timeout after ten tries
            //    if (tries > 10)
            //    {
            //        break;
            //    }

            //    // Sleep for a bit to make the cpu not work it's ass off as much
            //    Thread.Sleep(1);
            //    tries++;
            //}
        }

        public void Dispose()
        {
            client?.Close();
            client?.Dispose();
        }
    }
}
