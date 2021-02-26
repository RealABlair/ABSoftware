using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEMP.ABSoftware
{
    public class Maths
    {
        public static float Lerp(float start, float end, float t)
        {
            return start * (1 - t) + end * t;
        }

        public class Random
        {
            static System.Random rnd = new System.Random();

            public static float Range(float start, float end)
            {
                return Lerp(start, end, (float)rnd.NextDouble());
            }
        }
    }
}
