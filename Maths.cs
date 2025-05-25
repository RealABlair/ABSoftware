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

        public static float RoundTo(float min, float max, float value)
        {
            value %= max;

            if (value < min)
                value += max;

            return value;
        }

        public static float RoundToNew(float min, float max, float value)
        {
            if (value > max)
                value = min + (value % max);
            if (value < min)
                if (min == 0f)
                    value = max + value;
                else
                    value = max + (value % min);

            return value;
        }

        public static float SmoothApproach(float value, float target, float smoothness)
        {
            return value + (target - value) * smoothness;
        }

        public static float Clamp(float value, float min, float max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;

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
