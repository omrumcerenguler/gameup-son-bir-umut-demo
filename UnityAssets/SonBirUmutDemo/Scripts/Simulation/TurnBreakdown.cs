namespace SonBirUmutDemo.Simulation
{
    // Signed intended effects are kept separate from the final 0..100 bounds.
    public sealed class TurnBreakdown
    {
        public int DecisionEnergy { get; }
        public int DecisionWater { get; }
        public int DecisionPlants { get; }
        public int DecisionEmissions { get; }
        public int WaterUse { get; }
        public int WaterPlantEffect { get; }
        public int DroughtDamage { get; }
        public int EnergyLimitAdjustment { get; }
        public int WaterLimitAdjustment { get; }
        public int PlantLimitAdjustment { get; }
        public int EmissionsLimitAdjustment { get; }

        public TurnBreakdown(ResourceState before, ResourceState after, int energy, int water,
            int plants, int emissions, int waterUse, int waterPlantEffect, int droughtDamage)
        {
            DecisionEnergy = energy;
            DecisionWater = water;
            DecisionPlants = plants;
            DecisionEmissions = emissions;
            WaterUse = waterUse;
            WaterPlantEffect = waterPlantEffect;
            DroughtDamage = droughtDamage;
            EnergyLimitAdjustment = after.Energy - before.Energy - energy;
            WaterLimitAdjustment = after.Water - before.Water - (water - waterUse);
            PlantLimitAdjustment = after.PlantHealth - before.PlantHealth - (plants + waterPlantEffect - droughtDamage);
            EmissionsLimitAdjustment = after.Emissions - before.Emissions - emissions;
        }
    }
}
