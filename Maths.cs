using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware
{
    public class Maths
    {
        public static string ToHex(int number)
        {
            string hex = "";
            double lastDiv = number;
            while ((int)lastDiv != 0)
            {
                double div = lastDiv / 16;
                double rem = lastDiv % 16;
                int result = (int)div;
                lastDiv = result;
                hex += HEX_CHARS((int)(rem));
            }
            char[] array = hex.ToCharArray();
            Array.Reverse(array);
            return new string(array);
        }

        public static string ToHex(long number)
        {
            string hex = "";
            double lastDiv = number;
            while ((int)lastDiv != 0)
            {
                double div = lastDiv / 16;
                double rem = lastDiv % 16;
                int result = (int)div;
                lastDiv = result;
                hex += HEX_CHARS((int)(rem));
            }
            char[] array = hex.ToCharArray();
            Array.Reverse(array);
            return new string(array);
        }

        public static int ToInt(string hex)
        {
            char[] array = hex.ToCharArray();
            Array.Reverse(array);
            int sum = 0;
            for (int i = 0; i < array.Length; i++)
            {
                int byt = INT_FROM_HEX(array[i]);
                sum += byt * PowerNumber(16, i);
            }
            return sum;
        }

        public static long ToLong(string hex)
        {
            char[] array = hex.ToCharArray();
            Array.Reverse(array);
            long sum = 0;
            for (int i = 0; i < array.Length; i++)
            {
                int byt = INT_FROM_HEX(array[i]);
                sum += byt * PowerNumber(16, i);
            }
            return sum;
        }

        public static int PowerNumber(int num, int power)
        {
            if (power == 0)
                return 1;
            for (int i = 1; i < power; i++)
            {
                num *= num;
            }
            return num;
        }

        public static float PowerNumber(float num, int power)
        {
            if (power == 0)
                return 1f;
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
        
        public static float InvLerp(float start, float end, float val)
        {
            return (val / start) / (end - start);
        }

        public class Random
        {
            static System.Random rnd = new System.Random();

            public static float Range(float start, float end)
            {
                return Lerp(start, end, (float)rnd.NextDouble());
            }
        }

        private static char HEX_CHARS(int integer)
        {
            switch (integer)
            {
                case 0:
                    return '0';
                case 1:
                    return '1';
                case 2:
                    return '2';
                case 3:
                    return '3';
                case 4:
                    return '4';
                case 5:
                    return '5';
                case 6:
                    return '6';
                case 7:
                    return '7';
                case 8:
                    return '8';
                case 9:
                    return '9';
                case 10:
                    return 'A';
                case 11:
                    return 'B';
                case 12:
                    return 'C';
                case 13:
                    return 'D';
                case 14:
                    return 'E';
                case 15:
                    return 'F';
            }
            return '0';
        }

        private static int INT_FROM_HEX(char hex)
        {
            switch (hex)
            {
                case '0':
                    return 0;
                case '1':
                    return 1;
                case '2':
                    return 2;
                case '3':
                    return 3;
                case '4':
                    return 4;
                case '5':
                    return 5;
                case '6':
                    return 6;
                case '7':
                    return 7;
                case '8':
                    return 8;
                case '9':
                    return 9;
                case 'A':
                    return 10;
                case 'B':
                    return 11;
                case 'C':
                    return 12;
                case 'D':
                    return 13;
                case 'E':
                    return 14;
                case 'F':
                    return 15;
            }
            return 0;
        }
    }
}
