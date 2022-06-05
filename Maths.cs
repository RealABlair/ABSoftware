using System;

namespace ABSoftware
{
    public class Maths
    {
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

        public static float InvLerp(float start, float end, float val)
        {
            return (val - start) / (end - start);
        }

        public static void AddBit(ref int value, int bit)
        {
            value |= 1 << bit;
        }

        public static void RemoveBit(ref int value, int bit)
        {
            value &= ~(1 << bit);
        }

        public static bool HasBit(int value, int bit)
        {
            return (value & 1 << bit) != 0;
        }

        public static float WrapAngleTo180(float value)
        {
            value = value % 360.0f;

            if (value >= 180.0f)
            {
                value -= 360.0f;
            }

            if (value < -180.0f)
            {
                value += 360.0f;
            }

            return value;
        }

        public static double WrapAngleTo180(double value)
        {
            value = value % 360.0;

            if (value >= 180.0)
            {
                value -= 360.0;
            }

            if (value < -180.0)
            {
                value += 360.0;
            }

            return value;
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
