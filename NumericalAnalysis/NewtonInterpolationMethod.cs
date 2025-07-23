using System;

namespace NumericalAnalysis
{
    public class NewtonInterpolationMethod
    {
        /*static void Main(string[] args)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            int inputLength = 0;
            Console.WriteLine("Введите количество известных x: ");
            inputLength = int.Parse(Console.ReadLine());
            float[] x = new float[inputLength], y = new float[inputLength];
            
            for(int i = 0; i < inputLength; i++)
            {
                x[i] = AskForNumber($"Введите 'x' с индексом '{i}': ");
            }

            for (int i = 0; i < inputLength; i++)
            {
                y[i] = AskForNumber($"Введите 'y' с индексом '{i}': ");
            }

            //inputLength = 5;
            //float[] x = new float[5] { 1.25f, 1.30f, 1.35f, 1.40f, 1.45f }, y = new float[5] { 1.60f, 1.71f, 1.81f, 1.88f, 1.94f };

            float step = x[1] - x[0];

            Console.WriteLine("Шаг x: " + step);

            Console.Write("x: ");
            PrintArray(x);
            Console.Write("y: ");
            PrintArray(y);

            float[,] dy = new float[inputLength, inputLength]; //[delta power, data]
            for(int i = 0; i < dy.GetLength(0); i++)
            {
                for(int j = 0; j < dy.GetLength(1) - i; j++)
                {
                    if(i == 0)
                    {
                        dy[i, j] = y[j];
                    }
                    else
                    {
                        dy[i, j] = dy[i - 1, j + 1] - dy[i - 1, j];
                    }
                }
            }
            PrintYDeltas(dy);

            float inputX;
            while(true)
            {
                Console.WriteLine("Введите число или ключевое слово exit, для выхода из цикла: ");
                string consoleInput = Console.ReadLine();
                if (consoleInput.ToLower() == "exit")
                    break;

                inputX = float.Parse(consoleInput);

                float result = 0;
                for(int i = 0; i < inputLength; i++)
                {
                    float a = GetA(i, step, ref dy);
                    float multiplier = 1;
                    for(int j = 0; j < i; j++)
                    {
                        multiplier *= (inputX - x[j]);
                    }
                    result += a * multiplier;
                }

                Console.WriteLine($"Результат (Fx): {result}");
            }

            Console.WriteLine("Программа закончила свою работу.");
            Console.ReadLine();
        }*/

        static float AskForNumber(string text)
        {
            Console.WriteLine(text);
            return float.Parse(Console.ReadLine());
        }

        static void PrintArray(float[] array)
        {
            Console.Write("[");
            for (int i = 0; i < array.Length; i++)
            {
                if (i + 1 < array.Length)
                    Console.Write($"{array[i]}, ");
                else
                    Console.Write($"{array[i]}");
            }
            Console.WriteLine("]");
        }

        static int GetMaxNumberLength(ref float[,] dy)
        {
            int max = 0;

            for(int i = 0; i < dy.GetLength(0); i++)
            {
                for (int j = 0; j < dy.GetLength(1); j++)
                {
                    int size = dy[i, j].ToString().Length;
                    if (max < size)
                        max = size;
                }
            }

            return max;
        }

        static void PrintYDeltas(float[,] dy)
        {
            int l0 = dy.GetLength(0), l1 = dy.GetLength(1), maxNumberLength = GetMaxNumberLength(ref dy) + 1;
            for(int j = -1; j < l1; j++)
            {
                if(j == -1)
                {
                    for (int i = 0; i < l0; i++)
                    {
                        if (i == 0)
                            Console.Write("y  ".PadRight(maxNumberLength));
                        else
                            Console.Write($"dx{i}".PadRight(maxNumberLength));
                    }
                    Console.WriteLine();
                }
                else
                {
                    for (int i = 0; i < l0; i++)
                    {
                        Console.Write(dy[i, j].ToString().PadRight(maxNumberLength));
                    }
                    Console.WriteLine();
                }
            }    
        }

        static int Factorial(int number)
        {
            if (number <= 0)
                return 1;
            else
                return number * Factorial(number - 1);
        }

        static float GetA(int aIndex, float step, ref float[,] dy)
        {
            return dy[aIndex, 0] / (Factorial(aIndex) * (float)Math.Pow(step, aIndex));
        }
    }
}
