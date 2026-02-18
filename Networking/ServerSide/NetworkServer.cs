using ABSoftware.Networking.Packets;
using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
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

        internal bool useReflection = false;
        ArrayList<PacketHandlerData> packetHandlers = null;

        public NetworkServer(int Port)
        {
            this.Port = Port;
        }

        public NetworkServer(int Port, bool useReflection)
        {
            this.Port = Port;

            this.useReflection = useReflection;

            if (useReflection)
            {
                packetHandlers = new ArrayList<PacketHandlerData>();
            }
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
                this.listener.Stop();
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
            while(clients.Size > 0)
            {
                Client client = clients[0];
                if (client == null)
                    clients.Remove(client);
                else
                {
                    OnClientDisconnect(client);
                    client.Disconnect();
                    clients.Remove(client);
                }
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

        /// <summary>
        /// Registers a packet handler.
        /// </summary>
        /// <param name="handlerType">Handler instance</param>
        public void RegisterHandler(object handlerObject)
        {
            if (!useReflection)
                return;

            MethodInfo[] methodInfos = handlerObject.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic);
            for (int m = 0; m < methodInfos.Length; m++)
            {
                PacketHandlerAttribute attribute = methodInfos[m].GetPacketHandlerAttribute();
                if (attribute != null)
                {
                    bool done = false;
                    for (int i = 0; i < packetHandlers.Size; i++)
                    {
                        if (packetHandlers[i].packetType == attribute.packetType)
                        {
                            packetHandlers[i].packetHandlers.Add((methodInfos[m], methodInfos[m].IsStatic ? null : handlerObject));
                            done = true;
                        }
                    }

                    if (!done)
                    {
                        packetHandlers.Add(new PacketHandlerData(attribute.packetType));
                        packetHandlers[packetHandlers.Size - 1].packetHandlers.Add((methodInfos[m], methodInfos[m].IsStatic ? null : handlerObject));
                    }
                }
            }
        }

        /// <summary>
        /// Registers a packet handler. Note that non-static methods will not be registered.
        /// </summary>
        /// <param name="handlerType">Handler type</param>
        public void RegisterHandler(Type handlerType)
        {
            if (!useReflection)
                return;

            MethodInfo[] methodInfos = handlerType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic);
            for (int m = 0; m < methodInfos.Length; m++)
            {
                if (!methodInfos[m].IsStatic)
                    continue;
                PacketHandlerAttribute attribute = methodInfos[m].GetPacketHandlerAttribute();
                if (attribute != null)
                {
                    bool done = false;
                    for (int i = 0; i < packetHandlers.Size; i++)
                    {
                        if (packetHandlers[i].packetType == attribute.packetType)
                        {
                            packetHandlers[i].packetHandlers.Add((methodInfos[m], null));
                            done = true;
                        }
                    }

                    if (!done)
                    {
                        packetHandlers.Add(new PacketHandlerData(attribute.packetType));
                        packetHandlers[packetHandlers.Size - 1].packetHandlers.Add((methodInfos[m], null));
                    }
                }
            }
        }

        public virtual void OnStart() { }
        public virtual void OnStop() { }
        public virtual void OnClientConnect(Client client) { }
        public virtual void OnClientDisconnect(Client client) { }
        public virtual void OnPacketIncome(Client client, IPacket packet) { }
        internal void OnPacketIncomeReflection(Client client, IPacket packet)
        {
            Type packetType = packet.GetType();
            for (int i = 0; i < packetHandlers.Size; i++)
            {
                if (packetType == packetHandlers[i].packetType)
                {
                    for (int j = 0; j < packetHandlers[i].packetHandlers.Size; j++)
                    {
                        packetHandlers[i].packetHandlers[j].Item1.Invoke(packetHandlers[i].packetHandlers[j].Item2, new object[] { client, packet });
                    }
                    break;
                }
            }
        }
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
                    IPacket packet = PacketManager.GetPacket(byteBuilder.GetRange(0, endingIndex));
                    if(parent.useReflection)
                    {
                        parent.OnPacketIncomeReflection(this, packet);
                    }
                    parent.OnPacketIncome(this, packet);
                    byteBuilder.RemoveFirstElements(endingIndex + PacketManager.PacketEnding.Length);
                }
                //byteBuilder.Clear();
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
