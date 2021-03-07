using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware
{
    public class KLINToken
    {
        public string Property { get; set; }
        public object value { get; set; }

        public override string ToString()
        {
            return value.ToString();
        }
    }
}
