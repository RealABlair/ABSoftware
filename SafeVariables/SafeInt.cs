using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware.SafeVariables
{
    public class SafeInt : Safe
    {
        public int Value;
        public int value { get { OnRead(); return Value; } set { OnSet(value); } }

        public static SafeInt instance;

        public SafeInt(int value, string name)
        {
            this.name = name;
            this.type = "Int";
            this.lastValue = value;
            this.Value = value;
            instance = this;
        }

        public void OnRead()
        {
            if (this.Value != (int)lastValue)
            {
                OnDetect();
            }
        }

        public override void OnSet(object value)
        {
            if (this.Value != (int)lastValue)
            {
                OnDetect();
            }
            this.Value = (int)value;
            lastValue = value;
        }

        public override string ToString()
        {
            return value.ToString();
        }

        public static implicit operator int(SafeInt si)
        {
            return instance.value;
        }

        public static implicit operator SafeInt(int integer)
        {
            instance.value = integer;
            return instance;
        }
    }
}
