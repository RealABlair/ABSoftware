using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using ABSoftware.Networking.Packets;
using System.Reflection;

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

        internal bool useReflection = false;
        ArrayList<PacketHandlerData> packetHandlers = null;

        public NetworkClient(string IP, int Port)
        {
            this.IP = IP;
            this.Port = Port;
        }

        public NetworkClient(string IP, int Port, bool useReflection)
        {
            this.IP = IP;
            this.Port = Port;
            this.useReflection = useReflection;

            if(useReflection)
            {
                packetHandlers = new ArrayList<PacketHandlerData>();
            }
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
                    IPacket packet = PacketManager.GetPacket(byteBuilder.GetRange(0, endingIndex));
                    if(useReflection)
                    {
                        OnPacketIncomeReflection(packet);
                    }
                    OnPacketIncome(packet);
                    byteBuilder.RemoveFirstElements(endingIndex + PacketManager.PacketEnding.Length);
                }
                //byteBuilder.Clear();
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
        public virtual void OnPacketIncome(IPacket packet) { }
        internal void OnPacketIncomeReflection(IPacket packet)
        {
            Type packetType = packet.GetType();
            for(int i = 0; i < packetHandlers.Size; i++)
            {
                if(packetType == packetHandlers[i].packetType)
                {
                    for(int j = 0; j < packetHandlers[i].packetHandlers.Size; j++)
                    {
                        packetHandlers[i].packetHandlers[j].Item1.Invoke(packetHandlers[i].packetHandlers[j].Item2, new object[] { packet });
                    }
                    break;
                }
            }
        }
    }
}
