using System;
using ABSoftware.ServerFiles.Utils;

namespace ABSoftware.ServerFiles.PacketUtils.Packets
{
    public class Packet
    {
        public int ID { get; internal set; }
        public int PacketDataSize { get; internal set; }
        public byte[] PacketData { get; private set; }

        public virtual void LoadPacket(byte[] packetData) { this.PacketData = packetData; }
        public virtual byte[] GetPacket() { return PacketData; }
    }
}