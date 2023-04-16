using System;

namespace GA.Data
{
    [Serializable]
    internal class CostItem
    {
        public int Source { get; set; }
        public int Dest { get; set; }
        public double Cost { get; set; }
    }
}
