using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace ABSoftware.SafeVariables
{
    public class Safe
    {
        public string name;
        public string type;

        public object lastValue;
        
        public void OnDetect()
        {
            Console.WriteLine("Detected Cheat!");
            Application.Exit();
        }

        public virtual void OnSet(object value)
        {

        }
    }
}
