using System;
using System.Collections.Generic;
using System.Linq;
using GA.Data;

namespace GA
{
    internal class GeneticAlgorithm
    {
        private const ProblemInstance Problem = ProblemInstance.Flat;
        private const int PopulationSize = 100;
        private const double TournamentParameter = 0.2;
        private const double CrossoverRate = 0.7;
        private const double MutationRate = 0.08;
        
        private int _numberIterations;
        private int _rows, _columns, _machines;
        private Random _random;
        private Population _currentPop;

        public void Initialise(int p)
        {
            _numberIterations = p;
            (_rows, _columns, _machines) = GetInstanceData();
            _random = new Random();
            var converter = new JsonConverter(Problem);

            _currentPop = new Population(_rows, _columns, _machines, PopulationSize)
            {
                Costs = converter.LoadCosts(),
                Flows = converter.LoadFlows()
            };
        }
        
        public Individual Run()
        {
            _currentPop.GeneratePopulation();

            Individual bestIndividual = _currentPop.Solutions[0];
            
            int iteration = 0;
            while (iteration < _numberIterations)
            {
                List<Individual> newPop = new List<Individual>();

                while (newPop.Count != _currentPop.Solutions.Count)
                {
                    Individual p1 = TournamentSelection();

                    Individual child;
                    if (_random.NextDouble() < CrossoverRate)
                    {
                        Individual p2;
                        do
                        {
                            p2 = TournamentSelection();

                        } while (p1 == p2);
                        child = Crossover2(p1, p2);
                        _currentPop.Estimate(child);
                    }
                    else
                    {
                        child = new Individual
                        {
                            Genes = new List<int>(p1.Genes),
                            Fitness = p1.Fitness
                        };
                    }

                    if (_random.NextDouble() < MutationRate)
                    {
                        Mutate(child);
                    }

                    if (child.Fitness < bestIndividual.Fitness)
                    {
                        bestIndividual = child;
                    }

                    newPop.Add(child);
                }
                
                //currentPop.Sort();
                //Console.WriteLine($"iter:{iteration}; best: {currentPop.Solutions[0]}");
                _currentPop.Solutions = newPop;
                iteration++;
            }

            return bestIndividual;
        }


        private Individual TournamentSelection()
        {
            var tournament = new HashSet<int>() { _random.Next(PopulationSize) };
            Individual best = _currentPop.Solutions[tournament.First()];

            while (tournament.Count != Math.Ceiling(PopulationSize * TournamentParameter))
            {
                int nextIndex = _random.Next(PopulationSize);
                if (!tournament.Add(nextIndex)) continue;
                if (best.Fitness > _currentPop.Solutions[nextIndex].Fitness)
                {
                    best = _currentPop.Solutions[nextIndex];
                }
            }

            return best;
        }
        
        public Individual RouletteSelection()
        {
            double sumFitness = _currentPop.Solutions.Sum(i => 1 / (Math.Pow(i.Fitness, 3)));
            double[] probabilities = _currentPop.Solutions.Select(i => 1 / (Math.Pow(i.Fitness, 3) * sumFitness)).ToArray();

            double randomPoint = _random.NextDouble();
            double offset = 0.0;
            int returnIndex = 0;
            for (int i = 0; i < PopulationSize; i++)
            {
                offset += probabilities[i];
                if (randomPoint < offset)
                {
                    returnIndex = i;
                    break;
                }
            }
            
            return _currentPop.Solutions[returnIndex];
        }

        private Individual Crossover2(Individual p1, Individual p2)
        {
            int pivot1, pivot2;
            int[] genes = new int[p1.Genes.Count];
            
            do
            {
                pivot1 = _random.Next(p1.Genes.Count);
                pivot2 = _random.Next(p1.Genes.Count);
            } while (pivot1 == pivot2);
            if (pivot1 > pivot2)
            {
                (pivot1, pivot2) = (pivot2, pivot1);
            }
            
            int index;
            var swaps = new Dictionary<int, int>();
            for (int i = pivot1; i <= pivot2; i++)
            {
                swaps.Add(p2.Genes[i], p1.Genes[i]);
                genes[i] = p2.Genes[i];
            }

            for (int i = 0; i < pivot1; i++)
            {
                index = p2.Genes.IndexOf(p1.Genes[i]);
                if (index >= pivot1 && index <= pivot2)
                {
                    int toSwap = p2.Genes[index];
                    while (swaps.ContainsKey(toSwap))
                    {
                        toSwap = swaps[toSwap];
                    }
                    genes[i] = toSwap;
                }
                else
                {
                    genes[i] = p1.Genes[i];
                }
            }
            for (int i = pivot2 + 1; i < p1.Genes.Count; i++)
            {
                index = p2.Genes.IndexOf(p1.Genes[i]);
                if (index >= pivot1 && index <= pivot2)
                {
                    int toSwap = p2.Genes[index];
                    while (swaps.ContainsKey(toSwap))
                    {
                        toSwap = swaps[toSwap];
                    }
                    genes[i] = toSwap;
                }
                else
                {
                    genes[i] = p1.Genes[i];
                }
            }
            
            return new Individual() { Genes = genes.ToList() };
        }

        public Individual Crossover(Individual p1, Individual p2)
        {
            int[] genes = new int[p1.Genes.Count];
            int pivot = _random.Next(p1.Genes.Count);
            
            var swaps = new Dictionary<int, int>();
            for (int i = pivot + 1; i < p1.Genes.Count; i++)
            {
                swaps.Add(p2.Genes[i], p1.Genes[i]);
                genes[i] = p2.Genes[i];
            }
            
            for (int i = 0; i < pivot + 1; i++)
            {
                int index = p2.Genes.IndexOf(p1.Genes[i]);
                if (index > pivot)
                {
                    int toSwap = p2.Genes[index];
                    while (swaps.ContainsKey(toSwap))
                    {
                        toSwap = swaps[toSwap];
                    }
                    genes[i] = toSwap;
                }
                else
                {
                    genes[i] = p1.Genes[i];
                }
            }

            return new Individual() { Genes = genes.ToList() };
        }
        
        private static void Mutate(Individual individual)
        {
            var randomList = Enumerable.Range(0, individual.Genes.Count)
                .OrderBy(i => Guid.NewGuid()).ToList();

            int firstIndex = randomList[0];
            int secondIndex = randomList[1];

            (individual.Genes[firstIndex], individual.Genes[secondIndex]) = (individual.Genes[secondIndex], individual.Genes[firstIndex]);
        }

        static (int, int, int) GetInstanceData()
        {
            return Problem switch
            {
                ProblemInstance.Easy => (3, 3, 9),
                ProblemInstance.Flat => (1, 12, 12),
                _ => (5, 6, 24)
            };
        }

        public void RandomSearch(int iterations)
        {
            Individual bestIndv = _currentPop.CreateIndividual();
            Individual worstIndv = bestIndv;
            double avg = 0;

            Individual currentIndv;

            for (int i = 0; i < iterations; i++)
            {
                currentIndv = _currentPop.CreateIndividual();
                avg += currentIndv.Fitness;

                if (currentIndv.Fitness < bestIndv.Fitness)
                {
                    bestIndv = currentIndv;
                }
                if (currentIndv.Fitness > worstIndv.Fitness)
                {
                    worstIndv.Fitness = currentIndv.Fitness;
                }
            }

            avg = avg / iterations;
            Console.WriteLine($"Rand method: best: {bestIndv.Fitness}\nworst: {worstIndv.Fitness}\n avg: {avg}");
        }
    }
}
