using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware
{
    public class Maths
    {
        public static int PowerNumber(int num, int power)
        {
            for(int i = 1; i < power; i++)
            {
                num *= num;
            }
            return num;
        }

        public static float PowerNumber(float num, int power)
        {
            for (int i = 1; i < power; i++)
            {
                num *= num;
            }
            return num;
        }

        public static double DegreesToRadians(double degrees)
        {
            return degrees / 180 * Math.PI;
        }

        public static double RadiansToDegrees(double radians)
        {
            return radians / Math.PI * 180;
        }
        
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
