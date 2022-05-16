using System;
using System.Collections.Generic;

namespace ABSoftware
{
    public class KLIN
    {
        const string v = "1.0b";
        List<KLINToken> tokens = new List<KLINToken>();

        public KLIN()
        {

        }

        public void Add(string PropertyName, object Value)
        {
            tokens.Add(new KLINToken(PropertyName, Value));
        }

        public void Parse(string klin)
        {
            if (klin.Length < 3)
                return;
            string[] Lines = klin.Split('\n');
            for (int i = 0; i < Lines.Length; i++)
            {
                if (Lines[i][0] != '#') //# MEANS COMMENT
                {
                    KLINToken kt = new KLINToken(Lines[i].Split('=')[0], Lines[i].Split(new char[] { '=' }, 2)[1]);
                    tokens.Add(kt);
                }
            }
        }

        public object this[string PropertyName]
        {
            get { return Get(PropertyName); }
        }

        public object Get(string PropertyName)
        {
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].PropertyName == PropertyName)
                {
                    return tokens[i].Value;
                }
            }
            return null;
        }

        public string[] GetProperties()
        {
            string[] s = new string[tokens.Count];
            for (int i = 0; i < s.Length; i++)
            {
                s[i] = tokens[i].PropertyName;
            }
            return s;
        }

        public override string ToString()
        {
            string klin = $"#KLIN version {v}\n";
            for (int i = 0; i < tokens.Count; i++)
            {
                klin += $"{tokens[i].PropertyName}={tokens[i].Value}\n";
            }
            klin += "#END";
            return klin;
        }

        public void Dispose()
        {
            tokens.Clear();
        }
    }
}
