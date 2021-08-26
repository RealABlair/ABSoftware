using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using ABSoftware.ServerSDK.Utils;

namespace ABSoftware.ServerSDK.ServerSideClient
{
    public class Client
    {
        public string ID;
        public string IP;
        public TcpClient client;

        byte[] buffer = new byte[Server.instance.ClientBufferSize];
        Thread packetListener = null;
        bool listeningPackets = false;

        public void sendPacket(byte[] buffer) => client.GetStream().Write(buffer, 0, buffer.Length);

        public void Start()
        {
            packetListener = new Thread(PacketBuffer);
            packetListener.Start();
        }

        void PacketBuffer()
        {
            try
            {
                listeningPackets = true;
                while (listeningPackets)
                {
                    int count = client.GetStream().Read(buffer, 0, buffer.Length);
                    if (count < 1)
                    {
                        Server.instance.DisconnectClient(ID);
                    }
                    else
                    {
                        byte[] bytes = buffer;
                        Array.Resize(ref bytes, count);
                        Server.instance.pinger.OnIncomingPacket(this, bytes);
                        Server.instance.OnIncomingPacket(this, bytes);
                    }
                }
            }
            catch (Exception ex)
            {
                Server.instance.DisconnectClient(ID);
            }
        }

        public void Disconnect()
        {
            client.Close();
        }
    }
}
