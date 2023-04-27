using System;

namespace ABSoftware.Networking.Packets
{
    public class ExamplePacket : IPacket
    {
        public ExamplePacket()
        {

        }

        public int data;

        public ExamplePacket(int data)
        {
            this.data = data;
        }

        public void WritePacket(PacketBuffer buffer)
        {
            buffer.Write<int>(data);
        }

        public void ReadPacket(PacketBuffer buffer)
        {
            this.data = buffer.Read<int>();
        }

        public int GetPacketId() => 0;
    }
}
