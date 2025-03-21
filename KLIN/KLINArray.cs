﻿using System;
using System.Text;

namespace ABSoftware
{
    public class KLINArray
    {
        object[] array;

        public int Capacity
        {
            get { return array.Length; }
            set
            {
                object[] newArray = new object[value];
                Array.Copy(array, 0, newArray, 0, Size);
                array = newArray;
            }
        }

        void ControlCapacity(int minCapacity)
        {
            if (this.array.Length < minCapacity)
            {
                int newCapacity = (array.Length == 0) ? 4 : (array.Length * 2);

                if (newCapacity > int.MaxValue - 8)
                    newCapacity = int.MaxValue - 8;
                if (newCapacity < minCapacity)
                    newCapacity = minCapacity;

                this.Capacity = newCapacity;
            }
        }

        public int Size { get; private set; }

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
            if (Size == array.Length)
                ControlCapacity(Size + 1);
            this.array[Size] = obj;
            Size++;
        }

        public object Get(int id)
        {
            if (id >= Size)
                return null;

            return this.array[id];
        }

        public void RemoveAt(int id)
        {
            Array.Copy(this.array, id + 1, this.array, id, this.Size - id - 1);
            Size--;
        }

        public void Clear()
        {
            this.array = new object[0];
            Size = 0;
            ControlCapacity(0);
        }

        private readonly StringBuilder ParseBuilder = new StringBuilder();
        public void Parse(string KLIN)
        {
            Clear();
            if (KLIN.Length > 0 && KLIN[0] != '[')
                return;
            bool readingToken = true;
            bool isInQuotes = false;

            ParseBuilder.Clear();

            for (int i = 1; i < KLIN.Length; i++)
            {
                if (KLIN[i] == ']')
                {
                    if (ParseBuilder.Length > 0)
                    {
                        if (isInQuotes)
                            Add(ParseBuilder.ToString());
                        else
                            Add(GetTokenType(ParseBuilder.ToString()));
                    }
                    break;
                }

                if (!isInQuotes && KLIN[i] == ' ')
                    continue;

                if (KLIN[i] == '"')
                {
                    if (isInQuotes && KLIN[i + 1] == ',')
                    {
                        Add(ParseBuilder.ToString());
                        ParseBuilder.Clear();
                        isInQuotes = false;
                        readingToken = true;
                    }
                    else
                        isInQuotes = true;
                    continue;
                }

                if (!readingToken && KLIN[i] != ',' && KLIN[i] != ' ')
                    readingToken = true;

                if (!isInQuotes && KLIN[i] == ',')
                {
                    readingToken = false;
                }

                if (readingToken)
                {
                    ParseBuilder.Append(KLIN[i]);
                }
                else if (ParseBuilder.Length > 0)
                {
                    Add(GetTokenType(ParseBuilder.ToString()));
                    ParseBuilder.Clear();
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
                    if (type.Name.ToLower()[0] == 'u')
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