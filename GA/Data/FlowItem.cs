using System;

namespace GA.Data
{
    [Serializable]
    internal class FlowItem
    {
        public int Source { get; set; }
        public int Dest { get; set; }
        public double Amount { get; set; }
    }
}
