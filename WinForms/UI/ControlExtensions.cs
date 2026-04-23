using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace ABSoftware.WinForms.UI
{
    public static class ControlExtensions
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

        [StructLayout(LayoutKind.Sequential)]
        public struct FLASHWINFO
        {
            public UInt32 cbSize;
            public IntPtr hwnd;
            public UInt32 dwFlags;
            public UInt32 uCount;
            public UInt32 dwTimeout;
        }

        public enum FlashWindow : uint
        {
            /// <summary>
            /// Stop flashing. The system restores the window to its original state.
            /// </summary>    
            FLASHW_STOP = 0,

            /// <summary>
            /// Flash the window caption
            /// </summary>
            FLASHW_CAPTION = 1,

            /// <summary>
            /// Flash the taskbar button.
            /// </summary>
            FLASHW_TRAY = 2,

            /// <summary>
            /// Flash both the window caption and taskbar button.
            /// This is equivalent to setting the FLASHW_CAPTION | FLASHW_TRAY flags.
            /// </summary>
            FLASHW_ALL = 3,

            /// <summary>
            /// Flash continuously, until the FLASHW_STOP flag is set.
            /// </summary>
            FLASHW_TIMER = 4,

            /// <summary>
            /// Flash continuously until the window comes to the foreground.
            /// </summary>
            FLASHW_TIMERNOFG = 12
        }

        public static bool FlashWindowEx(this Form form)
        {
            FLASHWINFO fInfo = new FLASHWINFO();

            fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo));
            fInfo.hwnd = form.Handle;
            fInfo.dwFlags = (uint)(FlashWindow.FLASHW_TRAY | FlashWindow.FLASHW_TIMERNOFG);
            fInfo.uCount = UInt32.MaxValue;
            fInfo.dwTimeout = 0;

            return FlashWindowEx(ref fInfo);
        }

        public static bool FlashWindowEx(this Form form, FlashWindow flash)
        {
            FLASHWINFO fInfo = new FLASHWINFO();

            fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo));
            fInfo.hwnd = form.Handle;
            fInfo.dwFlags = (uint)flash;
            fInfo.uCount = UInt32.MaxValue;
            fInfo.dwTimeout = 0;

            return FlashWindowEx(ref fInfo);
        }

        static Dictionary<string, Thread> THREADS_DICTIONARY = new Dictionary<string, Thread>();

        public static void Pulse(this Control control, Color pulseColor, float speed = 1f)
        {
            if (THREADS_DICTIONARY.ContainsKey($"pulse_{control.Name}"))
                return;

            Thread t = new Thread(() =>
            {
                float elapsedTime = 0f;
                DateTime startTime = DateTime.Now;
                bool active = true;
                Color startColor = control.BackColor;
                EventHandler handler = null;
                handler = (sender, e) =>
                {
                    active = false;
                };

                control.MouseEnter += handler;

                while (active)
                {
                    DateTime now = DateTime.Now;
                    elapsedTime += (float)((now - startTime).TotalSeconds);
                    float timer = (float)((Math.Sin(elapsedTime * speed - 1.570797) + 1d) / 2d);
                    Color lerp = startColor.Lerp(pulseColor, timer);
                    control.BackColor = lerp;
                    startTime = now;
                }

                control.MouseEnter -= handler;
                control.BackColor = startColor;
                THREADS_DICTIONARY.Remove($"pulse_{control.Name}");

            })
            { IsBackground = true };
            THREADS_DICTIONARY[$"pulse_{control.Name}"] = t;
            t.Start();
        }

        public static void Pulse(this ButtonBase control, Color pulseColor, float speed = 1f)
        {
            if (THREADS_DICTIONARY.ContainsKey($"pulse_buttonbase_{control.Name}"))
                return;
            Thread t = new Thread(() =>
            {
                bool useVisualStyle = control.UseVisualStyleBackColor;
                float elapsedTime = 0f;
                DateTime startTime = DateTime.Now;
                bool active = true;
                Color startColor = control.BackColor;
                EventHandler handler = null;
                handler = (sender, e) =>
                {
                    active = false;
                };

                control.MouseEnter += handler;

                while (active)
                {
                    DateTime now = DateTime.Now;
                    elapsedTime += (float)((now - startTime).TotalSeconds);
                    float timer = (float)((Math.Sin(elapsedTime * speed - 1.570797) + 1d) / 2d);
                    Color lerp = startColor.Lerp(pulseColor, timer);
                    control.BackColor = lerp;
                    startTime = now;
                }

                control.MouseEnter -= handler;
                control.BackColor = startColor;
                control.UseVisualStyleBackColor = useVisualStyle;
                THREADS_DICTIONARY.Remove($"pulse_buttonbase_{control.Name}");
            })
            { IsBackground = true };
            THREADS_DICTIONARY[$"pulse_buttonbase_{control.Name}"] = t;
            t.Start();
        }

        public static Color Lerp(this Color from, Color to, float t)
        {
            byte lR = (byte)((1f - t) * from.R + t * to.R);
            byte lG = (byte)((1f - t) * from.G + t * to.G);
            byte lB = (byte)((1f - t) * from.B + t * to.B);
            byte lA = (byte)((1f - t) * from.A + t * to.A);

            return Color.FromArgb(lA, lR, lG, lB);
        }
    }
}