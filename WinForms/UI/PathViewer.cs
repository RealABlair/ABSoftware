using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Windows.Forms.Design;
using System.Runtime.InteropServices;

namespace ABSoftware.WinForms.UI
{
    [Designer(typeof(PathViewerDesigner))]
    public class PathViewer : ScrollableControl
    {
        private const uint SHGFI_ICON = 0x100;
        private const uint SHGFI_LARGEICON = 0x0;
        private const uint SHGFI_SMALLICON = 0x1;

        [DllImport("shell32.dll")]
        static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        [StructLayout(LayoutKind.Sequential)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        public static Icon GetIconFromPath(string path)
        {
            SHFILEINFO shinfo = new SHFILEINFO();
            SHGetFileInfo(path, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), SHGFI_ICON | SHGFI_LARGEICON);
            return Icon.FromHandle(shinfo.hIcon);
        }


        [Category("ABSoftware UI"), Description("A path that needs to be checked and rendered.")]
        public string ViewerPath { get { return viewerPath; } set { viewerPath = FormatPath(value); UpdateView(); } }
        private string viewerPath;

        [Category("ABSoftware UI"), Description("Folders in the viewer will be active and clickable.")]
        public bool ActiveFolders { get; set; } = true;

        public List<PathViewerFile> Files = new List<PathViewerFile>();

