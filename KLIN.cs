using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware
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

        public class Convertation
        {
            //CONVERT↓

            public static string GetValue(string[] array)
            {
                string ret = "[\"";
                for(int i = 0; i < array.Length; i++)
                {
                    if (i + 1 != array.Length && i + 1 < array.Length)
                    {
                        ret += $"{array[i]}\", \"";
                    }
                    else
                    {
                        ret += $"{array[i]}";
                    }
                }
                ret += "\"]";
                return ret;
            }

            public static string GetValue(int[] array)
            {
                string ret = "[";
                for (int i = 0; i < array.Length; i++)
                {
                    if (i + 1 != array.Length && i + 1 < array.Length)
                    {
                        ret += $"{array[i]}, ";
                    }
                    else
                    {
                        ret += $"{array[i]}";
                    }
                }
                ret += "]";
                return ret;
            }

            //GET↓

            public static string[] GetStrings(string KLINArray)
            {
                List<string> array = new List<string>();
                for (int i = 1; i < KLINArray.Split(new char[] { '\"', '\"' }, StringSplitOptions.RemoveEmptyEntries).Length; i += 2)
                {
                    array.Add(KLINArray.Split(new char[] { '\"', '\"' }, StringSplitOptions.RemoveEmptyEntries)[i]);
                }
                return array.ToArray();
            }

            public static int[] GetInts(string KLINArray)
            {
                List<int> array = new List<int>();
                int count = KLINArray.Split(',').Length + 1;
                string splits = KLINArray.Split(new char[] { '[', ']' })[1];
                if(count == 1)
                {
                    array.Add(int.Parse(KLINArray.Split(new char[] { '[', ']' })[1]));
                }
                else
                {
                    for(int i = 0; i < count - 1; i++)
                    {
                        array.Add(int.Parse(splits.Split(',')[i]));
                    }
                }
                return array.ToArray();
            }
        }
    }
}
