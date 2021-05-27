using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware.ServerSDK
{
    public class Color
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }


        public static Color FromRGB(byte r, byte g, byte b)
        {
            Color p = new Color();
            p.R = r;
            p.G = g;
            p.B = b;
            return p;
        }

        public static Color FromHSV(float hue, float saturation, float value)
        {
            int H = (int)Maths.Lerp(0, 360, hue);

            float C = value * saturation;
            float X = C * (1 - (H / 60) % 2 - 1);
            float m = value - C;

            float uR = (0 <= H && H < 60) ? C : (60 <= H && H < 120) ? X : (120 <= H && H < 180) ? 0f : (180 <= H && H < 240) ? 0f : (240 <= H && H < 300) ? X : C;
            float uG = (0 <= H && H < 60) ? X : (60 <= H && H < 120) ? C : (120 <= H && H < 180) ? C : (180 <= H && H < 240) ? X : (240 <= H && H < 300) ? C : X;
            float uB = (0 <= H && H < 60) ? 0f : (60 <= H && H < 120) ? 0f : (120 <= H && H < 180) ? X : (180 <= H && H < 240) ? C : (240 <= H && H < 300) ? C : X;

            Color p = new Color();
            p.R = (byte)((uR + m) * 255);
            p.G = (byte)((uG + m) * 255);
            p.B = (byte)((uB + m) * 255);
            return p;
        }

        public static Color FromHSV(int hue, float saturation, float value)
        {
            float C = value * saturation;
            float X = C * (1 - (hue / 60) % 2 - 1);
            float m = value - C;

            float uR = (0 <= hue && hue < 60) ? C : (60 <= hue && hue < 120) ? X : (120 <= hue && hue < 180) ? 0f : (180 <= hue && hue < 240) ? 0f : (240 <= hue && hue < 300) ? X : C;
            float uG = (0 <= hue && hue < 60) ? X : (60 <= hue && hue < 120) ? C : (120 <= hue && hue < 180) ? C : (180 <= hue && hue < 240) ? X : (240 <= hue && hue < 300) ? C : X;
            float uB = (0 <= hue && hue < 60) ? 0f : (60 <= hue && hue < 120) ? 0f : (120 <= hue && hue < 180) ? X : (180 <= hue && hue < 240) ? C : (240 <= hue && hue < 300) ? C : X;

            Color p = new Color();
            p.R = (byte)((uR + m) * 255);
            p.G = (byte)((uG + m) * 255);
            p.B = (byte)((uB + m) * 255);
            return p;
        }

        public static Color InvertColor(Color pix)
        {
            Color p = new Color();
            p.R = (byte)(255 - pix.R);
            p.G = (byte)(255 - pix.G);
            p.B = (byte)(255 - pix.B);
            return p;
        }

        public static Color Random => FromRGB(Randomizer.RandomByte(0, 255), Randomizer.RandomByte(0, 255), Randomizer.RandomByte(0, 255));

        public static Color Red => FromRGB(255, 0, 0);
        public static Color Green => FromRGB(0, 255, 0);
        public static Color Blue => FromRGB(0, 0, 255);
        public static Color Cyan => FromRGB(0, 255, 255);
        public static Color DarkCyan => FromRGB(0, 55, 55);
        public static Color DarkRed => FromRGB(55, 0, 0);
        public static Color DarkGreen => FromRGB(0, 55, 0);
        public static Color DarkBlue => FromRGB(0, 0, 55);
        public static Color Sky => FromRGB(135, 206, 235);
        public static Color Amber => FromRGB(255, 191, 0);
        public static Color Gold => FromRGB(255, 213, 0);
        public static Color Dirt => FromRGB(80, 55, 21);
        public static Color Magenta => FromRGB(255, 0, 255);
        public static Color ProcessMagenta => FromRGB(255, 0, 144);
        public static Color Yellow => FromRGB(255, 255, 0);
        public static Color Black => FromRGB(0, 0, 0);
        public static Color White => FromRGB(255, 255, 255);
        public static Color LightGray => FromRGB(211, 211, 211);
        public static Color Gray => FromRGB(169, 169, 169);
        public static Color DarkGray => FromRGB(128, 128, 128);
    }
}
