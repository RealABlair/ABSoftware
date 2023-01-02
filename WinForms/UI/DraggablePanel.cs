using System;
using System.Windows.Forms;

namespace ABSoftware.UI
{
    public class DraggablePanel : Panel
    {

        public DraggablePanel() : base()
        {
            this.MouseDown += Drag;
        }

        private void Drag(object sender, MouseEventArgs e)
        {
            this.Capture = false;
            Message m = Message.Create(FindForm().Handle, 0xa1, new IntPtr(2), IntPtr.Zero);
            WndProc(ref m);
        }
    }
}
