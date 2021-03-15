using ABSoftware.Engine.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware.Engine
{
    public class Pixel
    {
        public Vector2 position;
        public string pixel;
        public ConsoleColor fColor;
        public ConsoleColor bColor;

        public Pixel(string pixel, Vector2 position, ConsoleColor fColor, ConsoleColor bColor)
        {
            this.pixel = pixel;
            this.position = position;
            this.fColor = fColor;
            this.bColor = bColor;
        }

        public Pixel(string pixel, ConsoleColor fColor, ConsoleColor bColor)
        {
            this.pixel = pixel;
            this.position = new Vector2(-1, -1);
            this.fColor = fColor;
            this.bColor = bColor;
        }
    }
}
