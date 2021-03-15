using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ABSoftware.Engine.Structs;

namespace ABSoftware.Engine
{
    public abstract class ConsoleEngine
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetConsoleWindow();

        public static string VOID_PIXEL = ".";


        private Thread engine_thread;
        private bool isRunning = false;
        private DateTime lastUpdate;

        public bool needToRender = true;
        public bool isFocused = false;
        private IntPtr handle = IntPtr.Zero;

        public Screen screen = new Screen(0, 0);
        public Graphics graphics = new Graphics();

        public Random random = new Random();

        // width <= 240 //
        // height <= 63 //
       /*Console.CursorVisible = cursorEnabled;
        *Console.SetWindowSize(width, height);
        *Console.SetBufferSize(width, height);
        */


        public ConsoleEngine()
        {
            isRunning = true;
            engine_thread = new Thread(new ThreadStart(EngineLoop));
            engine_thread.Start();
        }

        public void StartEngine()
        {
            if(!isRunning)
            {
                isRunning = true;
                engine_thread = new Thread(new ThreadStart(EngineLoop));
                engine_thread.Start();
            }
        }

        public void StopEngine()
        {
            if(isRunning)
            {
                isRunning = false;
                engine_thread.Abort();
                engine_thread = null;
            }
        }

        private void CheckFocus()
        {
            if(handle == GetForegroundWindow())
            {
                isFocused = true;
            }
            else
            {
                isFocused = false;
            }
        }

        public void EngineLoop()
        {
            Start();
            handle = GetConsoleWindow();
            while (isRunning)
            {
                //Calling the methods
                CheckFocus();
                Update((DateTime.Now - lastUpdate).TotalMilliseconds);
                Render();
            }
        }

        public float Lerp(float start, float end, float t)
        {
            return start * (1 - t) + end * t;
        }

        public void SetPixel(Pixel p)
        {
            graphics.Add(p);
        }

        public void RemovePixel(Vector2 v)
        {
            SetPixel(new Pixel(VOID_PIXEL, v, ConsoleColor.White, ConsoleColor.Black));
        }

        public void DrawLine(Vector2 startPoint, Vector2 endPoint, Pixel pixel)
        {
            //Изменения координат
            int dx = (endPoint.x > startPoint.x) ? (endPoint.x - startPoint.x) : (startPoint.x - endPoint.x);
            int dy = (endPoint.y > startPoint.y) ? (endPoint.y - startPoint.y) : (startPoint.y - endPoint.y);
            //Направление приращения
            int sx = (endPoint.x >= startPoint.x) ? (1) : (-1);
            int sy = (endPoint.y >= startPoint.y) ? (1) : (-1);

            if (dy < dx)
            {
                int d = (dy << 1) - dx;
                int d1 = dy << 1;
                int d2 = (dy - dx) << 1;
                SetPixel(new Pixel(pixel.pixel, new Vector2(startPoint.x, startPoint.y), pixel.fColor, pixel.bColor));
                int x = startPoint.x + sx;
                int y = startPoint.y;
                for (int i = 1; i <= dx; i++)
                {
                    if (d > 0)
                    {
                        d += d2;
                        y += sy;
                    }
                    else
                        d += d1;
                    SetPixel(new Pixel(pixel.pixel, new Vector2(x, y), pixel.fColor, pixel.bColor));
                    x += sx;
                }
            }
            else
            {
                int d = (dx << 1) - dy;
                int d1 = dx << 1;
                int d2 = (dx - dy) << 1;
                SetPixel(new Pixel(pixel.pixel, new Vector2(startPoint.x, startPoint.y), pixel.fColor, pixel.bColor));
                int x = startPoint.x;
                int y = startPoint.y + sy;
                for (int i = 1; i <= dy; i++)
                {
                    if (d > 0)
                    {
                        d += d2;
                        x += sx;
                    }
                    else
                        d += d1;
                    SetPixel(new Pixel(pixel.pixel, new Vector2(x, y), pixel.fColor, pixel.bColor));
                    y += sy;
                }
            }
        }

        public void DrawRectangle(Vector2 startPoint, Vector2 endPoint, Pixel pixel)
        {
            DrawLine(new Vector2(startPoint.x, startPoint.y), new Vector2(endPoint.x, startPoint.y), pixel);
            DrawLine(new Vector2(endPoint.x, startPoint.y), new Vector2(endPoint.x, endPoint.y), pixel);
            DrawLine(new Vector2(endPoint.x, endPoint.y), new Vector2(startPoint.x, endPoint.y), pixel);
            DrawLine(new Vector2(startPoint.x, endPoint.y), new Vector2(startPoint.x, startPoint.y), pixel);
        }

        public void DrawSolidRectangle(Vector2 startPoint, Vector2 endPoint, Pixel pixel)
        {
            for(int x = startPoint.x; x < endPoint.x; x++)
            {
                for (int y = startPoint.y; y < endPoint.y; y++)
                {
                    SetPixel(new Pixel(pixel.pixel, new Vector2(x, y), pixel.fColor, pixel.bColor));
                }
            }
        }

        public void DrawTriangle(Vector2 firstPoint, Vector2 secondPoint, Vector2 thirdPoint, Pixel pixel)
        {
            DrawLine(firstPoint, secondPoint, pixel);
            DrawLine(secondPoint, thirdPoint, pixel);
            DrawLine(thirdPoint, firstPoint, pixel);
        }

        public void DrawCurve(Vector2[] array, Pixel p)
        {
            for(int i = 0; i < array.Length - 1; i++)
            {
                DrawLine(array[i], array[i + 1], p);
            }
        }

        public abstract void Start(); 
        public abstract void Update(double elapsedTime);
        public abstract void Render();
    }
}
