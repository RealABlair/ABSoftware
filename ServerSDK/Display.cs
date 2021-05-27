using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static ABSoftware.ServerSDK.Windows;

namespace ABSoftware.ServerSDK
{
    public class Display
    {
        public bool isFocused { get { return (GetConsoleWindow() == GetForegroundWindow()); } }

        public void Init()
        {
            var iStdOut = GetStdHandle(STD_OUTPUT_HANDLE); GetConsoleMode(iStdOut, out var outConsoleMode); SetConsoleMode(iStdOut, outConsoleMode | ENABLE_VIRTUAL_TERMINAL_PROCESSING);
        }

        public string Colorize(string text, Color color, ColorType type)
        {
            return $"\x1b[{(byte)type};2;{color.R};{color.G};{color.B}m{text}\x1b[0m";
        }

        public string SetDefaultColor()
        {
            return $"\x1b[0m";
        }

        public void Println(string text, Color color)
        {
            Console.WriteLine(Colorize(text, color, ColorType.Foreground));
        }

        public void Print(string text, Color color)
        {
            Console.Write(Colorize(text, color, ColorType.Foreground));
        }

        public enum ColorType : byte
        {
            Foreground = 38,
            Background = 48
        }
    }
}
