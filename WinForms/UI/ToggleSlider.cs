using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Threading;

namespace ABSoftware.UI
{
    public class ToggleSlider : CheckBox
    {
        private Color onBackgroundColor = Color.FromArgb(0xAE, 0xDD, 0x92);
        private Color onForegroundColor = Color.WhiteSmoke;
        private Color offBackgroundColor = Color.FromArgb(0xEF, 0x6E, 0x6B);
        private Color offForegroundColor = Color.WhiteSmoke;
        private DefaultTogglePosition togglePosition = DefaultTogglePosition.Left;
        private bool flat = false;

        private bool toggleAnimations = false;
        private float currentTogglePosition;
        private ColorF currentBackgroundColor;
        private ColorF currentForegroundColor;
        private bool forceEndAnimation = false;

        [Category("ABSoftware"), RefreshProperties(RefreshProperties.Repaint)]
        public Color OnBackgroundColor { get { return onBackgroundColor; } set { onBackgroundColor = value; currentBackgroundColor = GetBackgroundColor(this.Checked); Invalidate(); } }
        [Category("ABSoftware"), RefreshProperties(RefreshProperties.Repaint)]
        public Color OnForegroundColor { get { return onForegroundColor; } set { onForegroundColor = value; currentForegroundColor = GetForegroundColor(this.Checked); Invalidate(); } }
        [Category("ABSoftware"), RefreshProperties(RefreshProperties.Repaint)]
        public Color OffBackgroundColor { get { return offBackgroundColor; } set { offBackgroundColor = value; currentBackgroundColor = GetBackgroundColor(this.Checked); Invalidate(); } }
        [Category("ABSoftware"), RefreshProperties(RefreshProperties.Repaint)]
        public Color OffForegroundColor { get { return offForegroundColor; } set { offForegroundColor = value; currentForegroundColor = GetForegroundColor(this.Checked); Invalidate(); } }
        [Category("ABSoftware"), RefreshProperties(RefreshProperties.Repaint)]
        public bool ToggleAnimations { get { return toggleAnimations; } set { toggleAnimations = value; Invalidate(); } }
        [Category("ABSoftware"), Description("Sets the speed of the animation. Accepts numbers from 0 to 1.")]
        public float AnimationSpeed { get; set; } = 0.125f;
        [Category("ABSoftware"), RefreshProperties(RefreshProperties.Repaint)]
        public DefaultTogglePosition TogglePosition { get { return togglePosition; } set { togglePosition = value; Invalidate(); } }
        [Category("ABSoftware"), RefreshProperties(RefreshProperties.Repaint)]
        public bool Flat { get { return flat; } set { flat = value; Invalidate(); } }

        public ToggleSlider()
        {
            
        }

        GraphicsPath GetSliderShape()
        {
            int arc = this.Height - 1;
            Rectangle leftArc = new Rectangle(0, 0, arc, arc);
            Rectangle rightArc = new Rectangle(this.Width - arc - 2, 0, arc, arc);

            GraphicsPath path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(leftArc, 90, 180);
            path.AddArc(rightArc, 270, 180);
            path.CloseFigure();

            return path;
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            if (this.Checked)
            {
                currentForegroundColor = onForegroundColor;
                currentBackgroundColor = onBackgroundColor;
                currentTogglePosition = (togglePosition == DefaultTogglePosition.Left) ? this.Width - this.Height + 1 : 2;
            }
            else
            {
                currentForegroundColor = offForegroundColor;
                currentBackgroundColor = offBackgroundColor;
                currentTogglePosition = (togglePosition == DefaultTogglePosition.Left) ? 2 : this.Width - this.Height + 1;
            }
        }

