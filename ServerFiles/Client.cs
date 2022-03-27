using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ABSoftware.ServerFiles.Utils;

namespace ABSoftware.ServerFiles
{
    public class Client
    {
        public Server connectedTo { get; internal set; }
        private byte[] buffer = new byte[4096];
        private ByteBuilder byteBuilder = new ByteBuilder();
        private Thread clientThread = null;
        private TcpClient client = null;

        public string IP { get; private set; }
        public string ID { get; private set; }
        public bool Awake { get; private set; }

        public Client(TcpClient tcpClient, string ID)
        {
            this.ID = ID;
            this.client = tcpClient;
            this.IP = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString();
            this.Awake = true;
            this.clientThread = new Thread(ClientLoop);
            this.clientThread.Start();
        }

        internal void Send(byte[] data)
        {
            client.GetStream().Write(data, 0, data.Length);
            client.GetStream().Flush();
        }

        private void ClientLoop()
        {
            try
            {
                while (Awake)
                {
                    int count = client.GetStream().Read(buffer, 0, buffer.Length);
                    if (count < 1)
                    {
                        throw new Exception();
                    }
                    else
                    {
                        byte[] bytes = buffer;
                        Array.Resize(ref bytes, count);
                        CheckBytes(bytes);
                    }
                    //Console.WriteLine(count);
                }
            }
            catch(Exception)
            {
                connectedTo.Disconnect(ID);
            }
        }

        private void CheckBytes(byte[] data)
        {
            byteBuilder.Append(data);
            if(byteBuilder.EndsWith(connectedTo.PacketEnding))
            {
                int i = -1;
                while((i = byteBuilder.IndexOf(connectedTo.PacketEnding)) != -1)
                {
                    connectedTo.OnIncomingPacket(this, byteBuilder.GetRange(0, i+connectedTo.PacketEnding.Length));
                    byteBuilder.RemoveFirstElements(i + connectedTo.PacketEnding.Length);
                }
                byteBuilder.Clear();
            }
        }

        public void Disconnect()
        {
            Awake = false;
            clientThread.Abort();
            clientThread = null;
            client.GetStream().Close();
            client = null;
        }
    }
}