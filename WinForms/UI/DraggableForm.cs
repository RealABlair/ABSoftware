using System;
using ABSoftware.UI;
using System.Windows.Forms;

namespace ABSoftware.UI
{
    public class DraggableForm : Form
    {
        public DraggableForm() : base()
        {
            this.MouseDown += Drag;
            this.FormBorderStyle = FormBorderStyle.None;
            InitializeComponent();
        }

        private void Drag(object sender, MouseEventArgs e)
        {
            this.Capture = false;
            Message m = Message.Create(Handle, 0xa1, new IntPtr(2), IntPtr.Zero);
            WndProc(ref m);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(700, 450);
            this.Name = "DraggableForm";
            this.ResumeLayout(true);
        }
    }
}
