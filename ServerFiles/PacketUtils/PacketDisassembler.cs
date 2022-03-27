using System;
using ABSoftware.ServerFiles.PacketUtils.Packets;
using ABSoftware.ServerFiles.Utils;

namespace ABSoftware.ServerFiles.PacketUtils
{
    public class PacketDisassembler
    {
        static readonly ByteBuilder byteBuilder = new ByteBuilder();

        public static Packet Disassemble(byte[] data)
        {
            if (data.Length < 8)
                return null;
            byteBuilder.Clear();
            byteBuilder.Append(data);
            int PacketID = BitConverter.ToInt32(byteBuilder.GetRange(0, 4), 0);
            Packet packet = PacketUtils.GetPacket(PacketID);
            if (packet == null)
                return null;
            int PacketSize = BitConverter.ToInt32(byteBuilder.GetRange(4, 4), 0); //8
            packet.ID = PacketID;
            packet.PacketDataSize = PacketSize;
            packet.LoadPacket(byteBuilder.GetRange(8, PacketSize));
            return packet;
        }
    }
}