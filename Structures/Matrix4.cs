using System;

namespace ABSoftware.Structures
{
    public class Matrix4
    {
        public float[,] m = new float[4, 4];

        public Vector3 MultiplyVector(Vector3 a)
        {
            Vector3 v = new Vector3();
            v.x = a.x * m[0, 0] + a.y * m[1, 0] + a.z * m[2, 0] + a.w * m[3, 0];
            v.y = a.x * m[0, 1] + a.y * m[1, 1] + a.z * m[2, 1] + a.w * m[3, 1];
            v.z = a.x * m[0, 2] + a.y * m[1, 2] + a.z * m[2, 2] + a.w * m[3, 2];
            v.w = a.x * m[0, 3] + a.y * m[1, 3] + a.z * m[2, 3] + a.w * m[3, 3];
            return v;
        }

        public void MakeIdentity()
        {
            m[0, 0] = 1f;
            m[1, 1] = 1f;
            m[2, 2] = 1f;
            m[3, 3] = 1f;
        }

        public void RotationX(float RadAngle)
        {
            m[0, 0] = 1f;
            m[1, 1] = (float)Math.Cos(RadAngle);
            m[1, 2] = (float)Math.Sin(RadAngle);
            m[2, 1] = (float)-Math.Sin(RadAngle);
            m[2, 2] = (float)Math.Cos(RadAngle);
            m[3, 3] = 1f;
        }

        public void RotationY(float RadAngle)
        {
            m[0, 0] = (float)Math.Cos(RadAngle);
            m[2, 0] = (float)Math.Sin(RadAngle);
            m[1, 1] = 1f;
            m[0, 2] = (float)-Math.Sin(RadAngle);
            m[2, 2] = (float)Math.Cos(RadAngle);
            m[3, 3] = 1f;
        }

        public void RotationZ(float RadAngle)
        {
            m[0, 0] = (float)Math.Cos(RadAngle);
            m[0, 1] = (float)Math.Sin(RadAngle);
            m[1, 0] = (float)-Math.Sin(RadAngle);
            m[1, 1] = (float)Math.Cos(RadAngle);
            m[2, 2] = 1f;
            m[3, 3] = 1f;
        }

        public void Translation(float x, float y, float z)
        {
            m[0, 0] = 1.0f;
            m[1, 1] = 1.0f;
            m[2, 2] = 1.0f;
            m[3, 3] = 1.0f;
            m[3, 0] = x;
            m[3, 1] = y;
            m[3, 2] = z;
        }

        public void Projection(float fov, float aspectRatio, float near, float far)
        {
            float fovRad = 1.0f / (float)Math.Tan(fov * 0.5f / 180.0f * Math.PI);
            m[0, 0] = aspectRatio * fovRad;
            m[1, 1] = fovRad;
            m[2, 2] = far / (far - near);
            m[3, 2] = (-far * near) / (far - near);
            m[2, 3] = 1.0f;
            m[3, 3] = 0.0f;
        }

        public Matrix4 MultiplyMatrix(Matrix4 a, Matrix4 b)
        {
            Matrix4 newMat = new Matrix4();
            for(int c = 0; c < 4; c++)
                for (int r = 0; r < 4; r++)
                    newMat.m[r, c] = a.m[r, 0] * b.m[0, c] + a.m[r, 1] * b.m[1, c] + a.m[r, 2] * b.m[2, c] + a.m[r, 3] * b.m[3, c];
            return newMat;
        }

        public Vector3 MultiplyMatrixVector(Vector3 i)
        {
            Vector3 o = new Vector3();
            o.x = i.x * m[0, 0] + i.y * m[1, 0] + i.z * m[2, 0] + m[3, 0];
            o.y = i.x * m[0, 1] + i.y * m[1, 1] + i.z * m[2, 1] + m[3, 1];
            o.z = i.x * m[0, 2] + i.y * m[1, 2] + i.z * m[2, 2] + m[3, 2];
            float w = i.x * m[0, 3] + i.y * m[1, 3] + i.z * m[2, 3] + m[3, 3];

            if (w != 0.0f)
            {
                o.x /= w; o.y /= w; o.z /= w;
            }

            return o;
        }
    }
}
