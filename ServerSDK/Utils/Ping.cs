using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABSoftware.ServerSDK.ServerSideClient;

namespace ABSoftware.ServerSDK.Utils
{
    public class Ping
    {
        Random rnd = new Random();
        List<PingWaiter> pings = new List<PingWaiter>();

        public void SendPing(Client pingClient)
        {
            byte[] bytes = Encoding.UTF8.GetBytes("ping" + GetNewID(pingClient));
            Server.instance.SendPacket(bytes, pingClient.ID);
            PingWaiter pw = new PingWaiter() { client = pingClient, ping = DateTime.Now.Ticks, pingPacket = bytes.Skip(4).ToArray() };
            pings.Add(pw);
        }

        public void OnIncomingPacket(Client client, byte[] packet)
        {
            PingWaiter pw = null;
            if (PingIsSent(client.ID, out pw))
            {
                if (pw.pingPacket.Length.Equals(packet.Length) && Enumerable.SequenceEqual(pw.pingPacket, packet))
                    ApplyPing(pw);
            }
        }

        bool PingIsSent(string ID, out PingWaiter waiter)
        {
            PingWaiter pw = pings.FirstOrDefault(id => Equals(id.client.ID, ID));
            waiter = pw;
            return pw != null;
        }

        void ApplyPing(PingWaiter pw)
        {
            DateTime t1 = new DateTime(pw.ping);
            DateTime t2 = DateTime.Now;
            Server.instance.OnPing(pw.client, (long)(t2 - t1).TotalMilliseconds);
            pings.Remove(pw);
        }

        string GetNewID(Client client)
        {
            return client.ID + "-" + rnd.Next(0, 999999).ToString("X2");
        }

        class PingWaiter
        {
            public Client client;
            public byte[] pingPacket;
            public long ping;
        }
    }
}
