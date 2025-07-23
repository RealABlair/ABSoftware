using System;

namespace NumericalAnalysis
{
    class BisectionMethod
    {
        /*static void Main(string[] args)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            float a = AskForNumber("Введите число 'a':"),
                b = AskForNumber("Введите число 'b':"),
                d = AskForNumber("Введите погрешность:"),
                c,
                x;
            while(b - a > d * 2)
            {
                c = (a + b) / 2;

                if (Function(a) * Function(c) < 0)
                    b = c;
                else
                    a = c;
            }

            x = (a + b) / 2;
            Console.WriteLine("Число 'x' = " + x.ToString());
            Console.WriteLine("Программа выполнила свою работу.");
            Console.ReadLine();
        }*/
        
        static float AskForNumber(string text)
        {
            Console.WriteLine(text);
            return float.Parse(Console.ReadLine());
        }

        static float Function(float x)
        {
            return 2 * (float)Math.Sin(x) - (float)Math.Atan(x);
        }
    }
}
