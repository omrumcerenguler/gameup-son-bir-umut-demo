using System;

namespace SonBirUmutDemo.Simulation
{
    public sealed class CrisisPressure
    {
        private readonly double initial;
        public double Value { get; private set; }
        public CrisisPressure(double initial = 60)
        {
            RunNumbers.Finite(initial, nameof(initial));
            this.initial = Math.Max(0, Math.Min(100, initial));
            Reset();
        }
        public void Change(double amount)
        {
            RunNumbers.Finite(amount, nameof(amount));
            Value = Math.Max(0, Math.Min(100, Value + amount));
        }
        public void Reset() => Value = initial;
    }
}
