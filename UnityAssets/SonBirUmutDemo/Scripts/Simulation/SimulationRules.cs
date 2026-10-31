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

        // One decision turn, not a continuous physical water/temperature model.
        public int PumpCapacity(int energy, int requestedWater)
        {
            if (energy < 0 || energy > 100) throw new ArgumentOutOfRangeException(nameof(energy));
            if (requestedWater < 0) throw new ArgumentOutOfRangeException(nameof(requestedWater));
            return Math.Min(requestedWater, energy * 2);
        }

        public ResourceState ApplyTurn(ResourceState state, DecisionOption option)
        {
            var preview = Preview(state, option);
            if (!preview.CanChoose) throw new InvalidOperationException(preview.Reason);
            return preview.After;
        }

        public DecisionPreview Preview(ResourceState state, DecisionOption option, bool efficientWaterUse = false)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            if (option == null) throw new ArgumentNullException(nameof(option));
            int cost = option.Cost + (option.Effect == DecisionEffect.Pump && state.Energy < 25 ? 5 : 0);
            if (state.Budget < cost) return DecisionPreview.Blocked(state, $"Bütçe yetersiz: {cost} gerekli, {state.Budget} var.", cost);
            if (state.Energy < option.RequiredEnergy) return DecisionPreview.Blocked(state, $"Enerji yetersiz: en az {option.RequiredEnergy} gerekli.", cost);
            if (state.Water < option.RequiredWater) return DecisionPreview.Blocked(state, $"Su yetersiz: en az {option.RequiredWater} gerekli.", cost);

            int waterDelta = option.WaterDelta;
            int plantSupport = 0;
            int use = efficientWaterUse || option.Effect == DecisionEffect.WaterEfficiency ? Math.Min(4, WaterUsePerTurn) : WaterUsePerTurn;
            switch (option.Effect)
            {
                case DecisionEffect.Pump: waterDelta += PumpCapacity(state.Energy, 50); break;
                case DecisionEffect.ProtectNature: plantSupport = Math.Min(22, state.Water / 2); break;
                case DecisionEffect.Irrigate: plantSupport = Math.Min(36, state.Water); break;
                case DecisionEffect.StoreWater: use = Math.Min(2, use); plantSupport = -5; break;
            }
            int water = Math.Max(0, Math.Min(100, state.Water + waterDelta - use));
            int plantChange = plantSupport + (water >= 35 ? 4 : -8) - DroughtPressure;
            var after = new ResourceState(state.Budget - cost, state.Energy + option.EnergyDelta,
                water, state.PlantHealth + plantChange, state.Emissions + option.EmissionsDelta);
            return new DecisionPreview(state, after, cost, "");
        }
    }
}