        protected override void OnCheckedChanged(EventArgs e)
        {
            base.OnCheckedChanged(e);
            if(toggleAnimations)
            {
                forceEndAnimation = true;
                new Thread(() =>
                {
                    forceEndAnimation = false;
                    bool done = false;
                    int target = 0;
                    ColorF targetBackgroundColor = (this.Checked) ? onBackgroundColor : offBackgroundColor;
                    ColorF targetForegroundColor = (this.Checked) ? onForegroundColor : offForegroundColor;
                    if (this.Checked) target = (togglePosition == DefaultTogglePosition.Left) ? this.Width - this.Height + 1 : 2;
                    else target = (togglePosition == DefaultTogglePosition.Left) ? 2 : this.Width - this.Height + 1;

                    while (!done && !forceEndAnimation)
                    {
                        currentTogglePosition = SmoothApproach(currentTogglePosition, target, AnimationSpeed);
                        currentBackgroundColor.R = SmoothApproach(currentBackgroundColor.R, targetBackgroundColor.R, AnimationSpeed);
                        currentBackgroundColor.G = SmoothApproach(currentBackgroundColor.G, targetBackgroundColor.G, AnimationSpeed);
                        currentBackgroundColor.B = SmoothApproach(currentBackgroundColor.B, targetBackgroundColor.B, AnimationSpeed);

                        currentForegroundColor.R = SmoothApproach(currentForegroundColor.R, targetForegroundColor.R, AnimationSpeed);
                        currentForegroundColor.G = SmoothApproach(currentForegroundColor.G, targetForegroundColor.G, AnimationSpeed);
                        currentForegroundColor.B = SmoothApproach(currentForegroundColor.B, targetForegroundColor.B, AnimationSpeed);
                        if (Math.Abs(target - currentTogglePosition) <= 1f)
                        {
                            currentTogglePosition = target;
                            currentBackgroundColor = targetBackgroundColor;
                            currentForegroundColor = targetForegroundColor;
                            done = true;
                        }
                        Invalidate();
                        Thread.Sleep(1);
                    }
                    targetBackgroundColor = default(ColorF);
                    targetForegroundColor = default(ColorF);
                    target = 0;
                }).Start();
            }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            int toggle = this.Height - 5;

            Rectangle toggleRight = new Rectangle(this.Width - this.Height + 1, 2, toggle, toggle), toggleLeft = new Rectangle(2, 2, toggle, toggle);

            pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            pevent.Graphics.Clear(Parent.BackColor);

            if(toggleAnimations)
            {
                if (this.Checked)
                {
                    if(flat)
                        pevent.Graphics.DrawPath(new Pen((Color)currentBackgroundColor), GetSliderShape());
                    else
                        pevent.Graphics.FillPath(new SolidBrush((Color)currentBackgroundColor), GetSliderShape());
                    pevent.Graphics.FillEllipse(new SolidBrush((Color)currentForegroundColor), currentTogglePosition, 2, toggle, toggle);
                }
                else
                {
                    if (flat)
                        pevent.Graphics.DrawPath(new Pen((Color)currentBackgroundColor), GetSliderShape());
                    else
                        pevent.Graphics.FillPath(new SolidBrush((Color)currentBackgroundColor), GetSliderShape());
                    pevent.Graphics.FillEllipse(new SolidBrush((Color)currentForegroundColor), currentTogglePosition, 2, toggle, toggle);
                }
            }
            else
            {
                if (this.Checked)
                {
                    if (flat)
                        pevent.Graphics.DrawPath(new Pen(onBackgroundColor), GetSliderShape());
                    else
                        pevent.Graphics.FillPath(new SolidBrush(onBackgroundColor), GetSliderShape());
                    pevent.Graphics.FillEllipse(new SolidBrush(onForegroundColor), (togglePosition == DefaultTogglePosition.Left) ? toggleRight : toggleLeft);
                }
                else
                {
                    if (flat)
                        pevent.Graphics.DrawPath(new Pen(offBackgroundColor), GetSliderShape());
                    else
                        pevent.Graphics.FillPath(new SolidBrush(offBackgroundColor), GetSliderShape());
                    pevent.Graphics.FillEllipse(new SolidBrush(offForegroundColor), (togglePosition == DefaultTogglePosition.Left) ? toggleLeft : toggleRight);
                }
            }
        }

        private float SmoothApproach(float value, float target, float smoothness)
        {
            return value + (target - value) * smoothness;
        }

        public void EnableAnimations(bool state)
        {
            currentBackgroundColor = (this.Checked) ? onBackgroundColor : offBackgroundColor;
            currentForegroundColor = (this.Checked) ? onForegroundColor : offForegroundColor;
            currentTogglePosition = (this.Checked) ? ((togglePosition == DefaultTogglePosition.Left) ? this.Width - this.Height + 1 : 2) : ((togglePosition == DefaultTogglePosition.Left) ? 2 : this.Width - this.Height + 1);
            this.ToggleAnimations = state;
        }

        public Color GetBackgroundColor(bool state)
        {
            return (state) ? onBackgroundColor : offBackgroundColor;
        }

        public Color GetForegroundColor(bool state)
        {
            return (state) ? onForegroundColor : offForegroundColor;
        }

        public enum DefaultTogglePosition
        {
            Left,
            Right
        }

        public struct ColorF
        {
            public float R, G, B;

            public ColorF(Color color)
            {
                this.R = color.R / 255f;
                this.G = color.G / 255f;
                this.B = color.B / 255f;
            }

            public static explicit operator Color(ColorF colorF)
            {
                return Color.FromArgb((byte)(colorF.R * 255f), (byte)(colorF.G * 255f), (byte)(colorF.B * 255f));
            }

            public static implicit operator ColorF(Color color)
            {
                return new ColorF(color);
            }
        }
    }
}
