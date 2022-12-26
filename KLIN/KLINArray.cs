using System;
using System.Text;

namespace ABSoftware
{
    public class KLINArray
    {
        object[] array;

        public int Size { get { return array.Length; } }

        public KLINArray()
        {
            this.array = new object[0];
        }

        public KLINArray(params object[] objects)
        {
            this.array = objects;
        }

        public object this[int id]
        {
            get { return this.array[id]; }
            set { this.array[id] = value; }
        }

        public void Add(object obj)
        {
            Array.Resize(ref this.array, this.array.Length + 1);
            this.array[this.array.Length - 1] = obj;
        }

        public object Get(int id)
        {
            if (id >= this.array.Length)
                return null;

            return this.array[id];
        }

        public void Clear()
        {
            this.array = new object[0];
        }

        public void Parse(string KLIN)
        {
            KLIN = KLIN.Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries)[0].Replace(", ", ",");
            string[] entries = KLIN.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            Clear();

            for(int i = 0; i < entries.Length; i++)
            {
                if(entries[i].Contains("\""))
                {
                    Add(entries[i].Split(new char[] { '\"' }, StringSplitOptions.RemoveEmptyEntries)[0]);
                }
                else if(entries[i].ToLower().Contains("true") || entries[i].ToLower().Contains("false"))
                {
                    Add(bool.Parse(entries[i]));
                }
                else if(entries[i].Contains("."))
                {
                    Add(float.Parse(entries[i].Replace(".", ",")));
                }
                else
                {
                    Add(int.Parse(entries[i]));
                }
            }
        }

        private readonly StringBuilder ToStringBuilder = new StringBuilder();
        public override string ToString()
        {
            ToStringBuilder.Clear();
            ToStringBuilder.Append('[');
            for(int i = 0; i < array.Length; i++)
            {
                if(array[i].GetType().Equals(typeof(String)))
                {
                    ToStringBuilder.Append("\"" + array[i] + "\"");
                }
                else
                {
                    ToStringBuilder.Append(array[i].ToString().Replace(",", "."));
                }

                if (i + 1 < array.Length)
                    ToStringBuilder.Append(", ");
            }
            ToStringBuilder.Append(']');
            return ToStringBuilder.ToString();
        }
    }
}
