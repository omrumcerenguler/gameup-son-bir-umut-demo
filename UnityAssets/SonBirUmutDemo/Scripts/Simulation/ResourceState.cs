using System;

namespace SonBirUmutDemo.Simulation
{
    // Dimensionless game balance values, not scientific measurements.
    public sealed class ResourceState
    {
        public int Budget { get; }
        public int Energy { get; }
        public int Water { get; }
        public int PlantHealth { get; }
        public int Emissions { get; }

        public ResourceState(int budget, int energy, int water, int plantHealth, int emissions)
        {
            Budget = Clamp(budget, 0, 999);
            Energy = Clamp(energy, 0, 100);
            Water = Clamp(water, 0, 100);
            PlantHealth = Clamp(plantHealth, 0, 100);
            Emissions = Clamp(emissions, 0, 100);
        }

        public static ResourceState Initial() => new ResourceState(100, 20, 40, 55, 0);
        private static int Clamp(int value, int min, int max) => Math.Max(min, Math.Min(max, value));
    }
}
