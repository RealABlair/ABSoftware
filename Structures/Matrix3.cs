using System;

namespace ABSoftware.Structures
{
    public class Matrix3
    {
        public float[,] m = new float[3, 3];

        public Matrix3()
        {

        }

        public Matrix3(float[,] m)
        {
            int Rows = m.GetLength(1);
            int Columns = m.GetLength(0);
            float[,] editedMatrix = new float[Rows, Columns];
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    editedMatrix[r, c] = m[c, r];
                }
            }
            this.m = editedMatrix;
        }

        public void Identity()
        {
            m[0, 0] = 1.0f; m[1, 0] = 0.0f; m[2, 0] = 0.0f;
            m[0, 1] = 0.0f; m[1, 1] = 1.0f; m[2, 1] = 0.0f;
            m[0, 2] = 0.0f; m[1, 2] = 0.0f; m[2, 2] = 1.0f;
        }

        public void Translate(float x, float y)
        {
            m[0, 0] = 1.0f; m[1, 0] = 0.0f; m[2, 0] = x;
            m[0, 1] = 0.0f; m[1, 1] = 1.0f; m[2, 1] = y;
            m[0, 2] = 0.0f; m[1, 2] = 0.0f; m[2, 2] = 1.0f;
        }

        public void Rotate(float RadAngle)
        {
            m[0, 0] = (float)Math.Cos(RadAngle); m[1, 0] = (float)Math.Sin(RadAngle); m[2, 0] = 0.0f;
            m[0, 1] = -(float)Math.Sin(RadAngle); m[1, 1] = (float)Math.Cos(RadAngle); m[2, 1] = 0.0f;
            m[0, 2] = 0.0f; m[1, 2] = 0.0f; m[2, 2] = 1.0f;
        }

        public void Scale(float x, float y)
        {
            m[0, 0] = x; m[1, 0] = 0.0f; m[2, 0] = 0.0f;
            m[0, 1] = 0.0f; m[1, 1] = y; m[2, 1] = 0.0f;
            m[0, 2] = 0.0f; m[1, 2] = 0.0f; m[2, 2] = 1.0f;
        }

        public void Shear(float x, float y)
        {
            m[0, 0] = 1.0f; m[1, 0] = x; m[2, 0] = 0.0f;
            m[0, 1] = y; m[1, 1] = 1.0f; m[2, 1] = 0.0f;
            m[0, 2] = 0.0f; m[1, 2] = 0.0f; m[2, 2] = 1.0f;
        }

        public Matrix3 Identity_M()
        {
            Matrix3 ma = new Matrix3();
            ma.Identity();
            return ma;
        }

        public Matrix3 Translate_M(float x, float y)
        {
            Matrix3 ma = new Matrix3();
            ma.Translate(x, y);
            return ma;
        }

        public Matrix3 Rotate_M(float RadAngle)
        {
            Matrix3 ma = new Matrix3();
            ma.Rotate(RadAngle);
            return ma;
        }

        public Matrix3 Scale_M(float x, float y)
        {
            Matrix3 ma = new Matrix3();
            ma.Scale(x, y);
            return ma;
        }

        public Matrix3 Shear_M(float x, float y)
        {
            Matrix3 ma = new Matrix3();
            ma.Shear(x, y);
            return ma;
        }

        public void MultiplyMatrix(Matrix3 b)
        {
            float[,] newM = new float[3, 3];
            for (int c = 0; c < 3; c++)
                for (int r = 0; r < 3; r++)
                    newM[c, r] = m[0, r] * b.m[c, 0] + m[1, r] * b.m[c, 1] + m[2, r] * b.m[c, 2];
            m = newM;
        }

        public static Matrix3 MultiplyMatrix(Matrix3 a, Matrix3 b)
        {
            Matrix3 newMat = new Matrix3();
            for (int c = 0; c < 3; c++)
                for (int r = 0; r < 3; r++)
                    newMat.m[c, r] = a.m[0, r] * b.m[c, 0] + a.m[1, r] * b.m[c, 1] + a.m[2, r] * b.m[c, 2];
            return newMat;
        }

        public void Forward(float ix, float iy, out float ox, out float oy)
        {
            ox = ix * m[0, 0] + iy * m[1, 0] + m[2, 0];
            oy = ix * m[0, 1] + iy * m[1, 1] + m[2, 1];
        }

        public Matrix3 Invert()
        {
            Matrix3 matOut = new Matrix3();
            float det = m[0, 0] * (m[1, 1] * m[2, 2] - m[1, 2] * m[2, 1]) -
            m[1, 0] * (m[0, 1] * m[2, 2] - m[2, 1] * m[0, 2]) +
            m[2, 0] * (m[0, 1] * m[1, 2] - m[1, 1] * m[0, 2]);

            float idet = 1.0f / det;
            matOut.m[0, 0] = (m[1, 1] * m[2, 2] - m[1, 2] * m[2, 1]) * idet;
            matOut.m[1, 0] = (m[2, 0] * m[1, 2] - m[1, 0] * m[2, 2]) * idet;
            matOut.m[2, 0] = (m[1, 0] * m[2, 1] - m[2, 0] * m[1, 1]) * idet;
            matOut.m[0, 1] = (m[2, 1] * m[0, 2] - m[0, 1] * m[2, 2]) * idet;
            matOut.m[1, 1] = (m[0, 0] * m[2, 2] - m[2, 0] * m[0, 2]) * idet;
            matOut.m[2, 1] = (m[0, 1] * m[2, 0] - m[0, 0] * m[2, 1]) * idet;
            matOut.m[0, 2] = (m[0, 1] * m[1, 2] - m[0, 2] * m[1, 1]) * idet;
            matOut.m[1, 2] = (m[0, 2] * m[1, 0] - m[0, 0] * m[1, 2]) * idet;
            matOut.m[2, 2] = (m[0, 0] * m[1, 1] - m[0, 1] * m[1, 0]) * idet;
            return matOut;
        }

        public bool Similar(Matrix3 matrix)
        {
            for (int c = 0; c < 3; c++)
                for (int r = 0; r < 3; r++)
                    if (m[c, r] != matrix.m[c, r])
                        return false;

            return true;
        }

        #region Operators
        public static Matrix3 operator +(Matrix3 a, Matrix3 b)
        {
            float[,] m = new float[3, 3];
            m[0, 0] = a.m[0, 0] + b.m[0, 0]; m[1, 0] = a.m[1, 0] + b.m[1, 0]; m[2, 0] = a.m[2, 0] + b.m[2, 0];
            m[0, 1] = a.m[0, 1] + b.m[0, 1]; m[1, 1] = a.m[1, 1] + b.m[1, 1]; m[2, 1] = a.m[2, 1] + b.m[2, 1];
            m[0, 2] = a.m[0, 2] + b.m[0, 2]; m[1, 2] = a.m[1, 2] + b.m[1, 2]; m[2, 2] = a.m[2, 2] + b.m[2, 2];
            return new Matrix3(m);
        }

        public static Matrix3 operator -(Matrix3 a, Matrix3 b)
        {
            float[,] m = new float[3, 3];
            m[0, 0] = a.m[0, 0] - b.m[0, 0]; m[1, 0] = a.m[1, 0] - b.m[1, 0]; m[2, 0] = a.m[2, 0] - b.m[2, 0];
            m[0, 1] = a.m[0, 1] - b.m[0, 1]; m[1, 1] = a.m[1, 1] - b.m[1, 1]; m[2, 1] = a.m[2, 1] - b.m[2, 1];
            m[0, 2] = a.m[0, 2] - b.m[0, 2]; m[1, 2] = a.m[1, 2] - b.m[1, 2]; m[2, 2] = a.m[2, 2] - b.m[2, 2];
            return new Matrix3(m);
        }

        public static Matrix3 operator *(Matrix3 a, Matrix3 b)
        {
            Matrix3 newMat = new Matrix3();
            for (int c = 0; c < 3; c++)
                for (int r = 0; r < 3; r++)
                    newMat.m[c, r] = a.m[0, r] * b.m[c, 0] + a.m[1, r] * b.m[c, 1] + a.m[2, r] * b.m[c, 2];
            return newMat;
        }

        public static Matrix3 operator %(Matrix3 a, Matrix3 b)
        {
            float[,] m = new float[3, 3];
            m[0, 0] = a.m[0, 0] % b.m[0, 0]; m[1, 0] = a.m[1, 0] % b.m[1, 0]; m[2, 0] = a.m[2, 0] % b.m[2, 0];
            m[0, 1] = a.m[0, 1] % b.m[0, 1]; m[1, 1] = a.m[1, 1] % b.m[1, 1]; m[2, 1] = a.m[2, 1] % b.m[2, 1];
            m[0, 2] = a.m[0, 2] % b.m[0, 2]; m[1, 2] = a.m[1, 2] % b.m[1, 2]; m[2, 2] = a.m[2, 2] % b.m[2, 2];
            return new Matrix3(m);
        }
        #endregion
    }
}
