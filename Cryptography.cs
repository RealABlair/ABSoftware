using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware
{
    public class Cryptography
    {
        public static string Hash(string text)
        {
            ulong hash = 0;
            for (var i = 0; i < text.Length; i++)
            {
                hash += (ulong)(text[i] + 1) * (ulong)(Math.Sqrt(hash) + 9);
            }
            hash += (ulong)text.Length;
            if (hash < ulong.MaxValue / 10)
                hash *= hash / (ulong)text.Length;
            return hash.ToString("X16");
        }
    }
}
