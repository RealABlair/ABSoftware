using System;

namespace NumericalAnalysis
{
    public class Integral_RectMethod
    {
        /*static void Main(string[] args)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            float integralLeft = AskForNumber("Введите нижнюю границу искомого интеграла:"), integralRight = AskForNumber("Введите верхнюю границу искомого интеграла:");
            int partsCount = (int)AskForNumber("Введите количество частей, на которые поделится промежуток:");
            float dx = (integralRight - integralLeft) / partsCount;
            float x0 = integralLeft + (dx / 2);
            float[] fx = new float[partsCount];
            float sum = 0f;
            for (int i = 0; i < partsCount; i++)
            {
                if(i == 0)
                {
                    fx[i] = Function(x0);
                    sum += fx[i];
                    continue;
                }
                fx[i] = Function(x0 + (dx * i));
                sum += fx[i];
            }
            Console.Write("F(x): ");
            PrintArray(fx);

            Console.WriteLine("Сумма чисел: " + sum);
            Console.WriteLine("Приближенное значение интеграла: " + sum*dx);

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

        static float Function(float x)
        {
            return (float)Math.Sin(x) / x;
        }
    }
}
