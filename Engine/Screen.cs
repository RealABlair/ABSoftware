using ABSoftware.Engine.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware.Engine
{
    public class Screen
    {
        public Pixel[,] matrix = new Pixel[0, 0];

        public Screen(int width, int height)
        {
            matrix = new Pixel[width, height];
            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    matrix[x, y] = new Pixel(ConsoleEngine.VOID_PIXEL, new Vector2(x, y), ConsoleColor.Gray, ConsoleColor.Black);
                }
            }
        }

        public int GetWidth()
        {
            return this.matrix.GetLength(0) - 1;
        }

        public int GetHeight()
        {
            return this.matrix.GetLength(1) - 1;
        }
    }
}
