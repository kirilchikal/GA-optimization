using System;
using System.Collections.Generic;
using System.Linq;
using GA.Data;

namespace GA
{
    internal class Population
    {
        private readonly int _columns;
        private readonly int _machines;
        private readonly int _populationSize;
        private readonly int _emptyFields;
        
        public List<CostItem> Costs { get; set; }
        public List<FlowItem> Flows { get; set; }
        public List<Individual> Solutions { get; set; }

        public Population(int m, int n, int machines, int popSize)
        {
            this._columns = n;
            this._machines = machines;
            this._populationSize = popSize;
            this._emptyFields = m * _columns - _machines;
        }
        
        public void GeneratePopulation()
        {
            var population = new List<Individual>();

            for (int i = 0; i < _populationSize; i++)
            {
                Individual individual = CreateIndividual();
                population.Add(individual);
            }

            Solutions = population;
        }

        public Individual CreateIndividual()
        {
            var genes = Enumerable.Range(0, _machines).ToList();

            if (_emptyFields > 0)
            {
                Enumerable.Range(1, _emptyFields).ToList().ForEach(arg => genes.Add(arg * -1));
            }

            genes = genes.OrderBy(g => Guid.NewGuid()).ToList();
            var individual = new Individual() { Genes = genes };
            Estimate(individual);
            return individual;
        }

        public void Estimate(Individual individual)
        {
            double fitness = 0;

            for (int i = 0; i < Costs.Count; i++)
            {
                int sourceIndex = individual.Genes.IndexOf(Costs[i].Source) + 1;
                int destIndex = individual.Genes.IndexOf(Costs[i].Dest) + 1;

                int distance = Math.Abs(sourceIndex % _columns - destIndex % _columns) 
                               + Math.Abs((int)Math.Floor((double)sourceIndex / _columns) 
                                          - (int)Math.Floor((double)destIndex / _columns));

                fitness += Costs[i].Cost * Flows[i].Amount * distance;
                //Console.WriteLine($"{Costs[i].source},{Costs[i].dest}: cost:{Costs[i].cost}, flow:{Flows[i].amount}, distance:{ distance}");
            }

            individual.Fitness = fitness;
        }

        public void Estimate()
        {
            foreach (var ind in Solutions)
            {
                Estimate(ind);
            }
        }
        
        public void Sort()
        {
            Solutions = Solutions.OrderBy(i => i.Fitness).ToList();
        }
    }
}
