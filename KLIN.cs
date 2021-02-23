using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPGTBot.ABSoftware
{
    public class KLIN
    {
        const string v = "1.0b";
        string KLCODE;
        List<KLINToken> tokens = new List<KLINToken>();

        public KLIN()
        {

        }

        public void Add(string Property, object value)
        {
            KLINToken kt = new KLINToken();
            kt.Property = Property;
            kt.value = value;
            tokens.Add(kt);
        }

        public void Parse(string KLIN)
        {
            KLCODE = KLIN;
            string[] Lines = KLIN.Split('\n');
            for(int i = 0; i < Lines.Length; i++)
            {
                if(Lines[i][0] != '#') //# MEANS COMMENT
                {
                    KLINToken kt = new KLINToken();
                    kt.Property = Lines[i].Split('=')[0];
                    kt.value = Lines[i].Split(new char[] { '=' }, 2)[1];
                    tokens.Add(kt);
                }
            }
        }

        public object Get(string Property)
        {
            for(int i = 0; i < tokens.Count; i++)
            {
                if(tokens[i].Property == Property)
                {
                    return tokens[i].value;
                }
            }
            return null;
        }

        public override string ToString()
        {
            string KLIN = $"#KLIN version {v}\n";
            for(int i = 0; i < tokens.Count; i++)
            {
                KLIN += $"{tokens[i].Property}={tokens[i].value}\n";
            }
            KLIN += "#END";
            return KLIN;
        }
    }
}
