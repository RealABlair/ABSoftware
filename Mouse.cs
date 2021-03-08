using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Engine2D.Engine
{
    public class Mouse
    {
        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, UIntPtr dwExtraInfo);

        public static void Event(uint Flag, int x, int y, uint data, UIntPtr ExtraInfo)
        {
            
        }

        public struct Flags
        {
            public const uint MOUSEEVENTF_ABSOLUTE = 0x8000;
            public const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
            public const uint MOUSEEVENTF_LEFTUP = 0x0004;
            public const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
            public const uint MOUSEEVENTF_MIDDLEUP = 0x0040;
            public const uint MOUSEEVENTF_MOVE = 0x0001;
            public const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
            public const uint MOUSEEVENTF_RIGHTUP = 0x0010;
            public const uint MOUSEEVENTF_XDOWN = 0x0080;
            public const uint MOUSEEVENTF_XUP = 0x0100;
            public const uint MOUSEEVENTF_WHEEL = 0x0800;
            public const uint MOUSEEVENTF_HWHEEL = 0x01000;
        }
    }
}
