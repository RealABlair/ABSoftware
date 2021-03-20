using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware.SafeVariables
{
    public class SafeDouble : Safe
    {
        public double Value;
        public double value { get { OnRead(); return Value; } set { OnSet(value); } }

        public static SafeDouble instance;

        public SafeDouble(double value, string name)
        {
            this.name = name;
            this.type = "Int";
            this.lastValue = value;
            this.Value = value;
            instance = this;
        }

        public void OnRead()
        {
            if (this.Value != (double)lastValue)
            {
                OnDetect();
            }
        }

        public override void OnSet(object value)
        {
            if (this.Value != (double)lastValue)
            {
                OnDetect();
            }
            this.Value = (double)value;
            lastValue = value;
        }

        public override string ToString()
        {
            return value.ToString();
        }

        public static implicit operator double(SafeDouble si)
        {
            return instance.value;
        }

        public static implicit operator SafeDouble(double integer)
        {
            instance.value = integer;
            return instance;
        }
    }
}
