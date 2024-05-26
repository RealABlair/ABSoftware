using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ABSoftware.UI
{
    public class RoundedButton : Button
    {
        private float _borderSize = 2f;
        private float _radius = 0.5f;
        private Color _borderColor = Color.Black;

        [Category("ABSoftware UI"), Description("Radius multiplier. Range from 0 to 1. 1 = max radius.")]
        public float RadiusStrength { get { return _radius; } set { _radius = value; Invalidate(); } }
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
            pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            pevent.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
            pevent.Graphics.Clear(Parent.BackColor);

            float radius = this.RadiusStrength * this.Height;

            RectangleF Rect = new RectangleF(0, 0, this.Width, this.Height);
            RectangleF RectBorder = RectangleF.Inflate(Rect, -BorderSize/2f, -BorderSize/2f);
            if (radius > 1)
            {
                using (GraphicsPath pathBorder = GetRoundedCorners(RectBorder, radius - this.BorderSize))
                using (GraphicsPath path = GetRoundedCorners(Rect, radius))
                using (SolidBrush brush = new SolidBrush(BackColor))
                using (Pen penBorder = new Pen(BorderColor, BorderSize))
                {
                    pevent.Graphics.FillPath(brush, path);

                    if (BorderSize > 0)
                        pevent.Graphics.DrawPath(penBorder, pathBorder);

                    PointF textLocation = GetTextLocation(pevent.Graphics.MeasureString(this.Text, this.Font), radius);
                    pevent.Graphics.DrawString(this.Text, this.Font, new SolidBrush(ForeColor), textLocation.X, textLocation.Y);
                }
            }
            else
            {
                using (SolidBrush brush = new SolidBrush(BackColor))
                using (Pen penBorder = new Pen(BorderColor, BorderSize))
                {
                    pevent.Graphics.FillRectangle(brush, 0, 0, this.Width - 1, this.Height - 1);
                    pevent.Graphics.DrawRectangle(penBorder, 1f, 1f, this.Width - 2, this.Height - 2);
                    PointF textLocation = GetTextLocation(pevent.Graphics.MeasureString(this.Text, this.Font), radius);
                    pevent.Graphics.DrawString(this.Text, this.Font, new SolidBrush(ForeColor), textLocation.X, textLocation.Y);
                }
            }
        }

        private PointF GetTextLocation(SizeF textSize, float radius)
        {
            switch(TextAlign)
            {
                case ContentAlignment.TopLeft:
                    return new PointF(0f + BorderSize + radius / 3, 0f + BorderSize);
                case ContentAlignment.TopCenter:
                    return new PointF(this.Width / 2f - textSize.Width / 2f, 0f + BorderSize);
                case ContentAlignment.TopRight:
                    return new PointF(this.Width - textSize.Width - BorderSize - radius / 3, 0f);
                case ContentAlignment.MiddleLeft:
                    return new PointF(0f + BorderSize, this.Height / 2f - textSize.Height / 2f);
                case ContentAlignment.MiddleCenter:
                    return new PointF(this.Width / 2f - textSize.Width / 2f, this.Height / 2f - textSize.Height / 2f);
                case ContentAlignment.MiddleRight:
                    return new PointF(this.Width - textSize.Width - BorderSize, this.Height / 2f - textSize.Height / 2f);
                case ContentAlignment.BottomLeft:
                    return new PointF(0f + this.BorderSize + radius / 3, this.Height - textSize.Height - BorderSize);
                case ContentAlignment.BottomCenter:
                    return new PointF(this.Width / 2f - textSize.Width / 2f, this.Height - textSize.Height - BorderSize);
                case ContentAlignment.BottomRight:
                    return new PointF(this.Width - textSize.Width - BorderSize - radius / 3, this.Height - textSize.Height - BorderSize);
                default:
                    return new PointF(0f, 0f);
            }
        }
    }
}