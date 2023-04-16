using System.Collections.Generic;

namespace GA
{
    internal class Individual
    {
        public List<int> Genes { get; set; } 
        public double Fitness { get; set; }

        public override string ToString()
        {
            return $"Solution: {string.Join(',', Genes)} -- fitness: {Fitness}";
        }

    }
}
