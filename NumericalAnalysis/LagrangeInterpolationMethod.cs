using System;

namespace NumericalAnalysis
{
    public class LagrangeInterpolationMethod
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

            //float[] x = new float[5] { 0.27f, 0.93f, 1.46f, 2.11f, 2.87f }, y = new float[5] { 2.60f, 2.43f, 2.06f, 0.25f, -2.60f };

            Console.Write("x: ");
            PrintArray(x);
            Console.Write("y: ");
            PrintArray(y);

            float inputX;
            while(true)
            {
                Console.WriteLine("Введите число или ключевое слово exit, для выхода из цикла: ");
                string consoleInput = Console.ReadLine();
                if (consoleInput.ToLower() == "exit")
                    break;

                inputX = float.Parse(consoleInput);

                float result = 0;
                for(int i = 0; i < y.Length; i++)
                {
                    float part = 0f;
                    for(int j = 0; j < x.Length; j++)
                    {
                        if (i == j) continue;

                        if (part == 0)
                            part = (inputX - x[j]) / (x[i] - x[j]);
                        else
                            part *= (inputX - x[j]) / (x[i] - x[j]);
                    }

                    result += (y[i] * part);
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
    }
}
