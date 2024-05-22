using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace ABSoftware.UI
{
    public class DraggablePanel : Panel
    {
        [Category("ABControls"), Description("Sets visibility flag and hides the control from designer"), RefreshProperties(RefreshProperties.Repaint)]
        public bool Hidden { get { return !Visible; } set { bool flag = Hidden; Visible = !value; if (onPageHiddenChange != null && flag != value) onPageHiddenChange.Invoke(); } }

        public delegate void PageHiddenChange();

        private PageHiddenChange onPageHiddenChange;

        [Browsable(true), Category("ABControls"), Description("Fires on hidden state change")]
        public event PageHiddenChange OnPageHiddenChange
        {
            add
            {
                onPageHiddenChange += value;
            }
            remove
            {
                onPageHiddenChange -= value;
            }
        }

        public DraggablePanel() : base()
        {
            this.SetStyle(
                System.Windows.Forms.ControlStyles.UserPaint |
                System.Windows.Forms.ControlStyles.AllPaintingInWmPaint |
                System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer,
                true);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.Capture = false;
            Message m = Message.Create(FindForm().Handle, 0xa1, new IntPtr(2), IntPtr.Zero);
            WndProc(ref m);
        }
    }
}