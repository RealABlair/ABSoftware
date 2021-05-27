using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware.ServerSDK
{
    public class Randomizer
    {
        static Random rnd = new Random();

        public static byte RandomByte(byte min, byte max) => (byte)rnd.Next(min, max);

        public static int RandomInt(int min, int max) => rnd.Next(min, max);
    }
}
