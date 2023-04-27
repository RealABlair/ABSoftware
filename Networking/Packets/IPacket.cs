using System;
using ABSoftware;

namespace ABSoftware.Networking.Packets
{
    public interface IPacket
    {
        void WritePacket(PacketBuffer buffer);
        void ReadPacket(PacketBuffer buffer);
        int GetPacketId();
    }
}
