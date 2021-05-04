using System;
using System.Diagnostics;

namespace ConcurrencyPattern_PipelinePattern
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Sequential Compression: ");
            SequentialStringCompression ssc = new SequentialStringCompression("ABC", 100, 25000);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.WriteLine("Average compression ratio: "+ ssc.Run());
            stopwatch.Stop();
            Console.WriteLine("Time used: "+ stopwatch.Elapsed);
            //1000 strings = 88% = 45 s

            Console.WriteLine("Pipeline Compression: ");
            PipelineStringCompression psc = new PipelineStringCompression("ABC", 100, 25000);
            stopwatch.Restart();
            Console.WriteLine("Average compression ratio: " + psc.Run());
            stopwatch.Stop();
            Console.WriteLine("Time used: " + stopwatch.Elapsed);




        }
    }
}
