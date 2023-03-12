using System;

namespace ABSoftware.Structures
{
    public struct Vector2
    {
        public static Vector2 Zero { get { return new Vector2(); } }
        public static Vector2 One { get { return new Vector2(1f, 1f); } }
        public static Vector2 Right { get { return new Vector2(1f); } }
        public static Vector2 Up { get { return new Vector2(0f, 1f); } }

        public float x, y;

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

        public static float Dot(Vector2 a, Vector2 b)
        {
            return a.x * b.x + a.y * b.y;
        }

        public static Vector2 Reflect(Vector2 direction, Vector2 normal)
        {
            return (new Vector2(normal.x, normal.y) * (-2f * Dot(normal, direction))) + direction;
        }

        public static Vector2 GetNormal(Vector2 a, Vector2 b, bool flipNormals = false)
        {
            Vector2 d = (b - a).Normalize();

            if (!flipNormals)
                return new Vector2(-d.y, d.x);
            else
                return new Vector2(d.y, -d.x);
        }

        public static bool Intersection(Vector2 start1, Vector2 end1, Vector2 start2, Vector2 end2, out Vector2 intersectionPoint)
        {
            Vector2 r = (end1 - start1);
            Vector2 s = (end2 - start2);

            float d = r.x * s.y - r.y * s.x;
            float u = ((start2.x - start1.x) * r.y - (start2.y - start1.y) * r.x) / d;
            float t = ((start2.x - start1.x) * s.y - (start2.y - start1.y) * s.x) / d;

            if (u >= 0f && u <= 1f && t >= 0f && t <= 1f)
            {
                intersectionPoint = start1 + (r * t);
                return true;
            }

            intersectionPoint = Vector2.Zero;
            return false;
        }

        public static Vector2 Lerp(Vector2 from, Vector2 to, float t)
        {
            return from * (1f - t) + to * t;
        }

        public Vector2 GetRotations(Vector2 to)
        {
            float sin = (to - this).x / Distance(to);
            float cos = (to - this).y / Distance(to);

            return new Vector2(sin, cos).Normalize();
        }

        public void Floor()
        {
            x = (int)x;
            y = (int)y;
        }

        public void Round()
        {
            x += 0.5f;
            y += 0.5f;
            Floor();
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

        public static Vector2 operator +(float a, Vector2 b)
        {
            return new Vector2(a + b.x, a + b.y);
        }

        public static Vector2 operator -(float a, Vector2 b)
        {
            return new Vector2(a - b.x, a - b.y);
        }

        public static Vector2 operator *(float a, Vector2 b)
        {
            return new Vector2(a * b.x, a * b.y);
        }

        public static Vector2 operator /(float a, Vector2 b)
        {
            return new Vector2(a / b.x, a / b.y);
        }

        public static Vector2 operator %(float a, Vector2 b)
        {
            return new Vector2(a % b.x, a % b.y);
        }

        public static bool operator >(float a, Vector2 b)
        {
            return (a > b.x && a > b.y);
        }

        public static bool operator <(float a, Vector2 b)
        {
            return (a < b.x && a < b.y);
        }

        public static bool operator >=(float a, Vector2 b)
        {
            return (a >= b.x && a >= b.y);
        }

        public static bool operator <=(float a, Vector2 b)
        {
            return (a <= b.x && a <= b.y);
        }
        #endregion
        #endregion

        public override string ToString()
        {
            return "(" + this.x + ", " + this.y + ")";
        }
    }
}
