using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware.SafeVariables
{
    public class SafeFloat : Safe
    {
        public float Value;
        public float value { get { OnRead(); return Value; } set { OnSet(value); } }

        public static SafeFloat instance;

        public SafeFloat(float value, string name)
        {
            this.name = name;
            this.type = "Int";
            this.lastValue = value;
            this.Value = value;
            instance = this;
        }

        public void OnRead()
        {
            if (this.Value != (float)lastValue)
            {
                OnDetect();
            }
        }

        public override void OnSet(object value)
        {
            if (this.Value != (float)lastValue)
            {
                OnDetect();
            }
            this.Value = (float)value;
            lastValue = value;
        }

        public override string ToString()
        {
            return value.ToString();
        }

        public static implicit operator float(SafeFloat si)
        {
            return instance.value;
        }

        public static implicit operator SafeFloat(float integer)
        {
            instance.value = integer;
            return instance;
        }
    }
}
