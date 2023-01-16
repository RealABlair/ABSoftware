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
            Clear();
            if (KLIN.Length > 0 && KLIN[0] != '[')
                return;
            bool readingToken = true;
            bool isInQuotes = false;

            string token = "";
            
            for(int i = 1; i < KLIN.Length; i++)
            {
                if (KLIN[i] == ']')
                {
                    if(token.Length > 0)
                    {
                        if (isInQuotes)
                            Add(token);
                        else
                            Add(GetTokenType(token));
                    }
                    break;
                }

                if (!isInQuotes && KLIN[i] == ' ')
                    continue;

                if(KLIN[i] == '"')
                {
                    if (isInQuotes && KLIN[i + 1] == ',')
                    {
                        Add(token);
                        token = "";
                        isInQuotes = false;
                        readingToken = true;
                    }
                    else
                        isInQuotes = true;
                    continue;
                }

                if (!readingToken && KLIN[i] != ',' && KLIN[i] != ' ')
                    readingToken = true;

                if(!isInQuotes && KLIN[i] == ',')
                {
                    readingToken = false;
                }

                if(readingToken)
                {
                    token += KLIN[i];
                }
                else if(token.Length > 0)
                {
                    Add(GetTokenType(token));
                    token = "";
                    readingToken = true;
                }
            }
        }

        private object GetTokenType(string token)
        {
            if (token[0] == 'u')
                return ulong.Parse(token.Substring(1));
            if (token.Contains("."))
                return double.Parse(token.Replace('.', ','));
            if (token.ToLower() == "true" || token.ToLower() == "false")
                return bool.Parse(token);

            return long.Parse(token);
        }

        private readonly StringBuilder ToStringBuilder = new StringBuilder();
        public override string ToString()
        {
            ToStringBuilder.Clear();
            ToStringBuilder.Append('[');
            for (int i = 0; i < array.Length; i++)
            {
                Type type = array[i].GetType();
                if (type.Equals(typeof(string)))
                {
                    ToStringBuilder.Append("\"" + array[i] + "\"");
                }
                else
                {
                    if(type.Name.ToLower()[0] == 'u')
                        ToStringBuilder.Append(array[i].ToString().Replace(",", ".").Insert(0, "u"));
                    else
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
