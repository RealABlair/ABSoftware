using System;

namespace ABSoftware.Structures
{
    public struct Vector3
    {
        public static Vector3 Zero { get { return new Vector3(); } }
        public static Vector3 One { get { return new Vector3(1f, 1f, 1f); } }
        public static Vector3 Right { get { return new Vector3(1f); } }
        public static Vector3 Up { get { return new Vector3(0f, 1f); } }
        public static Vector3 Forward { get { return new Vector3(0f, 0f, 1f); } }

        public float x;
        public float y;
        public float z;
        public float w;

        public Vector3(float x = 0f, float y = 0f, float z = 0f, float w = 1f)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public float Distance(Vector3 to)
        {
            float x = Math.Max(this.x, to.x) - Math.Min(this.x, to.x);
            float y = Math.Max(this.y, to.y) - Math.Min(this.y, to.y);
            float z = Math.Max(this.z, to.z) - Math.Min(this.z, to.z);

            return (float)Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2));
        }

        public void Cross(Vector3 a, Vector3 b)
        {
            this.x = a.y * b.z - a.z * b.y;
            this.y = a.z * b.x - a.x * b.z;
            this.z = a.x * b.y - a.y * b.x;
        }

        public void MoveHorizontally(float radians, float distance)
        {
            this.x = (float)Math.Sin(radians) * distance;
            this.z = (float)Math.Cos(radians) * distance;
        }

        public float DotProduct(Vector3 a, Vector3 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public float Length()
        {
            return (float)Math.Sqrt(x * x + y * y + z * z);
        }

        public void Normalize()
        {
            float l = Length();

            x /= l;
            y /= l;
            z /= l;
        }

        public void Sub(Vector3 a, Vector3 b)
        {
            x = a.x - b.x;
            y = a.y - b.y;
            z = a.z - b.z;
        }

        public void Add(Vector3 a, Vector3 b)
        {
            x = a.x + b.x;
            y = a.y + b.y;
            z = a.z + b.z;
        }

        public void Div(Vector3 a, float b)
        {
            x = a.x / b;
            y = a.y / b;
            z = a.z / b;
        }

        public void Mul(Vector3 a, float b)
        {
            x = a.x * b;
            y = a.y * b;
            z = a.z * b;
        }

        #region Operators
        #region Vectors
        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vector3 operator *(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public static Vector3 operator /(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
        }

        public static Vector3 operator %(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x % b.x, a.y % b.y, a.z % b.z);
        }

        public static bool operator >(Vector3 a, Vector3 b)
        {
            return (a.x > b.x && a.y > b.y && a.z > b.z);
        }

        public static bool operator <(Vector3 a, Vector3 b)
        {
            return (a.x < b.x && a.y < b.y && a.z < b.z);
        }

        public static bool operator >=(Vector3 a, Vector3 b)
        {
            return (a.x >= b.x && a.y >= b.y && a.z >= b.z);
        }

        public static bool operator <=(Vector3 a, Vector3 b)
        {
            return (a.x <= b.x && a.y <= b.y && a.z <= b.z);
        }
        #endregion

        #region Floats
        public static Vector3 operator +(Vector3 a, float b)
        {
            return new Vector3(a.x + b, a.y + b, a.z + b);
        }

        public static Vector3 operator -(Vector3 a, float b)
        {
            return new Vector3(a.x - b, a.y - b, a.z - b);
        }

        public static Vector3 operator *(Vector3 a, float b)
        {
            return new Vector3(a.x * b, a.y * b, a.z * b);
        }

        public static Vector3 operator /(Vector3 a, float b)
        {
            return new Vector3(a.x / b, a.y / b, a.z / b);
        }

        public static Vector3 operator %(Vector3 a, float b)
        {
            return new Vector3(a.x % b, a.y % b, a.z % b);
        }

        public static bool operator >(Vector3 a, float b)
        {
            return (a.x > b && a.y > b && a.z > b);
        }

        public static bool operator <(Vector3 a, float b)
        {
            return (a.x < b && a.y < b && a.z < b);
        }

        public static bool operator >=(Vector3 a, float b)
        {
            return (a.x >= b && a.y >= b && a.z >= b);
        }

        public static bool operator <=(Vector3 a, float b)
        {
            return (a.x <= b && a.y <= b && a.z <= b);
        }

        public static Vector3 operator +(float a, Vector3 b)
        {
            return new Vector3(a + b.x, a + b.y, a + b.z);
        }

        public static Vector3 operator -(float a, Vector3 b)
        {
            return new Vector3(a - b.x, a - b.y, a - b.z);
        }

        public static Vector3 operator *(float a, Vector3 b)
        {
            return new Vector3(a * b.x, a * b.y, a * b.z);
        }

        public static Vector3 operator /(float a, Vector3 b)
        {
            return new Vector3(a / b.x, a / b.y, a / b.z);
        }

        public static Vector3 operator %(float a, Vector3 b)
        {
            return new Vector3(a % b.x, a % b.y, a % b.z);
        }

        public static bool operator >(float a, Vector3 b)
        {
            return (a > b.x && a > b.y && a > b.z);
        }

        public static bool operator <(float a, Vector3 b)
        {
            return (a < b.x && a < b.y && a < b.z);
        }

        public static bool operator >=(float a, Vector3 b)
        {
            return (a >= b.x && a >= b.y && a >= b.z);
        }

        public static bool operator <=(float a, Vector3 b)
        {
            return (a <= b.x && a <= b.y && a <= b.z);
        }
        #endregion
        #endregion

        public override string ToString()
        {
            return "x:" + x + ", " + "y:" + y + ", " + "z:" + z;
        }
    }
}
