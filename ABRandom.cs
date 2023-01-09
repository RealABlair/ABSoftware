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

        public float[] Noise1D(int size, int octaveCount, float bias, float[] seed = null)
        {
            float[] output = new float[size];
            if(seed == null)
            {
                seed = new float[size];
                for (int i = 0; i < size; i++)
                    seed[i] = (float)GetRandomDouble();
            }

            for(int i = 0; i < size; i++)
            {
                float noise = 0f, scale = 1f, totalScale = 0f;

                for (int o = 0; o < octaveCount; o++)
                {
                    int pitch = size >> o;
                    int s1 = (i / pitch) * pitch;
                    int s2 = (s1 + pitch) % size;

                    float blend = (float)(i - s1) / (float)pitch;

                    float s = (1f - blend) * seed[s1] + blend * seed[s2];

                    totalScale += scale;
                    noise += s * scale;
                    scale /= bias;
                }
                output[i] = noise / totalScale;
            }

            return output;
        }

        public float[] Noise2D(int width, int height, int octaveCount, float bias, float[] seed = null)
        {
            float[] output = new float[width * height];

            if (seed == null)
            {
                seed = new float[width * height];
                for (int i = 0; i < width * height; i++)
                    seed[i] = (float)GetRandomDouble();
            }

            for (int x = 0; x < width; x++)
                for(int y = 0; y < height; y++)
                {
                    float noise = 0f, scale = 1f, totalScale = 0f;

                    for (int o = 0; o < octaveCount; o++)
                    {
                        int pitch = width >> o;
                        int s1x = (x / pitch) * pitch;
                        int s1y = (y / pitch) * pitch;
                        int s2x = (s1x + pitch) % width;
                        int s2y = (s1y + pitch) % width;

                        float blendX = (float)(x - s1x) / (float)pitch;
                        float blendY = (float)(y - s1y) / (float)pitch;

                        float s1 = (1f - blendX) * seed[s1y * width + s1x] + blendX * seed[s1y * width + s2x];
                        float s2 = (1f - blendX) * seed[s2y * width + s1x] + blendX * seed[s2y * width + s2x];

                        totalScale += scale;
                        noise += (blendY * (s2 - s1) + s1) * scale;
                        scale /= bias;
                    }
                    output[y * width + x] = noise / totalScale;
                }

            return output;
        }
    }
}
