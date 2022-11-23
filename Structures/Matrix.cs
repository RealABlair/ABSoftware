using System;

namespace ABSoftware.Structures
{
    public class Matrix
    {
        public int Rows { get { return matrix.GetLength(0); } }
        public int Columns { get { return matrix.GetLength(1); } }

        public float[,] matrix { get; private set; }

        public Matrix(int Rows, int Columns)
        {
            matrix = new float[Rows,Columns];
        }

        /// <summary>
        /// The "Rows" and "Columns" are inverted in this method
        /// </summary>
        /// <param name="matrix"></param>
        public Matrix(float[,] matrix)
        {
            int Rows = matrix.GetLength(1);
            int Columns = matrix.GetLength(0);
            float[,] editedMatrix = new float[Rows, Columns];
            for(int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    editedMatrix[r, c] = matrix[c, r];
                }
            }
            this.matrix = editedMatrix;
        }

        public float this[int Row, int Column]
        {
            get { return matrix[Row, Column]; }
            set { matrix[Row, Column] = value; }
        }

        public override string ToString()
        {
            string mat = "";
            for(int c = 0; c < Columns; c++)
            {
                for (int r = 0; r < Rows; r++)
                {
                    mat += matrix[r, c] + " ";
                }
                mat += Environment.NewLine;
            }
            return mat;
        }

        #region Operators
        public static Matrix operator +(Matrix a, Matrix b)
        {
            if (a.Columns != b.Columns || a.Rows != b.Rows)
                throw new Exception("The matrices sizes are not equal.");
            Matrix ret = new Matrix(a.Rows, a.Columns);
            for (int c = 0; c < ret.Columns; c++)
            {
                for (int r = 0; r < ret.Rows; r++)
                {
                    ret[r, c] = a[r, c] + b[r, c];
                }
            }
            return ret;
        }

        public static Matrix operator -(Matrix a, Matrix b)
        {
            if (a.Columns != b.Columns || a.Rows != b.Rows)
                throw new Exception("The matrices sizes are not equal.");
            Matrix ret = new Matrix(a.Rows, a.Columns);
            for (int c = 0; c < ret.Columns; c++)
            {
                for (int r = 0; r < ret.Rows; r++)
                {
                    ret[r, c] = a[r, c] - b[r, c];
                }
            }
            return ret;
        }

        public static Matrix operator *(Matrix a, Matrix b)
        {
            if (a.Columns != b.Rows)
                throw new Exception("The matrices sizes are not equal.");
            Matrix ret = new Matrix(b.Rows, a.Columns);
            for (int c = 0; c < ret.Columns; c++)
            {
                for (int r = 0; r < ret.Rows; r++)
                {
                    float irv = 0f; 
                    for(int ir = 0; ir < a.Rows; ir++) { irv += b[r, ir] * a[ir, c]; }
                    ret[r, c] = irv;
                }
            }
            return ret;
        }
        #endregion
    }
}
