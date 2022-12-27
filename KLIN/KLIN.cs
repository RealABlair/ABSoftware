using System;
using System.Text;

namespace ABSoftware
{
    public class KLIN
    {
        public static readonly string VERSION = "1.1";

        KLINToken[] tokens;

        public int Size { get { return tokens.Length; } }

        public KLIN()
        {
            this.tokens = new KLINToken[0];
        }

        public KLIN(KLINToken[] tokens)
        {
            this.tokens = tokens;
        }

        public KLINToken[] GetTokens() 
        {
            return this.tokens;
        }

        public KLINToken this[int PropertyId]
        {
            get { if (PropertyId < Size) return tokens[PropertyId]; else return null; }
            set { if (PropertyId < Size) this.tokens[PropertyId] = value; else { Array.Resize(ref this.tokens, PropertyId+1); this.tokens[PropertyId] = value; } }
        }

        public KLINToken this[string PropertyName]
        {
            get { if (PropertyExists(PropertyName)) return GetToken(PropertyName); else { AddProperty(PropertyName, null); return GetToken(PropertyName); } }
            set 
            { 
                if (PropertyExists(PropertyName))
                    for (int i = 0; i < Size; i++)
                    {
                        if (this.tokens[i].PropertyName.Equals(PropertyName)) this.tokens[i] = value;
                    }
                else
                { 
                    AddProperty(value);
                }
            }
        }

        public void AddProperty(string PropertyName, object PropertyObject)
        {
            Array.Resize(ref this.tokens, Size + 1);
            this.tokens[Size - 1] = new KLINToken(PropertyName, PropertyObject);
        }

        public void AddProperty(KLINToken token)
        {
            Array.Resize(ref this.tokens, Size + 1);
            this.tokens[Size - 1] = token;
        }

        public object GetProperty(string PropertyName)
        {
            for (int i = 0; i < Size; i++) if (this.tokens[i].PropertyName == PropertyName) return this.tokens[i].PropertyObject;
            return null;
        }

        public object GetProperty(int PropertyId)
        {
            if (PropertyId < Size)
                return this.tokens[PropertyId].PropertyObject;
            return null;
        }

        public KLINToken GetToken(string PropertyName)
        {
            for (int i = 0; i < Size; i++) if (this.tokens[i].PropertyName == PropertyName) return this.tokens[i];
            return null;
        }

        public KLINToken GetToken(int PropertyId)
        {
            if (PropertyId < Size)
                return this.tokens[PropertyId];
            return null;
        }

        public bool PropertyExists(string PropertyName)
        {
            for (int i = 0; i < Size; i++) if (this.tokens[i].PropertyName.Equals(PropertyName)) return true;
            return false;
        }

        public KLIN Copy()
        {
            return new KLIN(tokens);
        }

        public void Parse(string KLIN)
        {
            string[] Lines = KLIN.Split('\n');

            string[] TrimLines(string[] lines)
            {
                string[] newLines = new string[0];
                for (int i = 0; i < lines.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(lines[i]) || string.IsNullOrEmpty(lines[i]))
                        continue;

                    Array.Resize(ref newLines, newLines.Length + 1);
                    newLines[newLines.Length - 1] = lines[i];
                }
                return newLines;
            }

            Lines = TrimLines(Lines);

            if (!(Lines[0][0] == '(' && Lines[Lines.Length-1][0] == ')'))
                return;
            bool subTokens = false;
            KLINToken lastToken = null;
            KLINToken parent = null;
            for (int i = 1; i < Lines.Length - 1; i++)
            {
                string Line = Lines[i].Split(new char[] { '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)[0];
                if (Line.Length < 1 || Line.Equals(string.Empty))
                    continue;
                if (Line[0] == '#')
                {
                    AddProperty(Line, null);
                }
                else
                {
                    if (Line[0] == '(') { parent = lastToken; subTokens = true; continue; }
                    else if (Line[0] == ')') { subTokens = false; continue; }

                    if (!subTokens)
                    {
                        string[] split = Line.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                        lastToken = new KLINToken(split[0], (split.Length > 1) ? split[1] : null);
                        AddProperty(lastToken);
                    }
                    else if(subTokens)
                    {
                        string[] split = Line.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                        lastToken = new KLINToken(split[0], (split.Length > 1) ? split[1] : null);
                        parent.AddChild(lastToken);
                    }
                }
            }
        }


        private readonly StringBuilder ToStringBuilder = new StringBuilder();
        public override string ToString()
        {
            ToStringBuilder.Clear();
            int indent = 1;
            ToStringBuilder.AppendLine("(");
            for(int i = 0; i < Size; i++)
            {
                KLINToken token = this.tokens[i];
                ToStringBuilder.Append(WriteHelper(ref token, ref indent));
            }
            ToStringBuilder.Append(")");
            return ToStringBuilder.ToString();
        }

        private string WriteHelper(ref KLINToken token, ref int indent)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Indent(indent) + token.TokenString);
            if(token.HasChildren)
            {
                string currentIndent = Indent(indent);
                sb.AppendLine(currentIndent + "(");
                indent += 1;
                for (int i = 0; i < token.Children.Length; i++)
                {
                    sb.Append(WriteHelper(ref token.Children[i], ref indent));
                }
                indent -= 1;
                sb.AppendLine(currentIndent + ")");
            }
            return sb.ToString();
        }

        private string Indent(int count)
        {
            return "".PadLeft(count, '\t');
        }
    }
}