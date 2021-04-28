using System;
using System.Diagnostics;


namespace GameOfLife
{
    class Program
    {
        static void Main(string[] args)
        {
            var gol = new GameOfLife(1000);
            var sw = new Stopwatch();

            Console.WriteLine("TaskBarrier:");
            sw.Restart();
            var golTB = new GameOfLifeTB(1000);
            golTB.Run(50);
            Console.WriteLine(sw.Elapsed);

            Console.WriteLine("Sequential:");
            sw.Restart();
            gol.Run(50);
            sw.Stop();
            Console.WriteLine(sw.Elapsed);

            Console.WriteLine("Parallel:");
            sw.Restart();
            var golP = new GameOfLifeP(1000);
            golP.Run(50);
            Console.WriteLine(sw.Elapsed);

           

        }
    }
}
