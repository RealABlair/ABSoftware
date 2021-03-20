using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware.SafeVariables
{
    public class SafeLong : Safe
    {
        public long Value;
        public long value { get { OnRead(); return Value; } set { OnSet(value); } }

        public static SafeLong instance;

        public SafeLong(long value, string name)
        {
            this.name = name;
            this.type = "Int";
            this.lastValue = value;
            this.Value = value;
            instance = this;
        }

        public void OnRead()
        {
            if (this.Value != (long)lastValue)
            {
                OnDetect();
            }
        }

        public override void OnSet(object value)
        {
            if (this.Value != (long)lastValue)
            {
                OnDetect();
            }
            this.Value = (long)value;
            lastValue = value;
        }

        public override string ToString()
        {
            return value.ToString();
        }

        public static implicit operator long(SafeLong si)
        {
            return instance.value;
        }

        public static implicit operator SafeLong(long integer)
        {
            instance.value = integer;
            return instance;
        }
    }
}
