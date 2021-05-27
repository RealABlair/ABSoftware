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
        public Ping ping = new Ping();


        public void sendPacket(byte[] buffer) => client.GetStream().Write(buffer, 0, buffer.Length);

        public void Start()
        {
            
        }

        public void Disconnect()
        {
            client.Close();
        }
    }
}
