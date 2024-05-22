using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace ABSoftware.UI
{
    [Designer(typeof(TabControllerRadioButtonDesigner))]
    public class TabControllerRadioButton : RadioButton
    {
        [RefreshProperties(RefreshProperties.All)]
        public int LinksCount { get { return links.Length; } set { Array.Resize(ref links, value); Refresh(); } }
        private DraggablePanel[] links = new DraggablePanel[0];
        [RefreshProperties(RefreshProperties.All)]
        public DraggablePanel[] Links { get { return links; } set { links = value; Refresh(); } }
    }

    public class TabControllerRadioButtonDesigner : ControlDesigner
    {
        public TabControllerRadioButtonDesigner()
        {

        }

        bool isClickable = false;
        TabControllerRadioButton radioButton;

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);

            radioButton = component as TabControllerRadioButton;
            radioButton.CheckedChanged += (sender, args) => { UpdateLinks(); };
        }

        public void UpdateLinks()
        {
            for (int i = 0; i < radioButton.Links.Length; i++)
            {
                radioButton.Links[i].Hidden = !radioButton.Checked;
            }
        }

        protected override void OnMouseEnter()
        {
            base.OnMouseEnter();

            isClickable = true;
        }

        protected override void OnMouseLeave()
        {
            base.OnMouseLeave();

            isClickable = false;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 514 && isClickable)
            {
                radioButton.PerformClick();
            }
            base.WndProc(ref m);
        }
    }
}
