using System;

namespace NumericalAnalysis
{
    class SeidelMethod
    {
        /*static void Main(string[] args)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            float[,] m = AskForMatrix();
            //float[,] m = new float[3, 4] { { 16.63f, -0.24f, -6.10f, 7.29f }, { -3.45f, -23.13f, 1.11f, -3.41f }, { 3.76f, -8.72f, -27.01f, -8.19f } };
            PrintMatrix(m);
            float d = AskForNumber("Введите значение погрешности:");

            float[] px = new float[m.GetLength(1) - 1];
            float[] x = new float[m.GetLength(1) - 1];

            int iteration = 0;

            bool isSolvable = m.GetLength(0) == m.GetLength(1)-1;
            for(int i = 0; i < x.Length; i++)
            {
                float number = 0f;
                for (int j = 0; j < x.Length; j++)
                {
                    if (i == j) continue;

                    number += Math.Abs(m[i, j]);
                }
                isSolvable = isSolvable && (Math.Abs(m[i, i]) >= number);
            }

            if(isSolvable)
            {
                while (!RootsAreFound(x, px, d) || iteration == 0)
                {
                    if (iteration == 0)
                    {
                        for (int i = 0; i < x.Length; i++)
                        {
                            x[i] = (m[i, x.Length]) / (m[i, i]);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < x.Length; i++) px[i] = x[i];

                        for (int i = 0; i < x.Length; i++)
                        {
                            x[i] = 0f;

                            for (int j = 0; j < x.Length; j++)
                            {
                                if (i == j) continue;

                                x[i] += (x[j] * (-m[i, j]));
                            }
                            x[i] += m[i, x.Length];
                            x[i] /= m[i, i];
                        }
                    }

                    iteration++;
                }

                Console.WriteLine("Корни найдены!");
                for(int i = 0; i < x.Length; i++)
                {
                    Console.WriteLine($"x{i+1} = {x[i]}");
                }
            }
            else
            {
                Console.WriteLine("Условия сходимости не соблюдены.");
            }
            Console.ReadLine();
        }*/

        static bool RootsAreFound(float[] x, float[] px, float d)
        {
            bool found = true;
            for(int i = 0; i < x.Length; i++)
            {
                found = found && Math.Abs(x[i] - px[i])/Math.Abs(x[i]) <= d;
            }
            return found;
        }

        static float AskForNumber(string text)
        {
            Console.WriteLine(text);
            return float.Parse(Console.ReadLine());
        }

        static float[,] AskForMatrix()
        {
            float[,] m = new float[(int)AskForNumber("Введите количество строк матрицы:"), (int)AskForNumber("Введите количество столбцов матрицы:")];
            int rows = m.GetLength(0), columns = m.GetLength(1);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    m[i, j] = AskForNumber($"Введите значение для элемента [{i},{j}]");
                }
            }
            return m;
        }

        static void PrintMatrix(float[,] matrix)
        {
            int rows = matrix.GetLength(0), columns = matrix.GetLength(1);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (j + 1 < columns)
                        Console.Write($"{matrix[i, j]}\t");
                    else
                        Console.Write($"{matrix[i, j]}");
                }
                Console.WriteLine();
            }
        }
    }
}