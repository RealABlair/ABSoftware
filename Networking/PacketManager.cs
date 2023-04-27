using System;
using ABSoftware;
using ABSoftware.Networking.Packets;

namespace ABSoftware.Networking
{
    public class PacketManager
    {
        public static byte[] PacketEnding = new byte[] { 0xDE, 0xAD, 0xDA, 0xED, 0x1A };

        public static ArrayList<IPacket> registredPackets = new ArrayList<IPacket>();

        static PacketManager()
        {
            //Register packets here:
            RegisterPacket(new ExamplePacket());
        }

        public static void RegisterPacket(IPacket packet)
        {
            registredPackets.Add(packet);
        }

        public static IPacket GetPacket(byte[] data)
        {
            PacketBuffer buffer = new PacketBuffer(data);
            int packetId = buffer.Read<int>();

            for(int i = 0; i < registredPackets.Size; i++)
            {
                if (packetId.Equals(registredPackets[i].GetPacketId()))
                {
                    IPacket packet = registredPackets[i];
                    packet.ReadPacket(buffer);
                    return packet;
                }
            }

            return null;
        }
    }
}
