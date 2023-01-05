using System;

namespace ABSoftware
{
    public class ABRandom
    {
        public uint Seed { get; set; }

        public ABRandom()
        {
            this.Seed = (uint)DateTime.Now.Ticks;
        }

        public ABRandom(uint Seed)
        {
            this.Seed = Seed;
        }

        private uint GetRandom()
        {
            this.Seed += 0xA5F23012;
            ulong buffer;
            buffer = (ulong)this.Seed * 0x15FABDA0;
            uint block1 = (uint)((buffer >> 32) ^ buffer);
            buffer = (ulong)block1 * 0x09BAFD01;
            uint block2 = (uint)((buffer >> 32) ^ buffer);
            return block2;
        }

        public int GetRandomInt(int min = int.MinValue, int max = int.MaxValue)
        {
            return (int)(GetRandom() % (max - min)) + min;
        }

        public uint GetRandomUInt(uint min = 0, uint max = uint.MaxValue)
        {
            return (GetRandom() % (max - min)) + min;
        }

        public double GetRandomDouble()
        {
            return (double)(GetRandom() / (double)uint.MaxValue);
        }

        public bool GetRandomBool()
        {
            return (GetRandom() >> 31) != 0;
        }

        public bool GetRandomBool(float chance)
        {
            return !(GetRandomDouble() * 100 > chance);
        }

        public void RandomizeBytes(byte[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = (byte)(GetRandom() % 256);
            }
        }
    }
}
