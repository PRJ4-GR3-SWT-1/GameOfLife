using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrencyPattern_PipelinePattern
{
    public class PipelineStringCompression
    {
        private readonly int _nStrings;
        private readonly int _stringLength;
        private readonly string _charsInString;

        double _avgCompressionRatio = 0;

        public PipelineStringCompression(string charsInString, int nStrings, int stringLength)
        {
            _charsInString = charsInString;
            _nStrings = nStrings;
            _stringLength = stringLength;
        }


        public double Run()
        {
            BlockingCollection<string> input = new BlockingCollection<string>();
            BlockingCollection<Kurt> output = new BlockingCollection<Kurt>();
            BlockingCollection<string> inputCpy = new BlockingCollection<string>();


            Task generateStringsTask = Task.Run(() =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                try
                {
                    for (var i = 0; i < _nStrings; i++)
                    {
                        var str = Generate(_stringLength);
                        input.Add(str);
                        inputCpy.Add(str);
                    }

                }
                finally
                {
                    input.CompleteAdding();
                    sw.Stop();
                    Console.WriteLine("Generated strings in time: " + sw.Elapsed);
                }
            });

            //Flere tråde der compresser strings 
            Task compressStringsTask = Task.Run(() =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                try
                {

                    foreach (var basicstring in input.GetConsumingEnumerable())
                    {
                        var basic = Compress(basicstring);
                        output.Add(new Kurt(basicstring,basic));
                    }

                }
                finally
                {
                    output.CompleteAdding();
                    sw.Stop();
                    Console.WriteLine("Compressed strings in time: " + sw.Elapsed);
                }
            });

            Task updateCompressionStatsTask = Task.Run(() =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                var count = 0;
                try
                {

                    foreach (var Kurtpar in output.GetConsumingEnumerable())
                    {
                        UpdateCompressionStats(count, Kurtpar.org, Kurtpar.com);
                        count++;
                    }
                }
                finally
                {
                    Console.WriteLine("Finished: " + count);
                    sw.Stop();
                    Console.WriteLine("Updated strings compression ratio in time: " + sw.Elapsed);
                }

            });

            Task.WaitAll(generateStringsTask, compressStringsTask, updateCompressionStatsTask);
            return _avgCompressionRatio;
        }


        private void UpdateCompressionStats(int i, string str, string compressedStr)
        {
            _avgCompressionRatio = ((i * _avgCompressionRatio) + ((double)(compressedStr.Length) / str.Length)) / (i + 1);

        }


        private string Compress(string str)
        {
            var result = "";
            for (var i = 0; i < str.Length; i++)
            {
                var j = i;
                result += str[i];
                while ((j < str.Length) && (str[i] == str[j]))
                    j++;

                if (j > i + 1)
                {
                    result += (j - i);
                    i = j - 1;
                }
            }
            return result;
        }

        private string Generate(int stringLength)
        {
            var random = new Random();
            var result = new string(Enumerable.Repeat(_charsInString, stringLength).Select(s => s[random.Next(s.Length)]).ToArray());
            return result;
        }




    }

    public class Kurt
    {
        public string org;
        public string com;

        public Kurt(string o, string c)
        {
            org = o;
            com = c; 
        }

    }
}
