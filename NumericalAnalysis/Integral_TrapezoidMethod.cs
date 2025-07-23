using System;

namespace NumericalAnalysis
{
    public class Integral_TrapezoidMethod
    {
        static void Main(string[] args)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            float integralLeft = AskForNumber("Введите нижнюю границу искомого интеграла:"), integralRight = AskForNumber("Введите верхнюю границу искомого интеграла:");
            int partsCount = (int)AskForNumber("Введите количество частей, на которые поделится промежуток:");
            float dx = (integralRight - integralLeft) / partsCount;
            float x0 = integralLeft;
            float[] fx = new float[partsCount+1];
            float[] s = new float[partsCount];

            for (int i = 0; i < fx.Length; i++)
            {
                if(i == 0)
                {
                    fx[i] = Function(x0);
                    continue;
                }
                fx[i] = Function(x0 + (dx * i));
            }

            float sum = 0f;
            for (int i = 0; i < partsCount; i++)
            {
                s[i] = (fx[i] + fx[i + 1]) / 2 * dx;
                sum += s[i];
            }
            Console.Write("F(x): ");
            PrintArray(fx);
            Console.Write("S: ");
            PrintArray(s);

            Console.WriteLine("Приближенное значение интеграла: " + sum);

            Console.ReadLine();
        }

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

        static float Function(float x)
        {
            return (float)Math.Pow(x * (float)Math.Sin(x), 2);
        }
    }
}
