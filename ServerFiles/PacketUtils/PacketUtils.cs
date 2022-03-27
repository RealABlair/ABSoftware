using System;
using ABSoftware.ServerFiles.PacketUtils.Packets;

namespace ABSoftware.ServerFiles.PacketUtils
{
    public class PacketUtils
    {
        public static Packet GetPacket(int packetId) //Client packets
        {
            switch(packetId)
            {
                default:
                    return null;
            }
        }

        public enum PacketIds : int
        {
            Connected = 0x00
        }
    }
}
