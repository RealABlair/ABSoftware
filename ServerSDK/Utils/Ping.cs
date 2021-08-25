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
        List<PingWaiter> pings = new List<PingWaiter>();

        public void SendPing(Client pingClient)
        {
            Server.instance.Println(DateTime.Now.Ticks.ToString(), Color.Red);
            pings.Add(new PingWaiter() { client = pingClient, ping = DateTime.Now.Ticks });
            Server.instance.SendPacket(new byte[] { 0x00 }, pingClient.ID);
        }

        public void OnIncomingPacket(Client client, byte[] packet)
        {
            PingWaiter pw;
            if (PingIsSent(client.ID, out pw) && packet.Length == 1 && packet[0] == 0x00)
            {
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
            pings.Remove(pw);
            DateTime t1 = new DateTime(pw.ping);
            DateTime t2 = DateTime.Now;
            Server.instance.OnPing(pw.client, (long)(t2 - t1).TotalMilliseconds);
        }

        class PingWaiter
        {
            public Client client;
            public long ping;
        }
    }
}
