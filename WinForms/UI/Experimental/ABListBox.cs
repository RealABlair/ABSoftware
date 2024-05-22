using LeagueOfLegends;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ABSoftware.UI
{
    public class ABListBox : ListBox
    {
        private Color itemBackColor = Color.White;
        private Color selectedItemBackColor = Color.White;
        private Color itemForeColor = Color.White;
        private Color selectedItemForeColor = Color.White;
        private Color itemSeparatorColor = Color.Black;
        private float itemSeparatorSize = 1f;
        private float descriptionFontSize = 10f;

        [Category("ABControls"), Description("Sets the list item's back color"), RefreshProperties(RefreshProperties.Repaint)]
        public Color ItemBackColor { get { return itemBackColor; } set { bool flag = itemBackColor != value; itemBackColor = value; if (flag) Invalidate(); } }

        [Category("ABControls"), Description("Sets the selected list item's back color"), RefreshProperties(RefreshProperties.Repaint)]
        public Color SelectedItemBackColor { get { return selectedItemBackColor; } set { bool flag = selectedItemBackColor != value; selectedItemBackColor = value; if(flag) Invalidate(); } }

        [Category("ABControls"), Description("Sets the list item's forecolor"), RefreshProperties(RefreshProperties.Repaint)]
        public Color ItemForeColor { get { return itemForeColor; } set { bool flag = itemForeColor != value; itemForeColor = value; if (flag) Invalidate(); } }

        [Category("ABControls"), Description("Sets the selected list item's forecolor"), RefreshProperties(RefreshProperties.Repaint)]
        public Color SelectedItemForeColor { get { return selectedItemForeColor; } set { bool flag = selectedItemForeColor != value; selectedItemForeColor = value; if (flag) Invalidate(); } }

        [Category("ABControls"), Description("Sets the separator color"), RefreshProperties(RefreshProperties.Repaint)]
        public Color ItemSeparatorColor { get { return itemSeparatorColor; } set { bool flag = itemSeparatorColor != value; itemSeparatorColor = value; if (flag) Invalidate(); } }

        [Category("ABControls"), Description("Sets the separator size"), RefreshProperties(RefreshProperties.Repaint)]
        public float ItemSeparatorSize { get { return itemSeparatorSize; }  set { bool flag = itemSeparatorSize != value; itemSeparatorSize = value; if (flag) Invalidate(); } }
        
        [Category("ABControls"), Description("Sets the description size"), RefreshProperties(RefreshProperties.Repaint)]
        public float DescriptionFontSize { get { return descriptionFontSize; } set { bool flag = descriptionFontSize != value; descriptionFontSize = value; if (flag) Invalidate(); } }

        public ABListBox() : base()
        {
            this.DoubleBuffered = true;
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer, true);
            DrawMode = DrawMode.OwnerDrawFixed;
            DrawItem += ABDrawItem;
        }

        public void ABDrawItem(object sender, DrawItemEventArgs args)
        {
            if (args.Index >= Items.Count || args.Index < 0)
                return;

            ABListBoxItem item = (ABListBoxItem)Items[args.Index];
            if ((args.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                args.Graphics.FillRectangle(new SolidBrush(SelectedItemBackColor), args.Bounds);
                args.Graphics.DrawString(item.Name, Font, new SolidBrush(SelectedItemForeColor), args.Bounds.X, args.Bounds.Y);
                args.Graphics.DrawString(item.Description, new Font(Font.FontFamily, DescriptionFontSize), new SolidBrush(SelectedItemForeColor), new RectangleF(args.Bounds.X, args.Bounds.Y + Font.Height, args.Bounds.Width, args.Bounds.Height - Font.Height));
            }
            else
            {
                args.Graphics.FillRectangle(new SolidBrush(ItemBackColor), args.Bounds);
                args.Graphics.DrawString(item.Name, Font, new SolidBrush(ItemForeColor), args.Bounds.X, args.Bounds.Y);
                args.Graphics.DrawString(item.Description, new Font(Font.FontFamily, DescriptionFontSize), new SolidBrush(ItemForeColor), new RectangleF(args.Bounds.X, args.Bounds.Y + Font.Height, args.Bounds.Width, args.Bounds.Height - Font.Height));
            }

            if(args.Index > 0) args.Graphics.DrawLine(new Pen(ItemSeparatorColor, ItemSeparatorSize), args.Bounds.X, args.Bounds.Y, args.Bounds.X + args.Bounds.Width, args.Bounds.Y);
            args.DrawFocusRectangle();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left && !this.ClientRectangle.Contains(e.Location))
            {
                LeagueOfLegendsABLauncher.MainForm.instance.Focus();
            }
        }

        public class ABListBoxItem
        {
            public string Name;
            public string Description;
            public ROFL replay;

            public ABListBoxItem(string Name, string Description, ROFL replay)
            {
                this.Name = Name;
                this.Description = Description;
                this.replay = replay;
            }

            /*public override string ToString()
            {
                return Name;
            }*/
        }
    }
}
