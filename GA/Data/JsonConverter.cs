using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace GA.Data
{
    internal class JsonConverter
    {
        private readonly ProblemInstance _problem;
        private const string Directory = @"FLO-Data\";

        public JsonConverter(ProblemInstance problem)
        {
            this._problem = problem;
        }

        public List<CostItem> LoadCosts()
        {
            var file = Directory + $"{_problem.ToString().ToLower()}_cost.json";
            using var r = new StreamReader(file);
            var json = r.ReadToEnd();
            return JsonConvert.DeserializeObject<List<CostItem>>(json);
        }

        public List<FlowItem> LoadFlows()
        {
            var file = Directory + $"{_problem.ToString().ToLower()}_flow.json";
            using var r = new StreamReader(file);
            var json = r.ReadToEnd();
            return JsonConvert.DeserializeObject<List<FlowItem>>(json);
        }
    }
}
