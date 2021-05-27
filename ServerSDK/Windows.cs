using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware.ServerSDK
{
    public class Windows
    {
        #region Imports
        [DllImport(KERNEL)]
        public static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport(KERNEL)]
        public static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport(KERNEL, SetLastError = true)]
        public static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport(KERNEL)]
        public static extern IntPtr GetConsoleWindow();

        [DllImport(USER)]
        public static extern IntPtr GetForegroundWindow();



        #endregion

        #region Consts
        public const string KERNEL = "kernel32.dll";
        public const string USER = "user32.dll";

        public const int STD_OUTPUT_HANDLE = -11;
        public const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;
        #endregion
    }
}
