using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace ABSoftware.WinForms.UI
{
    [DefaultEvent("ValueChanged")]
    public class CircularKnob : Control
    {
        [Category("ABSoftware UI"), Description("Knob back color")]
        public Color KnobBackColor { get { return knobBackColor; } set { knobBackColor = value; Invalidate(); } }
        Color knobBackColor = SystemColors.ControlLight;

        [Category("ABSoftware UI"), Description("Knob circle width")]
        public float KnobWidth { get { return knobWidth; } set { knobWidth = value; Invalidate(); } }
        float knobWidth = 1f;

        [Category("ABSoftware UI"), Description("Dashes count")]
        public int DashesCount { get { return dashesCount; } set { dashesCount = value; Invalidate(); } }
        int dashesCount = 12;

        [Category("ABSoftware UI"), Description("Dash width")]
        public float DashWidth { get { return dashWidth; } set { dashWidth = value; Invalidate(); } }
        float dashWidth = 1f;

        [Category("ABSoftware UI"), Description("Dash length")]
        public float DashLength { get { return dashLength; } set { dashLength = value; Invalidate(); } }
        float dashLength = 1f;

        [Category("ABSoftware UI"), Description("Dash gap")]
        public float DashGap { get { return dashGap; } set { dashGap = value; Invalidate(); } }
        float dashGap = 1f;

        [Category("ABSoftware UI"), Description("Knob handle back color")]
        public Color KnobHandleBackColor { get { return knobHandleBackColor; } set { knobHandleBackColor = value; Invalidate(); } }
        Color knobHandleBackColor = SystemColors.ControlLight;

        [Category("ABSoftware")]
        public float MinimumValue { get { return minimumValue; } set { minimumValue = value; Invalidate(); } }
        public float minimumValue = 0f;
        [Category("ABSoftware")]
        public float MaximumValue { get { return maximumValue; } set { maximumValue = value; Invalidate(); } }
        float maximumValue = 1f;
        [Category("ABSoftware")]
        public float Value { get { return value; } set { this.value = value; Invalidate(); OnValueChanged(EventArgs.Empty); } }
        float value = 0f;

        private static readonly float PI = 3.141592653589f;
        private static readonly float PI2 = PI * 2f;
        private static readonly float PIH = PI / 2f;

        bool canChangeNumber = false;

        public CircularKnob()
        {
            this.DoubleBuffered = true;
        }

        float GetDirectionAngle()
        {
            float minAngle = (dashGap / 2f);
            float maxAngle = (PI2 - dashGap) + (dashGap / 2f);
            float t = (value - minimumValue) / (maximumValue - minimumValue);

            return minAngle * (1f - t) + maxAngle * t;
        }

        PointF[] TransformRotate(PointF[] array, float radAngle, float xOffset, float yOffset)
        {
            PointF[] transformedPoints = new PointF[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                transformedPoints[i] = new PointF(array[i].X * (float)Math.Cos(radAngle) - array[i].Y * (float)Math.Sin(radAngle) + xOffset,
                                                  array[i].X * (float)Math.Sin(radAngle) + array[i].Y * (float)Math.Cos(radAngle) + yOffset);
            }

            return transformedPoints;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            RectangleF rect = new RectangleF(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width - 1f, ClientRectangle.Height - 1f);
            float radiusX = rect.Width / 2f;
            float radiusY = rect.Height / 2f;
            e.Graphics.FillEllipse(new SolidBrush(KnobBackColor), rect);
            using(Pen pen = new Pen(new SolidBrush(ForeColor), knobWidth))
            {
                pen.Alignment = PenAlignment.Inset;
                e.Graphics.DrawEllipse(pen, rect);

                using (Pen dashPen = new Pen(new SolidBrush(ForeColor), dashWidth))
                {
                    float cx = Width / 2f;
                    float cy = Height / 2f;
                    float step = (PI2 - dashGap) / (dashesCount - 1);
                    for (int i = 0; i < dashesCount; i++)
                    {
                        float angle = step * i + (dashGap / 2f);
                        e.Graphics.DrawLine(dashPen, cx + (float)Math.Sin(angle) * (radiusX - dashLength), cy + (float)Math.Cos(angle) * (radiusY - dashLength), cx + (float)Math.Sin(angle) * radiusX, cy + (float)Math.Cos(angle) * radiusY);
                    }
                }
            }

            e.Graphics.FillEllipse(new SolidBrush(KnobHandleBackColor), new RectangleF(radiusX / 2f, radiusY / 2f, rect.Width / 2f, rect.Height / 2f));
            float direction = GetDirectionAngle() + PI;
            e.Graphics.FillPolygon(new SolidBrush(KnobHandleBackColor), TransformRotate(new PointF[] { new PointF(-30f, 0f), new PointF(0f, -50f), new PointF(30f, 0f) }, direction, radiusX, radiusY));
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            canChangeNumber = true;
            base.OnMouseDown(e);

            TryChangeNumber(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            canChangeNumber = false;
            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            TryChangeNumber(e);
        }

        void TryChangeNumber(MouseEventArgs e)
        {
            if (!canChangeNumber)
                return;

            float minAngle = (dashGap / 2f);
            float maxAngle = (PI2 - dashGap) + (dashGap / 2f);

            float cx = Width / 2f;
            float cy = Height / 2f;

            float dx = e.X - cx;
            float dy = e.Y - cy;
            float angle = (float)Math.Atan2(dy, dx) - PIH;
            if (angle < 0)
                angle += PI2;

            float t = Clamp(0f, 1f, (angle - minAngle) / (maxAngle - minAngle));

            Value = minimumValue * (1f - t) + maximumValue * t;
        }

        float Clamp(float min, float max, float value)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;

            return value;
        }

        protected void OnValueChanged(EventArgs e)
        {
            if(this.ValueChanged != null)
            {
                this.ValueChanged.Invoke(this, e);
            }
        }

        [Category("ABSoftware Events")]
        public event EventHandler ValueChanged;
    }
}
