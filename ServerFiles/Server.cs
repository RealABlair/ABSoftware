using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using ABSoftware.ServerFiles.Utils;

namespace ABSoftware.ServerFiles
{
    public class Server
    {
        private Random rnd = new Random();
        private Thread serverLoop = null;
        private Thread consoleLoop = null;
        private TcpListener tcpListener = null;

        public List<Client> clients = new List<Client>();

        public int Port { get; private set; }
        public bool Awake = false;
        public DateTime Time { get { return DateTime.Now; } }

        public byte[] PacketEnding = new byte[] { 0xFF, 0x00, 0xFA, 0xAF, 0xAA };

        public PacketBuilder packetBuilder = null;

        public string ServerName { get { return Console.Title; } set { Console.Title = value; } }

        public bool Start(int port)
        {
            if(!Awake)
            {
                ServerName = GetType().Name;
                this.Port = port;
                serverLoop = new Thread(ServerLoop);
                serverLoop.Start();
                consoleLoop = new Thread(ConsoleLoop);
                consoleLoop.Start();
                packetBuilder = new PacketBuilder(this);
                Awake = true;
                return true;
            }
            return false;
        }

        public bool Stop()
        {
            if(Awake)
            {
                Disconnect();
                Awake = false;
                tcpListener.Stop();
                serverLoop.Abort();
                serverLoop = null;
                tcpListener = null;
                packetBuilder = null;
                OnServerStop();
                return true;
            }
            return false;
        }

        public bool SendPacket(byte[] data, string ID)
        {
            Client target = clients.FirstOrDefault(c => Equals(c.ID, ID));
            if (target == null)
                return false;
            target.Send(data);
            return true;
        }

        public void BroadcastPacket(byte[] data)
        {
            foreach(Client client in clients.ToArray())
            {
                if (client != null)
                    client.Send(data);
            }
        }

        public void BroadcastPacket(byte[] data, Client[] except = null)
        {
            foreach (Client client in clients.ToArray())
            {
                if (client != null && (except == null || !except.Contains(client)))
                    client.Send(data);
            }
        }

        public bool Disconnect(string ID)
        {
            Client target = clients.FirstOrDefault(c => Equals(c.ID, ID));
            if (target == null)
                return false;
            OnClientDisconnect(target);
            clients.Remove(target);
            target.Disconnect();
            return true;
        }

        public void Disconnect()
        {
            foreach (Client client in clients.ToArray())
            {
                if (client != null)
                {
                    OnClientDisconnect(client);
                    clients.Remove(client);
                    client.Disconnect();
                }    
            }
        }

        private void ServerLoop()
        {
            OnServerStart();

            tcpListener = new TcpListener(IPAddress.Any, Port);
            tcpListener.Start();
            while(Awake)
            {
                TcpClient client = tcpListener.AcceptTcpClient();
                Client serverSideClient = new Client(client, GenerateNewID());
                serverSideClient.connectedTo = this;
                clients.Add(serverSideClient);
                OnClientConnect(serverSideClient);
            }
        }

        private void ConsoleLoop()
        {
            while(true)
            {
                OnConsoleInput(Console.ReadLine());
            }
        }

        private string GenerateNewID()
        {
            return $"{rnd.Next(0, 999999)}-{rnd.Next(0, 999999)}-{rnd.Next(0, 999999)}";
        }

        #region Server
        public virtual void OnServerStart() { }
        public virtual void OnServerStop() { }
        public virtual void OnClientConnect(Client client) { }
        public virtual void OnClientDisconnect(Client client) { }
        public virtual void OnIncomingPacket(Client client, byte[] packet) { }
        public virtual void OnOutgoingPacket(Client client, byte[] packet) { }
        #endregion

        #region Console
        public virtual void OnConsoleInput(string input) { }
        #endregion
    }
}