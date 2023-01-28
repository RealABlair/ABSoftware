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

        public static bool Intersection(Vector2 a0, Vector2 a1, Vector2 b0, Vector2 b1, out Vector2 intersectionPoint)
        {
            Vector2 r = (a1 - a0);
            Vector2 s = (b1 - b0);

            float d = r.x * s.y - r.y * s.x;
            float u = ((b0.x - a0.x) * r.y - (b0.y - a0.y) * r.x) / d;
            float t = ((b0.x - a0.x) * s.y - (b0.y - a0.y) * s.x) / d;

            if (u >= 0f && u <= 1f && t >= 0f && t <= 1f)
            {
                intersectionPoint = a0 + (r * t);
                return true;
            }

            intersectionPoint = Vector2.Zero;
            return false;
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
