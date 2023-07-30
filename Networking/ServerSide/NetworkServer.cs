using ABSoftware.Networking.Packets;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ABSoftware.Networking.ServerSide
{
    public class NetworkServer
    {
        public int Port { get; private set; }

        public bool Active { get; private set; }

        TcpListener listener;
        Thread networkThread;
        ABRandom random = new ABRandom();

        public ArrayList<Client> clients = new ArrayList<Client>();

        public NetworkServer(int Port)
        {
            this.Port = Port;
        }

        public void Start()
        {
            if(!Active)
            {
                Active = true;
                this.networkThread = new Thread(NetworkLoop);
                this.listener = new TcpListener(IPAddress.Any, Port);
                this.networkThread.Start();
            }
        }

        public void Stop()
        {
            if(Active)
            {
                Active = false;
                this.listener = null;
                this.networkThread = null;
            }
        }

        string GetRandomID()
        {
            byte b1 = (byte)((random.GetRandom() >> 24) & 0xFF);
            ushort b2 = (ushort)((random.GetRandom() >> 8) & 0xFFFF);
            uint b3 = random.GetRandom();

            return $"{b1:X2}-{b2:X4}-{b3:X8}";
        }

        void NetworkLoop()
        {
            OnStart();
            try
            {
                this.listener.Start();
                while (Active)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    IPEndPoint ipep = (IPEndPoint)client.Client.RemoteEndPoint;
                    Client serversideClient = new Client(client, ipep.Address.ToString(), ipep.Port, this);
                    serversideClient.ID = GetRandomID();
                    clients.Add(serversideClient);
                    OnClientConnect(serversideClient);
                }
            }
            catch(Exception) { }
            this.listener.Stop();
            OnStop();
        }

        public void Disconnect(Client client)
        {
            OnClientDisconnect(client);
            client.Disconnect();
            clients.Remove(client);
        }

        public void Disconnect(string ID)
        {
            Client client = clients.FirstOrDefault(c => c.ID.Equals(ID));
            if (client == null)
                return;
            OnClientDisconnect(client);
            client.Disconnect();
            clients.Remove(client);
        }

        public void Disconnect()
        {
            for(int i = 0; i < clients.Size; i++)
            {
                Client client = clients[i];
                if (client == null)
                    return;
                OnClientDisconnect(client);
                client.Disconnect();
                clients.Remove(client);
            }
        }

        public void SendPacket(Client client, IPacket packet)
        {
            PacketBuffer buffer = new PacketBuffer();
            buffer.Write(packet.GetPacketId());
            packet.WritePacket(buffer);
            buffer.Append(PacketManager.PacketEnding);

            client.SendPacket(buffer.ToArray());
        }

        public void SendPackets(Client client, params IPacket[] packets)
        {
            ByteBuilder buffer = new ByteBuilder();
            for(int i = 0; i < packets.Length; i++)
            {
                PacketBuffer packetBuffer = new PacketBuffer();
                packetBuffer.Write(packets[i].GetPacketId());
                packets[i].WritePacket(packetBuffer);
                packetBuffer.Append(PacketManager.PacketEnding);

                buffer.Append(packetBuffer.ToArray());
            }

            client.SendPacket(buffer.ToArray());
        }

        public void SendPacket(string ID, IPacket packet)
        {
            Client client = clients.FirstOrDefault(c => c.ID.Equals(ID));
            if (client == null)
                return;
            PacketBuffer buffer = new PacketBuffer();
            buffer.Write(packet.GetPacketId());
            packet.WritePacket(buffer);
            buffer.Append(PacketManager.PacketEnding);

            client.SendPacket(buffer.ToArray());
        }

        public void SendPacket(Client[] clients, IPacket packet)
        {
            for(int i = 0; i < clients.Length; i++)
            {
                if (clients[i] == null) continue;

                PacketBuffer buffer = new PacketBuffer();
                buffer.Write(packet.GetPacketId());
                packet.WritePacket(buffer);
                buffer.Append(PacketManager.PacketEnding);

                clients[i].SendPacket(buffer.ToArray());
            }
        }

        public void SendPacket(IPacket packet)
        {
            PacketBuffer buffer = new PacketBuffer();
            buffer.Write(packet.GetPacketId());
            packet.WritePacket(buffer);
            buffer.Append(PacketManager.PacketEnding);
            for (int i = 0; i < clients.Size; i++)
            {
                Client client = clients[i];
                if (client == null)
                    return;
                client.SendPacket(buffer.ToArray());
            }
        }

        public virtual void OnStart() { }
        public virtual void OnStop() { }
        public virtual void OnClientConnect(Client client) { }
        public virtual void OnClientDisconnect(Client client) { }
        public virtual void OnPacketIncome(Client client, IPacket packet) { }
    }

    public class Client
    {
        public NetworkServer parent;

        public string ID { get; set; }
        public string IP { get; private set; }
        public int Port { get; private set; }

        public TcpClient client { get; private set; }

        public bool Active { get; private set; }

        Thread networkThread;
        byte[] buffer = new byte[2048];
        ByteBuilder byteBuilder = new ByteBuilder();

        public Client(TcpClient client, string IP, int Port, NetworkServer parent)
        {
            this.parent = parent;
            this.client = client;
            this.IP = IP;
            this.Port = Port;
            this.Active = true;
            networkThread = new Thread(NetworkLoop);
            networkThread.Start();
        }

        void NetworkLoop()
        {
            try
            {
                while (Active)
                {
                    int count = client.GetStream().Read(buffer, 0, buffer.Length);
                    if (count < 1)
                    {
                        parent.Disconnect(ID);
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
                parent.Disconnect(ID);
            }
        }

        void PacketFabricator(byte[] data)
        {
            byteBuilder.Append(data);
            if (byteBuilder.Contains(PacketManager.PacketEnding))
            {
                int endingIndex = -1;
                while ((endingIndex = byteBuilder.IndexOf(PacketManager.PacketEnding)) != -1)
                {
                    parent.OnPacketIncome(this, PacketManager.GetPacket(byteBuilder.GetRange(0, endingIndex)));
                    byteBuilder.RemoveFirstElements(endingIndex + PacketManager.PacketEnding.Length);
                }
                byteBuilder.Clear();
            }
        }

        public void SendPacket(byte[] data)
        {
            try
            {
                client.GetStream().Write(data, 0, data.Length);
            }
            catch(Exception) { }
        }

        public void Disconnect()
        {
            this.Active = false;
            this.networkThread = null;
            if(this.client != null)
            {
                this.client.Close();
                this.client = null;
            }
        }
    }
}
