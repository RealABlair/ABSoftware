using System;

namespace ABSoftware
{
    public class KLINToken
    {
        public string PropertyName { get; set; }
        public object Value { get; set; }

        public KLINToken(string PropertyName = null, object Value = null)
        {
            this.PropertyName = PropertyName;
            this.Value = Value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
