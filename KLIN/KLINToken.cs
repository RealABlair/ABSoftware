using System;

namespace ABSoftware
{
    public class KLINToken
    {
        public string PropertyName { get; set; } = null;
        public object PropertyObject { get; set; } = null;

        public string TokenString { get { return (PropertyObject != null && !IsComment) ? PropertyName + "=" + PropertyObject : PropertyName; } }

        public KLINToken Parent = null;
        public KLINToken[] Children = null;

        public int ChildrenCapacity
        {
            get { return Children.Length; }
            set
            {
                KLINToken[] newArray = new KLINToken[value];
                Array.Copy(Children, 0, newArray, 0, ChildrenCount);
                Children = newArray;
            }
        }

        void ControlCapacity(int minCapacity)
        {
            if (this.Children.Length < minCapacity)
            {
                int newCapacity = (Children.Length == 0) ? 4 : (Children.Length * 2);

                if (newCapacity > int.MaxValue - 8)
                    newCapacity = int.MaxValue - 8;
                if (newCapacity < minCapacity)
                    newCapacity = minCapacity;

                this.ChildrenCapacity = newCapacity;
            }
        }

        public int ChildrenCount { get; private set; }

        public bool HasParent { get { return Parent != null; } }
        public bool HasChildren { get { return Children != null && ChildrenCount > 0; } }
        public bool IsComment { get { return PropertyName.Length >= 1 && PropertyName[0] == '#'; } }

        public KLINToken()
        {
            return;
        }

        public KLINToken(string PropertyName, object PropertyObject)
        {
            this.Children = new KLINToken[0];
            this.ChildrenCount = 0;

            this.PropertyName = PropertyName;
            this.PropertyObject = PropertyObject;
        }

        public KLINToken(string PropertyName, object PropertyObject, KLINToken Parent = null, KLINToken[] Children = null)
        {
            this.Children = new KLINToken[0];
            this.ChildrenCount = 0;

            this.PropertyName = PropertyName;
            this.PropertyObject = PropertyObject;
            this.Parent = Parent;

            if (Children != null)
            {
                this.Children = Children;
                this.ChildrenCount = Children.Length;
            }
        }

        public KLINToken this[string PropertyName]
        {
            get { if (HasChild(PropertyName)) return GetChild(PropertyName); else { AddChild(PropertyName, null); return GetChild(PropertyName); } }
            set { if (HasChild(PropertyName)) for (int i = 0; i < ChildrenCount; i++) { if (Children[i].PropertyName.Equals(PropertyName)) Children[i] = value; } }
        }

        public void AddChild(string PropertyName, object PropertyObject)
        {
            if (ChildrenCount == Children.Length)
                ControlCapacity(ChildrenCount + 1);
            Children[ChildrenCount] = new KLINToken(PropertyName, PropertyObject, this);
            ChildrenCount++;
        }

        public void AddChild(KLINToken token)
        {
            if (ChildrenCount == Children.Length)
                ControlCapacity(ChildrenCount + 1);
            Children[ChildrenCount] = token;
            ChildrenCount++;
            token.Parent = this;
        }

        public KLINToken GetChild(string PropertyName)
        {
            for (int i = 0; i < ChildrenCount; i++)
            {
                if (Children[i].PropertyName.Equals(PropertyName)) return Children[i];
            }
            return null;
        }

        public bool HasChild(string PropertyName)
        {
            for (int i = 0; i < ChildrenCount; i++) if (Children[i].PropertyName.Equals(PropertyName)) return true;
            return false;
        }

        public void RemoveChild(string PropertyName)
        {
            if (!HasChildren)
                return;
            for (int i = 0; i < ChildrenCount; i++)
                if (Children[i].PropertyName == PropertyName)
                    RemoveChild(i);
        }

        public void RemoveChild(int PropertyId)
        {
            if (!HasChildren)
                return;
            Array.Copy(this.Children, PropertyId + 1, this.Children, PropertyId, ChildrenCount - PropertyId - 1);
            ChildrenCount--;
        }
    }
}