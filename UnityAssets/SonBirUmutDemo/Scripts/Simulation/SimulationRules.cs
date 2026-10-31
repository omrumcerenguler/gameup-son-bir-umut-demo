using System;
using SonBirUmutDemo.Decisions;

namespace SonBirUmutDemo.Simulation
{
    public sealed class SimulationRules
    {
        public int DroughtPressure { get; }
        public int WaterUsePerTurn { get; }

        public SimulationRules(int droughtPressure = 6, int waterUsePerTurn = 8)
        {
            if (droughtPressure < 0 || droughtPressure > 100) throw new ArgumentOutOfRangeException(nameof(droughtPressure));
            if (waterUsePerTurn < 0 || waterUsePerTurn > 100) throw new ArgumentOutOfRangeException(nameof(waterUsePerTurn));
            DroughtPressure = droughtPressure;
            WaterUsePerTurn = waterUsePerTurn;
        }

        // Future pump decisions can use this limit. No pump runs in the current prototype.
        public int PumpCapacity(int energy, int requestedWater)
        {
            if (energy < 0 || energy > 100) throw new ArgumentOutOfRangeException(nameof(energy));
            if (requestedWater < 0) throw new ArgumentOutOfRangeException(nameof(requestedWater));
            return Math.Min(requestedWater, energy * 2);
        }

        public ResourceState ApplyTurn(ResourceState state, DecisionOption option)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            if (option == null) throw new ArgumentNullException(nameof(option));
            if (state.Budget < option.Cost) throw new InvalidOperationException("Insufficient budget.");

            int water = Math.Max(0, Math.Min(100, state.Water + option.WaterDelta - WaterUsePerTurn));
            int plantChange = (water >= 35 ? 4 : -8) - DroughtPressure;
            return new ResourceState(state.Budget - option.Cost, state.Energy + option.EnergyDelta,
                water, state.PlantHealth + plantChange, state.Emissions + option.EmissionsDelta);
        }
    }
}
