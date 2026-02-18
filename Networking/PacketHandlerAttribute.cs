using System;
using System.Reflection;

namespace ABSoftware.Networking
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class PacketHandlerAttribute : Attribute
    {
        public PacketHandlerAttribute(Type packetType)
        {
            this.packetType = packetType;
        }

        public Type packetType;
    }

    public static class PacketHandlerAttributeUtils
    {
        public static PacketHandlerAttribute GetPacketHandlerAttribute(this MethodInfo method)
        {
            PacketHandlerAttribute[] attributes = (PacketHandlerAttribute[])method.GetCustomAttributes(typeof(PacketHandlerAttribute), false);
            if (attributes.Length > 0)
                return attributes[0];

            return null;
        }

        public static PacketHandlerAttribute[] GetPacketHandlerAttributes(this MethodInfo method)
        {
            PacketHandlerAttribute[] attributes = (PacketHandlerAttribute[])method.GetCustomAttributes(typeof(PacketHandlerAttribute), false);

            return attributes;
        }
    }

    public struct PacketHandlerData
    {
        public Type packetType;
        public ArrayList<(MethodInfo, object)> packetHandlers;

        public PacketHandlerData(Type packetType)
        {
            this.packetType = packetType;
            this.packetHandlers = new ArrayList<(MethodInfo, object)>();
        }
    }
}