        [Category("ABSoftware UI")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public PathViewerFile Template { get; }

        public PathViewer() : base()
        {
            this.Size = new Size(250, 250);
            this.AutoScroll = true;

            //Invalidate();
            this.Template = new PathViewerFile();
            this.Controls.Add(Template);

            if (!IsInDesignMode)
                Template.Visible = false;
            //this.Controls.Add(template);
        }

        private string FormatPath(string path)
        {
            if (path == null)
                return string.Empty;

            path = path.Replace('/', '\\');
            if (path.EndsWith("\\"))
                return path;

            return (path + "\\");
        }

        public void UpdateView()
        {
            if (IsInDesignMode || DesignMode)
                return;
            this.Controls.Clear();
            this.Controls.Add(Template);

            if (this.viewerPath == string.Empty || this.viewerPath == "\\")
                return;

            this.SuspendLayout();
            string[] directories = Directory.GetDirectories(viewerPath);
            for (int i = 0; i < directories.Length; i++)
            {
                string directory = directories[i] + "\\";
                PathViewerFile pvf = new PathViewerFile(Template, directory, false);
                Files.Add(pvf);
                this.Controls.Add(pvf);
                this.Controls.SetChildIndex(pvf, 0);

                if (ActiveFolders)
                {
                    pvf.SetClickHandler((sender, e) =>
                    {
                        ViewerPath = directory;
                    });
                }
            }
            
            string[] files = Directory.GetFiles(viewerPath);
            for (int i = 0; i < files.Length; i++)
            {
                PathViewerFile pvf = new PathViewerFile(Template, files[i], true);
                Files.Add(pvf);
                this.Controls.Add(pvf);
                this.Controls.SetChildIndex(pvf, 0);
            }

            if(Files.Count > 0)
            {
                DirectoryInfo info = Directory.GetParent(Path.GetFullPath(viewerPath.TrimEnd('\\')));
                if (info != null)
                {
                    PathViewerFile pvf = new PathViewerFile(Template, "..", GetIconFromPath(".."));
                    this.Controls.Add(pvf);
                    this.Controls.SetChildIndex(pvf, -1);

                    if(ActiveFolders)
                    {
                        pvf.SetClickHandler((sender, e) =>
                        {
                            ViewerPath = info.FullName;
                        });
                    }
                }
            }
            this.ResumeLayout();
        }

        static bool IsInDesignMode
        {
            get
            {
                return LicenseManager.UsageMode == LicenseUsageMode.Designtime;
            }
        }

        [Designer(typeof(PathViewerFileDesigner))]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class PathViewerFile : Panel
        {
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
            public PictureBox EntryIcon { get; private set; }

            [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
            public Label EntryName { get; private set; }

            public PathViewerFile() : base()
            {
                this.Dock = DockStyle.Top;
                this.Height = 40;
                this.Padding = new Padding(5);


                EntryIcon = new PictureBox
                {
                    Name = "EntryIcon",
                    Dock = DockStyle.Left,
                    Size = new Size(30, 30),
                    SizeMode = PictureBoxSizeMode.Zoom,
                };

                EntryName = new Label
                {
                    Name = "EntryName",
                    Text = "File Name",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft
                };

                this.Controls.Add(EntryName);
                this.Controls.Add(EntryIcon);
            }

            public PathViewerFile(PathViewerFile template, string path, bool isFile) : base()
            {
                this.BackColor = template.BackColor;
                this.BackgroundImage = template.BackgroundImage;
                this.BackgroundImageLayout = template.BackgroundImageLayout;
                this.BorderStyle = template.BorderStyle;
                this.Cursor = template.Cursor;
                this.Font = template.Font;
                this.ForeColor = template.ForeColor;
                this.RightToLeft = template.RightToLeft;
                this.UseWaitCursor = template.UseWaitCursor;

                this.Tag = template.Tag;

                this.Anchor = template.Anchor;
                this.AutoScroll = template.AutoScroll;
                this.AutoScrollMargin = template.AutoScrollMargin;
                this.AutoScrollMinSize = template.AutoScrollMinSize;
                this.AutoSize = template.AutoSize;
                this.AutoSizeMode = template.AutoSizeMode;
                this.Dock = template.Dock;
                this.Location = template.Location;
                this.Margin = template.Margin;
                this.MaximumSize = template.MaximumSize;
                this.MinimumSize = template.MinimumSize;
                this.Padding = template.Padding;
                this.Size = template.Size;

                this.AllowDrop = template.AllowDrop;
                this.ContextMenuStrip = template.ContextMenuStrip;
                this.Enabled = template.Enabled;
                this.ImeMode = template.ImeMode;
                //this.TabIndex = template.TabIndex;
                this.TabStop = template.TabStop;

                this.CausesValidation = template.CausesValidation;


                EntryIcon = new PictureBox
                {
                    Name = "EntryIcon",
                    Dock = DockStyle.Left,
                    Size = new Size(30, 30),
                    Image = isFile ? Icon.ExtractAssociatedIcon(path).ToBitmap() : GetIconFromPath(path).ToBitmap()
                };

                EntryIcon.SizeMode = template.EntryIcon.SizeMode;

                EntryIcon.BackColor = template.EntryIcon.BackColor;
                EntryIcon.BackgroundImage = template.EntryIcon.BackgroundImage;
                EntryIcon.BackgroundImageLayout = template.EntryIcon.BackgroundImageLayout;
                EntryIcon.BorderStyle = template.EntryIcon.BorderStyle;
                EntryIcon.Cursor = template.EntryIcon.Cursor;
                EntryIcon.Font = template.EntryIcon.Font;
                EntryIcon.ForeColor = template.EntryIcon.ForeColor;
                EntryIcon.RightToLeft = template.EntryIcon.RightToLeft;
                EntryIcon.UseWaitCursor = template.EntryIcon.UseWaitCursor;

                EntryIcon.Tag = template.EntryIcon.Tag;

                EntryIcon.Anchor = template.EntryIcon.Anchor;
                EntryIcon.AutoSize = template.EntryIcon.AutoSize;
                EntryIcon.Dock = template.EntryIcon.Dock;
                EntryIcon.Location = template.EntryIcon.Location;
                EntryIcon.Margin = template.EntryIcon.Margin;
                EntryIcon.MaximumSize = template.EntryIcon.MaximumSize;
                EntryIcon.MinimumSize = template.EntryIcon.MinimumSize;
                EntryIcon.Padding = template.EntryIcon.Padding;
                EntryIcon.Size = template.EntryIcon.Size;

                EntryIcon.AllowDrop = template.EntryIcon.AllowDrop;
                EntryIcon.ContextMenuStrip = template.EntryIcon.ContextMenuStrip;
                EntryIcon.Enabled = template.EntryIcon.Enabled;
                EntryIcon.ImeMode = template.EntryIcon.ImeMode;
                //EntryIcon.TabIndex = template.TabIndex;
                EntryIcon.TabStop = template.EntryIcon.TabStop;

                EntryIcon.CausesValidation = template.EntryIcon.CausesValidation;

                EntryName = new Label
                {
                    Name = "EntryName",
                    Text = isFile ? Path.GetFileName(path) : Path.GetFileName(Path.GetDirectoryName(path)),
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft
                };

                EntryName.BackColor = template.EntryName.BackColor;
                EntryName.BackgroundImage = template.EntryName.BackgroundImage;
                EntryName.BackgroundImageLayout = template.EntryName.BackgroundImageLayout;
                EntryName.BorderStyle = template.EntryName.BorderStyle;
                EntryName.Cursor = template.EntryName.Cursor;
                EntryName.Font = template.EntryName.Font;
                EntryName.ForeColor = template.EntryName.ForeColor;
                EntryName.RightToLeft = template.EntryName.RightToLeft;
                EntryName.UseWaitCursor = template.EntryName.UseWaitCursor;
                
                EntryName.Tag = template.EntryName.Tag;

                EntryName.TextAlign = template.EntryName.TextAlign;

                EntryName.Anchor = template.EntryName.Anchor;
                EntryName.AutoSize = template.EntryName.AutoSize;
                EntryName.Dock = template.EntryName.Dock;
                EntryName.Location = template.EntryName.Location;
                EntryName.Margin = template.EntryName.Margin;
                EntryName.MaximumSize = template.EntryName.MaximumSize;
                EntryName.MinimumSize = template.EntryName.MinimumSize;
                EntryName.Padding = template.EntryName.Padding;
                EntryName.Size = template.EntryName.Size;
                
                EntryName.AllowDrop = template.EntryName.AllowDrop;
                EntryName.ContextMenuStrip = template.EntryName.ContextMenuStrip;
                EntryName.Enabled = template.EntryName.Enabled;
                EntryName.ImeMode = template.EntryName.ImeMode;
                //EntryName.TabIndex = template.TabIndex;
                EntryName.TabStop = template.EntryName.TabStop;
                
                EntryName.CausesValidation = template.EntryName.CausesValidation;
                
                this.Controls.Add(EntryName);
                this.Controls.Add(EntryIcon);
            }

            public PathViewerFile(PathViewerFile template, string name, Icon icon) : base()
            {
                this.BackColor = template.BackColor;
                this.BackgroundImage = template.BackgroundImage;
                this.BackgroundImageLayout = template.BackgroundImageLayout;
                this.BorderStyle = template.BorderStyle;
                this.Cursor = template.Cursor;
                this.Font = template.Font;
                this.ForeColor = template.ForeColor;
                this.RightToLeft = template.RightToLeft;
                this.UseWaitCursor = template.UseWaitCursor;

                this.Tag = template.Tag;

                this.Anchor = template.Anchor;
                this.AutoScroll = template.AutoScroll;
                this.AutoScrollMargin = template.AutoScrollMargin;
                this.AutoScrollMinSize = template.AutoScrollMinSize;
                this.AutoSize = template.AutoSize;
                this.AutoSizeMode = template.AutoSizeMode;
                this.Dock = template.Dock;
                this.Location = template.Location;
                this.Margin = template.Margin;
                this.MaximumSize = template.MaximumSize;
                this.MinimumSize = template.MinimumSize;
                this.Padding = template.Padding;
                this.Size = template.Size;

                this.AllowDrop = template.AllowDrop;
                this.ContextMenuStrip = template.ContextMenuStrip;
                this.Enabled = template.Enabled;
                this.ImeMode = template.ImeMode;
                //this.TabIndex = template.TabIndex;
                this.TabStop = template.TabStop;

                this.CausesValidation = template.CausesValidation;


                EntryIcon = new PictureBox
                {
                    Name = "EntryIcon",
                    Dock = DockStyle.Left,
                    Size = new Size(30, 30),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Image = icon.ToBitmap()
                };

                EntryIcon.SizeMode = template.EntryIcon.SizeMode;

                EntryIcon.BackColor = template.EntryIcon.BackColor;
                EntryIcon.BackgroundImage = template.EntryIcon.BackgroundImage;
                EntryIcon.BackgroundImageLayout = template.EntryIcon.BackgroundImageLayout;
                EntryIcon.BorderStyle = template.EntryIcon.BorderStyle;
                EntryIcon.Cursor = template.EntryIcon.Cursor;
                EntryIcon.Font = template.EntryIcon.Font;
                EntryIcon.ForeColor = template.EntryIcon.ForeColor;
                EntryIcon.RightToLeft = template.EntryIcon.RightToLeft;
                EntryIcon.UseWaitCursor = template.EntryIcon.UseWaitCursor;

                EntryIcon.Tag = template.EntryIcon.Tag;

                EntryIcon.Anchor = template.EntryIcon.Anchor;
                EntryIcon.AutoSize = template.EntryIcon.AutoSize;
                EntryIcon.Dock = template.EntryIcon.Dock;
                EntryIcon.Location = template.EntryIcon.Location;
                EntryIcon.Margin = template.EntryIcon.Margin;
                EntryIcon.MaximumSize = template.EntryIcon.MaximumSize;
                EntryIcon.MinimumSize = template.EntryIcon.MinimumSize;
                EntryIcon.Padding = template.EntryIcon.Padding;
                EntryIcon.Size = template.EntryIcon.Size;

                EntryIcon.AllowDrop = template.EntryIcon.AllowDrop;
                EntryIcon.ContextMenuStrip = template.EntryIcon.ContextMenuStrip;
                EntryIcon.Enabled = template.EntryIcon.Enabled;
                EntryIcon.ImeMode = template.EntryIcon.ImeMode;
                //EntryIcon.TabIndex = template.TabIndex;
                EntryIcon.TabStop = template.EntryIcon.TabStop;

                EntryIcon.CausesValidation = template.EntryIcon.CausesValidation;

                EntryName = new Label
                {
                    Name = "EntryName",
                    Text = name,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft
                };

                EntryName.BackColor = template.EntryName.BackColor;
                EntryName.BackgroundImage = template.EntryName.BackgroundImage;
                EntryName.BackgroundImageLayout = template.EntryName.BackgroundImageLayout;
                EntryName.BorderStyle = template.EntryName.BorderStyle;
                EntryName.Cursor = template.EntryName.Cursor;
                EntryName.Font = template.EntryName.Font;
                EntryName.ForeColor = template.EntryName.ForeColor;
                EntryName.RightToLeft = template.EntryName.RightToLeft;
                EntryName.UseWaitCursor = template.EntryName.UseWaitCursor;

                EntryName.Tag = template.EntryName.Tag;

                EntryName.TextAlign = template.EntryName.TextAlign;

                EntryName.Anchor = template.EntryName.Anchor;
                EntryName.AutoSize = template.EntryName.AutoSize;
                EntryName.Dock = template.EntryName.Dock;
                EntryName.Location = template.EntryName.Location;
                EntryName.Margin = template.EntryName.Margin;
                EntryName.MaximumSize = template.EntryName.MaximumSize;
                EntryName.MinimumSize = template.EntryName.MinimumSize;
                EntryName.Padding = template.EntryName.Padding;
                EntryName.Size = template.EntryName.Size;

                EntryName.AllowDrop = template.EntryName.AllowDrop;
                EntryName.ContextMenuStrip = template.EntryName.ContextMenuStrip;
                EntryName.Enabled = template.EntryName.Enabled;
                EntryName.ImeMode = template.EntryName.ImeMode;
                //EntryName.TabIndex = template.TabIndex;
                EntryName.TabStop = template.EntryName.TabStop;

                EntryName.CausesValidation = template.EntryName.CausesValidation;

                this.Controls.Add(EntryName);
                this.Controls.Add(EntryIcon);
            }

            public void SetClickHandler(EventHandler handler)
            {
                this.Click += handler;
                this.EntryIcon.Click += handler;
                this.EntryName.Click += handler;
            }
        }

        public class PathViewerDesigner : ParentControlDesigner
        {
            public override void Initialize(IComponent component)
            {
                base.Initialize(component);
                PathViewer control = (PathViewer)component;

                EnableDesignMode(control.Template, "Template");
            }
        }

        public class PathViewerFileDesigner : ParentControlDesigner
        {
            public override void Initialize(IComponent component)
            {
                base.Initialize(component);
                PathViewerFile control = (PathViewerFile)component;

                EnableDesignMode(control.EntryIcon, "EntryIcon");
                EnableDesignMode(control.EntryName, "EntryName");
            }
        }
    }
}
