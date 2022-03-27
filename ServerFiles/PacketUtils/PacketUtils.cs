using System;
using ABSoftware.ServerFiles.PacketUtils.Packets;

namespace ABSoftware.ServerFiles.PacketUtils
{
    public class PacketUtils
    {
        public static Packet GetPacket(int packetId)
        {
            switch(packetId)
            {
                case (int)PacketIds.Message:
                    return new MessagePacket();
                case (int)PacketIds.Nickname:
                    return new NicknamePacket();
                case (int)PacketIds.UserIsReady:
                    return new UserIsReadyPacket();
                case (int)PacketIds.WordAnswer:
                    return new WordAnswer();
                default:
                    return null;
            }
        }

        public enum PacketIds : int
        {
            Connected = 0x00,
            Message = 0x01,
            Nickname = 0x02,
            UserIsReady = 0x03,
            StartGuessing = 0x04,
            WordAnswer = 0x05,
        }
    }
}
