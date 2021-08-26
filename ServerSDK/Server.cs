using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

using ABSoftware.ServerSDK.ServerSideClient;
using ABSoftware.ServerSDK.Utils;

namespace ABSoftware.ServerSDK
{
    public class Server : Display
    {
        public static Server instance;

        Random rnd = new Random();

        List<Client> clients = new List<Client>();

        TcpListener tcpListener;
        Thread serverLoopThread;
        Thread consoleInputListener;
        bool isRunning = false;
        
        public int Port = -1;
        public int ClientBufferSize;

        public Ping pinger = new Ping();

        public string ServerName { set { Console.Title = value; } }

        public void StartNetwork(int port, int ClientBufferSize = 1024)
        {
            instance = this;
            Init();
            ServerName = $"'{GetType().Name}' - Server is supported by ABlair";
            this.Port = port;
            this.ClientBufferSize = ClientBufferSize;
            isRunning = true;
            consoleInputListener = new Thread(ConsoleListenLoop);
            consoleInputListener.Start();
            serverLoopThread = new Thread(ServerLoop);
            serverLoopThread.Start();
        }

        public void StopNetwork()
        {
            isRunning = false;
            DisconnectClients();
            clients.Clear();
            Port = -1;
            serverLoopThread.Join(200);
            serverLoopThread = null;
            tcpListener.Stop();
            tcpListener = null;
            OnServerStop();
        }

        
        void ServerLoop()
        {
            OnServerStart();
            tcpListener = new TcpListener(IPAddress.Any, Port);
            tcpListener.Start();
            while(isRunning)
            {
                if (tcpListener.Pending())
                {
                    TcpClient tcp = tcpListener.AcceptTcpClient();
                    IPEndPoint iep = (IPEndPoint)tcp.Client.RemoteEndPoint;
                    Client client = new Client();
                    client.ID = randomId();
                    client.IP = iep.ToString().Split(':')[0];
                    client.client = tcp;
                    client.Start();
                    clients.Add(client);
                    OnClientConnect(client);
                }
            }
        }

        void ConsoleListenLoop()
        {
            while(true)
            {
                string line = Console.ReadLine();
                OnConsoleInput(line);
            }
        }

        string randomId()
        {
            return $"{rnd.Next(0, 9999999).ToString("X2")}-{rnd.Next(0, 9999999).ToString("X2")}";
        }

        public bool ClientExists(Client client)
        {
            foreach(Client c in clients)
            {
                if (c == client) return true;
            }
            return false;
        }

        public Client GetClientById(string ID)
        {
            return clients.FirstOrDefault(s => Equals(s.ID, ID));
        }

        public bool TryGetClientById(string ID, out Client client)
        {
            client = clients.FirstOrDefault(s => Equals(s.ID, ID));
            if (client == null)
                return false;

            return true;
        }

        public void DisconnectClient(string ID)
        {
            Client c = clients.FirstOrDefault(f => Equals(f.ID, ID));
            if (c == null)
                return;
            c.Disconnect();
            OnClientDisconnect(c);  
            clients.Remove(c);
        }

        public void DisconnectClients()
        {
            foreach(Client c in clients.ToArray())
            {
                if (c == null)
                    continue;
                c.Disconnect();
                OnClientDisconnect(c);
                clients.Remove(c);
            }
        }

        public void SendPacket(string packet, string ID)
        {
            Client c = clients.FirstOrDefault(f => Equals(f.ID, ID));
            if (c == null)
                return;
            c.sendPacket(Encoding.UTF8.GetBytes(packet));
            OnOutgoingPacket(c, Encoding.UTF8.GetBytes(packet));
        }

        public void SendPacket(string packet)
        {
            foreach(Client c in clients)
            {
                if (c == null)
                    continue;
                c.sendPacket(Encoding.UTF8.GetBytes(packet));
                OnOutgoingPacket(c, Encoding.UTF8.GetBytes(packet));
            }
        }

        public void SendPacket(byte[] packet, string ID)
        {
            Client c = clients.FirstOrDefault(f => Equals(f.ID, ID));
            if (c == null)
                return;
            c.sendPacket(packet);
            OnOutgoingPacket(c, packet);
        }

        public void SendPacket(byte[] packet)
        {
            foreach (Client c in clients)
            {
                if (c == null)
                    continue;
                c.sendPacket(packet);
                OnOutgoingPacket(c, packet);
            }
        }

        public Client[] GetClients()
        {
            return clients.ToArray();
        }

        public virtual void OnServerStart() { }
        public virtual void OnServerStop() { }
        public virtual void OnClientConnect(Client connectedClient) { }
        public virtual void OnClientDisconnect(Client disconnectedClient) { }
        public virtual void OnIncomingPacket(Client client, byte[] packet) { }
        public virtual void OnOutgoingPacket(Client client, byte[] packet) { }
        public virtual void OnConsoleInput(string input) { }
        public virtual void OnPing(Client pingedClient, long ping) { }
    }
}
