using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware.ServerSDK.Utils
{
    public class Ping
    {
        public long ms;

        DateTime lastPingTime;

        public void Update()
        {
            ms = (DateTime.Now - lastPingTime).Milliseconds;
            lastPingTime = DateTime.Now;
        }
    }
}
