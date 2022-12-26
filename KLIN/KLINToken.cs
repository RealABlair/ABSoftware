using System;
using System.Text;

namespace ABSoftware
{
    public class KLINToken
    {
        public string PropertyName { get; set; } = null;
        public object PropertyObject { get; set; } = null;

        public string TokenString { get { return (PropertyObject != null && !IsComment) ? PropertyName + "=" + PropertyObject : PropertyName; } }

        public KLINToken Parent = null;
        public KLINToken[] Children = null;

        public bool HasParent { get { return Parent != null; } }
        public bool HasChildren { get { return Children != null && Children.Length > 0; } }
        public bool IsComment { get { return PropertyName.Length >= 1 && PropertyName[0] == '#'; } }

        public KLINToken()
        {
            return;
        }

        public KLINToken(string PropertyName, object PropertyObject)
        {
            this.Children = new KLINToken[0];

            this.PropertyName = PropertyName;
            this.PropertyObject = PropertyObject;
        }

        public KLINToken(string PropertyName, object PropertyObject, KLINToken Parent = null, KLINToken[] Children = null)
        {
            this.Children = new KLINToken[0];

            this.PropertyName = PropertyName;
            this.PropertyObject = PropertyObject;
            this.Parent = Parent;

            if(Children != null)
            this.Children = Children;
        }

        public KLINToken this[string PropertyName]
        {
            get { if (HasChild(PropertyName)) return GetChild(PropertyName); else { AddChild(PropertyName, null); return GetChild(PropertyName); } }
            set { if (HasChild(PropertyName)) for (int i = 0; i < Children.Length; i++)  { if (Children[i].PropertyName.Equals(PropertyName)) Children[i] = value; } }
        }

        public void AddChild(string PropertyName, object PropertyObject)
        {
            Array.Resize(ref Children, Children.Length + 1);
            Children[Children.Length - 1] = new KLINToken(PropertyName, PropertyObject, this);
        }

        public void AddChild(KLINToken token)
        {
            Array.Resize(ref Children, Children.Length + 1);
            Children[Children.Length - 1] = token;
            token.Parent = this;
        }

        public KLINToken GetChild(string PropertyName)
        {
            for(int i = 0; i < Children.Length; i++)
            {
                if (Children[i].PropertyName.Equals(PropertyName)) return Children[i];
            }
            return null;
        }

        public bool HasChild(string PropertyName)
        {
            for(int i = 0; i < Children.Length; i++) if (Children[i].PropertyName.Equals(PropertyName)) return true;
            return false;
        }
    }
}