using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ABSoftware.UI
{
    public class RoundedButton : Button
    {
        private float _borderSize = 0f;
        private float _radius = 0f;
        private Color _borderColor = Color.Black;

        [Category("ABSoftware UI")]
        public float Radius { get { return _radius; } set { _radius = value; Invalidate(); } }
        [Category("ABSoftware UI")]
        public float BorderSize { get { return _borderSize; } set { _borderSize = value; Invalidate(); } }
        [Category("ABSoftware UI")]
        public Color BorderColor { get { return _borderColor; } set { _borderColor = value; Invalidate(); } }

        public RoundedButton() : base()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
        }

        private GraphicsPath GetRoundedCorners(RectangleF Rect, float Radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(Rect.X, Rect.Y, Radius, Radius, 180, 90);
            path.AddArc(Rect.X + Rect.Width - Radius, Rect.Y, Radius, Radius, 270, 90);
            path.AddArc(Rect.X + Rect.Width - Radius, Rect.Y + Rect.Height - Radius, Radius, Radius, 0, 90);
            path.AddArc(Rect.X, Rect.Y + Rect.Height - Radius, Radius, Radius, 90, 90);
            path.CloseFigure();
            return path;
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
            
            RectangleF Rect = new RectangleF(0, 0, this.Width, this.Height);
            RectangleF RectBorder = RectangleF.Inflate(Rect, -BorderSize/2f, -BorderSize/2f);
            if (this.Radius > 1)
            {
                using (GraphicsPath pathBorder = GetRoundedCorners(RectBorder, this.Radius - this.BorderSize))
                using (GraphicsPath path = GetRoundedCorners(Rect, this.Radius))
                using (Pen pen = new Pen(BackColor, 2f))
                using (Pen penBorder = new Pen(BorderColor, BorderSize))
                {
                    pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    this.Region = new Region(path);

                    pevent.Graphics.DrawPath(pen, path);

                    if (BorderSize > 0)
                        pevent.Graphics.DrawPath(penBorder, pathBorder);
                }
            }
            else
            {
                this.Region = new Region(Rect);
                using (Pen p = new Pen(BackColor, 1f))
                {
                    p.Alignment = PenAlignment.Inset;
                    pevent.Graphics.DrawRectangle(p, 0, 0, this.Width-1, this.Height-1);
                }
            }
        }

        /*protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);
            Invalidate();
        }*/
    }
}
