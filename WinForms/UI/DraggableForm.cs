using System;
using System.Windows.Forms;

namespace ABSoftware.UI
{
    public class DraggableForm : Form
    {
        public DraggableForm() : base()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            InitializeComponent();
        }

        protected override void OnMouseDown(MouseEventArgs e)
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