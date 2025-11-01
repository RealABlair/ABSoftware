using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;

namespace ABSoftware
{
    public class OverlayForm : Form
    {
        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(Keys vKey);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hwnd, out RECT IpRect);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindow(IntPtr hwnd, int wCmd);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        static extern IntPtr WindowFromPoint(Point p);

        public const int GW_HWNDFIRST = 0;
        public const int GW_HWNDLAST = 1;
        public const int GW_HWNDNEXT = 2;
        public const int GW_HWNDPREV = 3;

        public struct RECT
        {
            public int left, top, right, bottom;
        }

        private Color transparencyColor;
        public IntPtr hwnd { get; private set; }

        public bool overlayIsActive = false;

        public RECT windowRect;

        public BufferedGraphics graphics;

        private Thread windowUpdateThread;

        public bool isDrawing = false;
        public bool isTargetWindowVisible = false;

        public int OverlayWidth { get { return windowRect.right - windowRect.left; } }
        public int OverlayHeight { get { return windowRect.bottom - windowRect.top; } }

        public OverlayForm(IntPtr hWnd, Color transparencyColor)
        {
            this.transparencyColor = transparencyColor;

            this.hwnd = hWnd;
            this.Load += OverlayForm_Load;
            this.FormClosing += OverlayForm_FormClosing;
        }

        private void OverlayForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.overlayIsActive = false;
            this.windowUpdateThread.Abort();
            this.windowUpdateThread = null;
        }

        private void OverlayForm_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            this.BackColor = transparencyColor;
            this.TransparencyKey = transparencyColor;
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.Text = "overlay";

            int prevStyle = GetWindowLong(this.Handle, -20);
            SetWindowLong(this.Handle, -20, prevStyle | 0x80000 | 0x20);

            GetWindowRect(hwnd, out windowRect);
            this.Size = new Size(windowRect.right - windowRect.left, windowRect.bottom - windowRect.top);
            this.Left = windowRect.left;
            this.Top = windowRect.top;

            BufferedGraphicsContext graphicsContext = new BufferedGraphicsContext();
            this.graphics = graphicsContext.Allocate(this.CreateGraphics(), ClientRectangle);
            this.graphics.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            this.graphics.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

            overlayIsActive = true;

            windowUpdateThread = new Thread(UpdateWindowState);
            windowUpdateThread.Start();
        }

        void UpdateWindowState()
        {
            while(this.overlayIsActive)
            {
                GetWindowRect(this.hwnd, out windowRect);
                this.Size = new Size(windowRect.right - windowRect.left, windowRect.bottom - windowRect.top);
                this.Left = windowRect.left;
                this.Top = windowRect.top;
                this.isTargetWindowVisible = WindowFromPoint(new Point(windowRect.left + (windowRect.right - windowRect.left) / 2, windowRect.top + (windowRect.bottom - windowRect.top) / 2)) == this.hwnd;

                Thread.Sleep(10);
            }
        }

        public bool BeginDrawing(out Graphics graphics, bool clearCanvas = true)
        {
            if(!isDrawing)
            {
                isDrawing = true;
                graphics = this.graphics.Graphics;

                if (clearCanvas)
                    ClearWithTransparency(graphics);

                return true;
            }

            graphics = null;
            return false;
        }

        public bool EndDrawing()
        {
            if(isDrawing)
            {
                isDrawing = false;
                graphics.Render();
                return true;
            }

            return false;
        }

        public void ClearWithTransparency(Graphics graphics)
        {
            Rectangle rect = ClientRectangle;
            rect.Offset(-1, -1);
            rect.Inflate(1, 1);
            graphics.FillRectangle(new SolidBrush(transparencyColor), rect);
        }
    }
}
