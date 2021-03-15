using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware.Engine
{
    public class Graphics
    {
        private List<Pixel> pixelUpdateBuffer = new List<Pixel>();

        public Pixel[] ToArray()
        {
            return pixelUpdateBuffer.ToArray();
        }

        public void Add(Pixel p)
        {
            pixelUpdateBuffer.Add(p);
        }

        public void Remove(Pixel p)
        {
            pixelUpdateBuffer.Remove(p);
        }

        public void Remove(int id)
        {
            pixelUpdateBuffer.RemoveAt(id);
        }
    }
}
