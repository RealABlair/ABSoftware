using System;

namespace ABSoftware.Structures
{
    public class Vector2
    {
        public static Vector2 Zero { get { return new Vector2(); } }
        public static Vector2 One { get { return new Vector2(1f, 1f); } }
        public static Vector2 Right { get { return new Vector2(1f); } }
        public static Vector2 Up { get { return new Vector2(0f, 1f); } }

        public float x = 0, y = 0;

        public Vector2(float x = 0f, float y = 0f)
        {
            this.x = x;
            this.y = y;
        }

        public float Length()
        {
            return (float)Math.Sqrt(x * x + y * y);
        }

        public float Distance(Vector2 to)
        {
            return (to - this).Length();
        }

        public Vector2 Normalize()
        {
            float l = Length();
            return new Vector2(x / l, y / l);
        }
        
        public Vector2 Intersection(Vector2 a0, Vector2 a1, Vector2 b0, Vector2 b1)
        {
            float n;
            if (a1.y - a0.y != 0)
            {
                float q = (a1.x - a0.x) / (a0.y - a1.y);
                float sn = (b0.x - b1.x) + (b0.y - b1.y) * q;
                if (sn == 0f)
                    return Zero;
                float fn = (b0.x - a0.x) + (b0.y - a0.x) * q;
                n = fn / sn;
            }
            else
            {
                if (b0.y - b1.y == 0f)
                    return Zero;
                n = (b0.y - a0.y) / (b0.y - b1.y);
            }
            return new Vector2((b0.x + (b1.x - b0.x) * n), ((b0.y + (b1.y - b0.y) * n)));
        }

        public float[] GetRotations(Vector2 to)
        {
            float sin = (to - this).x / Distance(to);
            float cos = (to - this).y / Distance(to);

            return new float[] { sin, cos };
        }

        public void Floor()
        {
            x = (int)x;
            y = (int)y;
        }

        #region Operators
        #region Vectors
        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x + b.x, a.y + b.y);
        }

        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x - b.x, a.y - b.y);
        }

        public static Vector2 operator *(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x * b.x, a.y * b.y);
        }

        public static Vector2 operator /(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x / b.x, a.y / b.y);
        }

        public static Vector2 operator %(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x % b.x, a.y % b.y);
        }

        public static bool operator >(Vector2 a, Vector2 b)
        {
            return (a.x > b.x && a.y > b.y);
        }

        public static bool operator <(Vector2 a, Vector2 b)
        {
            return (a.x < b.x && a.y < b.y);
        }

        public static bool operator >=(Vector2 a, Vector2 b)
        {
            return (a.x >= b.x && a.y >= b.y);
        }

        public static bool operator <=(Vector2 a, Vector2 b)
        {
            return (a.x <= b.x && a.y <= b.y);
        }
        #endregion

        #region Floats
        public static Vector2 operator +(Vector2 a, float b)
        {
            return new Vector2(a.x + b, a.y + b);
        }

        public static Vector2 operator -(Vector2 a, float b)
        {
            return new Vector2(a.x - b, a.y - b);
        }

        public static Vector2 operator *(Vector2 a, float b)
        {
            return new Vector2(a.x * b, a.y * b);
        }

        public static Vector2 operator /(Vector2 a, float b)
        {
            return new Vector2(a.x / b, a.y / b);
        }

        public static Vector2 operator %(Vector2 a, float b)
        {
            return new Vector2(a.x % b, a.y % b);
        }

        public static bool operator >(Vector2 a, float b)
        {
            return (a.x > b && a.y > b);
        }

        public static bool operator <(Vector2 a, float b)
        {
            return (a.x < b && a.y < b);
        }

        public static bool operator >=(Vector2 a, float b)
        {
            return (a.x >= b && a.y >= b);
        }

        public static bool operator <=(Vector2 a, float b)
        {
            return (a.x <= b && a.y <= b);
        }
        #endregion
        #endregion

        public override string ToString()
        {
            return "(" + this.x + ", " + this.y + ")";
        }
    }
}
