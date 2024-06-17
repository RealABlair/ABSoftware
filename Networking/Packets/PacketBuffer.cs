using System;

namespace ABSoftware.Networking.Packets
{
    public class PacketBuffer : ByteBuilder
    {
        public int position = 0;

        public PacketBuffer() : base()
        {

        }

        public PacketBuffer(byte[] rawData) : base(rawData)
        {

        }

        public void Write<T>(T value)
        {
            Type type = typeof(T);

            if (type == typeof(sbyte)) this.Append(new byte[1] { (byte)(sbyte)(object)value });
            if (type == typeof(byte)) this.Append(new byte[1] { (byte)(object)value });
            if (type == typeof(short)) this.Append(new byte[2] { (byte)((short)(object)value & 0xFF), (byte)((short)(object)value >> 8 & 0xFF) });
            if (type == typeof(ushort)) this.Append(new byte[2] { (byte)((ushort)(object)value & 0xFF), (byte)((ushort)(object)value >> 8 & 0xFF) });
            if (type == typeof(int)) this.Append(new byte[4] { (byte)((int)(object)value & 0xFF), (byte)((int)(object)value >> 8 & 0xFF), (byte)((int)(object)value >> 16 & 0xFF), (byte)((int)(object)value >> 24 & 0xFF) });
            if (type == typeof(uint)) this.Append(new byte[4] { (byte)((uint)(object)value & 0xFF), (byte)((uint)(object)value >> 8 & 0xFF), (byte)((uint)(object)value >> 16 & 0xFF), (byte)((uint)(object)value >> 24 & 0xFF) });
            if (type == typeof(long)) this.Append(new byte[8] { (byte)((long)(object)value & 0xFF), (byte)((long)(object)value >> 8 & 0xFF), (byte)((long)(object)value >> 16 & 0xFF), (byte)((long)(object)value >> 24 & 0xFF), (byte)((long)(object)value >> 32 & 0xFF), (byte)((long)(object)value >> 40 & 0xFF), (byte)((long)(object)value >> 48 & 0xFF), (byte)((long)(object)value >> 56 & 0xFF) });
            if (type == typeof(ulong)) this.Append(new byte[8] { (byte)((ulong)(object)value & 0xFF), (byte)((ulong)(object)value >> 8 & 0xFF), (byte)((ulong)(object)value >> 16 & 0xFF), (byte)((ulong)(object)value >> 24 & 0xFF), (byte)((ulong)(object)value >> 32 & 0xFF), (byte)((ulong)(object)value >> 40 & 0xFF), (byte)((ulong)(object)value >> 48 & 0xFF), (byte)((ulong)(object)value >> 56 & 0xFF) });

            if (type == typeof(float)) this.Append(BitConverter.GetBytes((float)(object)value));
            if (type == typeof(double)) this.Append(BitConverter.GetBytes((double)(object)value));
        }

        public void WriteString(string text)
        {
            byte[] utf8 = System.Text.Encoding.UTF8.GetBytes(text);
            Write(utf8.Length);
            Write(utf8);
        }

        public void Write(byte[] data)
        {
            this.Append(data);
        }

        public T Read<T>()
        {
            TypeData typeData = GetTypeData<T>();

            byte[] data = this.GetRange(position, typeData.size);

            position += typeData.size;

            if (typeData.type == typeof(sbyte)) return (T)(object)(sbyte)data[0];
            if (typeData.type == typeof(byte)) return (T)(object)data[0];
            if (typeData.type == typeof(short)) return (T)(object)(short)(data[1] << 8 | data[0]);
            if (typeData.type == typeof(ushort)) return (T)(object)(ushort)(data[1] << 8 | data[0]);
            if (typeData.type == typeof(int)) return (T)(object)(int)(data[3] << 24 | data[2] << 16 | data[1] << 8 | data[0]);
            if (typeData.type == typeof(uint)) return (T)(object)(uint)(data[3] << 24 | data[2] << 16 | data[1] << 8 | data[0]);
            if (typeData.type == typeof(long)) return (T)(object)(long)((long)data[7] << 56 | (long)data[6] << 48 | (long)data[5] << 40 | (long)data[4] << 32 | (long)data[3] << 24 | (long)data[2] << 16 | (long)data[1] << 8 | (long)data[0]);
            if (typeData.type == typeof(ulong)) return (T)(object)(ulong)((long)data[7] << 56 | (long)data[6] << 48 | (long)data[5] << 40 | (long)data[4] << 32 | (long)data[3] << 24 | (long)data[2] << 16 | (long)data[1] << 8 | (long)data[0]);
                
            if (typeData.type == typeof(float)) return (T)(object)BitConverter.ToSingle(data, 0);
            if (typeData.type == typeof(double)) return (T)(object)BitConverter.ToDouble(data, 0);

            return (T)(object)0;
        }

        public string ReadString()
        {
            int size = Read<int>();
            byte[] utf8 = Read(size);

            return System.Text.Encoding.UTF8.GetString(utf8);
        }

        public byte[] Read(int count)
        {
            byte[] data = this.GetRange(position, count);
            position += count;
            return data;
        }

        public TypeData GetTypeData<T>()
        {
            Type type = typeof(T);

            if (type == typeof(sbyte)) return new TypeData(type, 1);
            if (type == typeof(byte)) return new TypeData(type, 1);
            if (type == typeof(short)) return new TypeData(type, 2);
            if (type == typeof(ushort)) return new TypeData(type, 2);
            if (type == typeof(int)) return new TypeData(type, 4);
            if (type == typeof(uint)) return new TypeData(type, 4);
            if (type == typeof(long)) return new TypeData(type, 8);
            if (type == typeof(ulong)) return new TypeData(type, 8);

            if (type == typeof(float)) return new TypeData(type, 4);
            if (type == typeof(double)) return new TypeData(type, 4);

            return new TypeData(type, 0);
        }

        public struct TypeData
        {
            public TypeData(Type type, int size)
            {
                this.type = type;
                this.size = size;
            }

            public Type type;
            public int size;
        }
    }
}
