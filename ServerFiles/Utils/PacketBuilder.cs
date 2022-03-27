using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware.ServerFiles.Utils
{
    public class PacketBuilder
    {
        private Server server;

        public PacketBuilder(Server server)
        {
            this.server = server;
        }

        public byte[] Build(int packetId, byte[] packetData)
        {
            ByteBuilder bb = new ByteBuilder();
            bb.Append(BitConverter.GetBytes(packetId));
            bb.Append(BitConverter.GetBytes(packetData.Length));
            bb.Append(packetData);
            bb.Append(server.PacketEnding);
            return bb.ToArray();
        }

        public byte[] BuildWithoutEnding(int packetId, byte[] packetData)
        {
            ByteBuilder bb = new ByteBuilder();
            bb.Append(BitConverter.GetBytes(packetId));
            bb.Append(BitConverter.GetBytes(packetData.Length));
            bb.Append(packetData);
            return bb.ToArray();
        }
    }
}
