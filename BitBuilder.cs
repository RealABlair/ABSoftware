using System;

namespace ABSoftware
{
    public class BitBuilder
    {
        bool[] array = null;

        public int Size { get => array.Length; }
        public int LastIndex { get => Size - 1; }
        public bool[] Array { get => this.array; }

        public BitBuilder()
        {
            array = new bool[0];
        }

        public BitBuilder(int length)
        {
            array = new bool[length];
        }

        public BitBuilder(int length, bool fillWith)
        {
            array = new bool[length];
            for (int i = 0; i < array.Length; i++)
                array[i] = fillWith;
        }

        public BitBuilder(bool[] values)
        {
            array = values;
        }

        public BitBuilder(byte[] values, bool pushBack = true)
        {
            if(pushBack)
            {
                array = new bool[values.Length * 8];
                for (int i = 0; i < values.Length; i++)
                    for (int j = 7; j >= 0; j--)
                    {
                        array[(values.Length - 1 - i) * 8 + j] = (values[i] & 1 << j) != 0;
                    }
            }
            else
            {
                array = new bool[values.Length * 8];
                for (int i = 0; i < values.Length; i++)
                    for (int j = 0; j < 8; j++)
                    {
                        array[i * 8 + j] = (values[i] & 1 << j) != 0;
                    }
            }
        }

        public bool this[int id]
        {
            get => array[id];
            set => array[id] = value;
        }

        public bool Get(int id)
        {
            return array[id];
        }

        public bool[] GetRange(int startIndex, int length)
        {
            bool[] data = new bool[length];
            Buffer.BlockCopy(this.array, startIndex, data, 0, length);
            return data;
        }

        public sbyte GetSByte(int startIndex, bool reversed = false)
        {
            sbyte value = 0;
            for (int i = 0; i < 8; i++)
                value |= (sbyte)((array[(reversed) ? LastIndex - startIndex - i : startIndex + i] ? 1 : 0) << ((reversed) ? 7 - i : i));
            return value;
        }

        public byte GetByte(int startIndex, bool reversed = false)
        {
            byte value = 0;
            for (int i = 0; i < 8; i++)
                value |= (byte)((array[(reversed) ? LastIndex - startIndex - i : startIndex + i] ? 1 : 0) << ((reversed) ? 7 - i : i));
            return value;
        }

        public short GetShort(int startIndex, bool reversed = false)
        {
            short value = 0;
            for (int i = 0; i < 16; i++)
                value |= (short)((array[(reversed) ? LastIndex - startIndex - i : startIndex + i] ? 1 : 0) << ((reversed) ? 15 - i : i));
            return value;
        }

        public ushort GetUShort(int startIndex, bool reversed = false)
        {
            ushort value = 0;
            for (int i = 0; i < 16; i++)
                value |= (ushort)((array[(reversed) ? LastIndex - startIndex - i : startIndex + i] ? 1 : 0) << ((reversed) ? 15 - i : i));
            return value;
        }

        public int GetInt(int startIndex, bool reversed = false)
        {
            int value = 0;
            for (int i = 0; i < 32; i++)
                value |= ((array[(reversed) ? LastIndex - startIndex - i : startIndex + i] ? 1 : 0) << ((reversed) ? 31 - i : i));
            return value;
        }

        public uint GetUInt(int startIndex, bool reversed = false)
        {
            uint value = 0;
            for (int i = 0; i < 32; i++)
                value |= (uint)((array[(reversed) ? LastIndex - startIndex - i : startIndex + i] ? 1 : 0) << ((reversed) ? 31 - i : i));
            return value;
        }

        public long GetLong(int startIndex, bool reversed = false)
        {
            long value = 0;
            for (int i = 0; i < 64; i++)
                value |= (long)((array[(reversed) ? LastIndex - startIndex - i : startIndex + i] ? 1 : 0) << ((reversed) ? 63 - i : i));
            return value;
        }

        public ulong GetULong(int startIndex, bool reversed = false)
        {
            ulong value = 0;
            for (int i = 0; i < 64; i++)
                value |= (uint)((array[(reversed) ? LastIndex - startIndex - i : startIndex + i] ? 1 : 0) << ((reversed) ? 63 - i : i));
            return value;
        }

        public void Set(int id, bool value)
        {
            array[id] = value;
        }

        public override string ToString()
        {
            string s = "";
            for(int i = 0; i < Size; i++)
            {
                s += (array[i] ? "1" : "0");
            }
            return s;
        }

        public string ToStringReversed()
        {
            string s = "";
            for (int i = Size - 1; i >= 0; i--)
            {
                s += (array[i] ? "1" : "0");
            }
            return s;
        }

        public static string ToString(bool[] array)
        {
            string s = "";
            for (int i = 0; i < array.Length; i++)
            {
                s += (array[i] ? "1" : "0");
            }
            return s;
        }

        public static string ToStringReversed(bool[] array)
        {
            string s = "";
            for (int i = array.Length - 1; i >= 0; i--)
            {
                s += (array[i] ? "1" : "0");
            }
            return s;
        }
    }
}
