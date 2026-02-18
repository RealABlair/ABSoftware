using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using ABSoftware.Networking.Packets;

namespace ABSoftware.Networking.ClientSide
{
    public class NetworkClient
    {
        public string IP { get; private set; }
        public int Port { get; private set; }

        public bool Active { get; private set; } = false;
        public TcpClient client { get; private set; }
        Thread networkThread;

        byte[] buffer = new byte[2048];

        ByteBuilder byteBuilder = new ByteBuilder();

        public NetworkClient(string IP, int Port)
        {
            this.IP = IP;
            this.Port = Port;
        }

        public bool Connect()
        {
            try
            {
                this.client = new TcpClient();
                this.client.Connect(IP, Port);

                Active = true;

                networkThread = new Thread(NetworkLoop);
                networkThread.Start();

                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public void Disconnect()
        {
            this.client.Close();
            Active = false;
            networkThread = null;
        }

        public void SendPacket(IPacket packet)
        {
            PacketBuffer buffer = new PacketBuffer();
            buffer.Write(packet.GetPacketId());
            packet.WritePacket(buffer);
            buffer.Append(PacketManager.PacketEnding);

            byte[] bytes = buffer.ToArray();

            this.client.GetStream().Write(bytes, 0, bytes.Length);
        }

        void NetworkLoop()
        {
            OnStart();
            try
            {
                while (Active)
                {
                    int count = client.GetStream().Read(buffer, 0, buffer.Length);
                    if (count < 1)
                    {
                        Disconnect();
                    }
                    else
                    {
                        byte[] bytes = buffer;
                        Array.Resize(ref bytes, count);

                        PacketFabricator(bytes);
                    }
                }
            }
            catch(Exception)
            {
                Disconnect();
            }
            OnStop();
        }

        void PacketFabricator(byte[] data)
        {
            byteBuilder.Append(data);
            if(byteBuilder.Contains(PacketManager.PacketEnding))
            {
                int endingIndex = -1;
                while ((endingIndex = byteBuilder.IndexOf(PacketManager.PacketEnding)) != -1)
                {
                    OnPacketIncome(PacketManager.GetPacket(byteBuilder.GetRange(0, endingIndex)));
                    byteBuilder.RemoveFirstElements(endingIndex + PacketManager.PacketEnding.Length);
                }
                //byteBuilder.Clear();
            }
        }

        public virtual void OnStart() { }
        public virtual void OnStop() { }
        public virtual void OnPacketIncome(IPacket packet) { }
    }
}

