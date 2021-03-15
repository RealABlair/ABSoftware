using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware.Engine.Structs
{
    public class Vector2
    {
        public static Vector2 Zero = new Vector2(0, 0);

        public int x, y;

        public Vector2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public float Distance(Vector2 a, Vector2 b)
        {
            float x = Math.Max(a.x, b.x) - Math.Min(a.x, a.x);
            float y = Math.Max(a.y, b.y) - Math.Min(a.y, a.y);

            return (float)Math.Sqrt((x * x) + (y * y));
        }
    }
}
