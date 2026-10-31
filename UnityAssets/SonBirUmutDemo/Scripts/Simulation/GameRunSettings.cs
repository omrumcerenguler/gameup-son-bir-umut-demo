using System;

namespace SonBirUmutDemo.Simulation
{
    public sealed class GameRunSettings
    {
        public double DurationSeconds { get; }
        public double InitialPressure { get; }
        public double BaseRise { get; }
        public double WaterRiskRise { get; }
        public double PlantRiskRise { get; }
        public double EnergyRiskRise { get; }
        public double MaximumRisePerSecond { get; }

        public GameRunSettings(double durationSeconds = 240, double initialPressure = 60,
            double baseRise = .04, double waterRiskRise = .04, double plantRiskRise = .03,
            double energyRiskRise = .02, double maximumRisePerSecond = .25)
        {
            RunNumbers.NonNegative(durationSeconds, nameof(durationSeconds));
            if (durationSeconds == 0) throw new ArgumentOutOfRangeException(nameof(durationSeconds));
            RunNumbers.InRange(initialPressure, 0, 100, nameof(initialPressure));
            RunNumbers.InRange(baseRise, 0, 1, nameof(baseRise));
            RunNumbers.InRange(waterRiskRise, 0, 1, nameof(waterRiskRise));
            RunNumbers.InRange(plantRiskRise, 0, 1, nameof(plantRiskRise));
            RunNumbers.InRange(energyRiskRise, 0, 1, nameof(energyRiskRise));
            RunNumbers.InRange(maximumRisePerSecond, 0, 1, nameof(maximumRisePerSecond));
            DurationSeconds = durationSeconds; InitialPressure = initialPressure;
            BaseRise = baseRise; WaterRiskRise = waterRiskRise; PlantRiskRise = plantRiskRise;
            EnergyRiskRise = energyRiskRise; MaximumRisePerSecond = maximumRisePerSecond;
        }

        // A dimensionless gameplay risk indicator. No temperature or emissions simulation.
        public double RiseRate(ResourceState state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            double rate = BaseRise + WaterRiskRise * (1 - state.Water / 100d) +
                PlantRiskRise * (1 - state.PlantHealth / 100d) + EnergyRiskRise * (1 - state.Energy / 100d);
            return Math.Min(MaximumRisePerSecond, rate);
        }
    }

    internal static class RunNumbers
    {
        public static void Finite(double value, string name)
        {
            if (double.IsNaN(value) || double.IsInfinity(value)) throw new ArgumentOutOfRangeException(name);
        }
        public static void NonNegative(double value, string name)
        {
            Finite(value, name);
            if (value < 0) throw new ArgumentOutOfRangeException(name);
        }
        public static void InRange(double value, double min, double max, string name)
        {
            Finite(value, name);
            if (value < min || value > max) throw new ArgumentOutOfRangeException(name);
        }
    }
}
