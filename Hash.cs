using System;

namespace ABSoftware
{
    public class Hash
    {
        public static int BLOCK_SIZE = 64;
        public static int BlockRange { get { return BLOCK_SIZE / 4; } }
        
        public static string GetHash(string input)
        {
            uint A = 0x00010203;
            uint B = 0x04050607;
            uint C = 0x08090A0B;
            uint D = 0x0C0D0E0F;

            int inputLength = RoundToClosest(input.Length, BLOCK_SIZE);
            input = input.PadRight(inputLength, '\0');

            char[] buffer;

            for(int i = 0; i < inputLength / BLOCK_SIZE; i++)
            {
                buffer = input.Substring(i * BLOCK_SIZE, BLOCK_SIZE).ToCharArray();

                for(int c = 0; c < buffer.Length; c++)
                {
                    uint F = 0;
                    if(c >= 0 && c <= BlockRange)
                    {
                        F = (uint)(buffer[c] & RotateLeft(buffer[c + 1], 6)) ^ 1;
                    }
                    else if(c > BlockRange && c <= BlockRange * 2)
                    {
                        F = (uint)(buffer[c] & buffer[c - 1] & ~buffer[c + 1]) ^ 1;
                    }
                    else if (c > BlockRange * 2 && c <= BlockRange * 3)
                    {
                        F = (uint)(~buffer[c] ^ buffer[c - 1]) ^ 1;
                    }
                    else if (c > BlockRange * 3 && c < BLOCK_SIZE)
                    {   
                        F = (uint)(buffer[c] & ~D ^ 1 << 31);
                    }
                    F = RotateLeft(F + A + buffer[c], 3);
                    A = RotateLeft(D, 3);
                    D = RotateLeft(C, 7);
                    C = RotateLeft(B, 1);
                    B = B + RotateLeft(F, 7);
                }
            }

            return A.ToString("x8") + B.ToString("x8") + C.ToString("x8") + D.ToString("x8");
        }

        public static string GetHash(byte[] input)
        {
            uint A = 0x00010203;
            uint B = 0x04050607;
            uint C = 0x08090A0B;
            uint D = 0x0C0D0E0F;

            byte[] data;

            int inputLength = RoundToClosest(input.Length, BLOCK_SIZE);
            data = new byte[inputLength];
            Array.Copy(input, 0, data, 0, input.Length);


            byte[] buffer = new byte[BLOCK_SIZE];

            for (int i = 0; i < inputLength / BLOCK_SIZE; i++)
            {
                Array.Copy(data, i * BLOCK_SIZE, buffer, 0, BLOCK_SIZE);

                for (int c = 0; c < buffer.Length; c++)
                {
                    uint F = 0;
                    if (c >= 0 && c <= BlockRange)
                    {
                        F = (uint)(buffer[c] & RotateLeft(buffer[c + 1], 4)) ^ 1;
                    }
                    else if (c > BlockRange && c <= BlockRange * 2)
                    {
                        F = (uint)(buffer[c] & buffer[c - 1] & ~buffer[c + 1]) ^ 1;
                    }
                    else if (c > BlockRange * 2 && c <= BlockRange * 3)
                    {
                        F = (uint)(~buffer[c] ^ buffer[c - 1]) ^ 1;
                    }
                    else if (c > BlockRange * 3 && c < BLOCK_SIZE)
                    {
                        F = (uint)(buffer[c] & ~D ^ 1 << 31);
                    }
                    F = RotateLeft(F + A + buffer[c], 3);
                    A = RotateLeft(D, 3);
                    D = RotateLeft(C, 7);
                    C = RotateLeft(B, 1);
                    B = B + RotateLeft(F, 7);
                }
            }

            return A.ToString("x8") + B.ToString("x8") + C.ToString("x8") + D.ToString("x8");
        }

        private static uint RotateLeft(uint x, byte n)
        {
            return (uint)((x << n) | (x >> (32 - n)));
        }

        private static uint RotateRight(uint x, byte n)
        {
            return (uint)((x >> n) | (x << (32 - n)));
        }

        private static int RoundToClosest(int value, int divisible)
        {
            return divisible * (int)Math.Ceiling(value / (float)divisible);
        }
    }
}
