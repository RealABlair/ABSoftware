using System;

namespace ABSoftware
{
    public class ABHA256 //Alexander Blair Hashing Algorithm
    {
        public static string Hash(byte[] data)
        {
            uint[] buffers = new uint[8]
            {
                0xAABBCCDD,
                0x11223344,
                0x55667788,
                0x99AABBCC,
                0xDDEEFF00,
                0x0F1E2D3C,
                0x4B5A6978,
                0x89ABCDEF
            };

            int rounds = 8;

            for (int r = 0; r < rounds; r++)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    byte currentData = data[i];

                    for (int j = 0; j < buffers.Length; j++)
                    {
                        buffers[j] = (buffers[j] ^ (uint)(currentData << (j % 4 * 8))) + MoveBitsLeftWithWrap(buffers[j], (j + r) % 32);
                        buffers[j] = MoveBitsRightWithWrap(buffers[j], (j + r + 1) % 32);

                        buffers[j] = buffers[j] + (uint)(currentData * (j + 1));
                    }
                }
            }

            return $"{buffers[0]:X8}{buffers[1]:X8}{buffers[2]:X8}{buffers[3]:X8}{buffers[4]:X8}{buffers[5]:X8}{buffers[6]:X8}{buffers[7]:X8}";
        }

        static uint MoveBitsRightWithWrap(uint value, int bitsCount)
        {
            return (uint)((value >> bitsCount) | (value << (32 - bitsCount)));
        }

        static uint MoveBitsLeftWithWrap(uint value, int bitsCount)
        {
            return (uint)((value << bitsCount) | (value >> (32 - bitsCount)));
        }
    }
}
