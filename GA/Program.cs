using System;
using System.Collections.Generic;
using System.Linq;

namespace GA
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var ga = new GeneticAlgorithm();
            
            //ga.RandomSearch(80000);
            var p = new[] { 2000 };
            for (int j = 0; j < 1; j++)
            {
                var list = new List<Individual>();
                for (int i = 1; i <= 10; i++)
                {
                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    ga.Initialise(p[j]);
                    Individual ind = ga.Run();
                    watch.Stop();
                    var elapsedMs = watch.ElapsedMilliseconds;

                    list.Add(ind);
                    Console.WriteLine($"{i}: {elapsedMs}");
                }

                list = list.OrderBy(i => i.Fitness).ToList();
                double avg = list.Average(i => i.Fitness);
                Console.WriteLine($"\n10x Result:\nbest: {list[0]}\nworst: {list[9]}\navg: {avg}\n\n");
            }
        }
    }
}
