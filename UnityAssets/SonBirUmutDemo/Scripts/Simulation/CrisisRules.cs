using System;

namespace SonBirUmutDemo.Simulation
{
    // Dimensionless game balance, not physical climate measurements.
    public sealed class CrisisRules
    {
        public double StableThreshold { get; }
        public double StableDuration { get; }
        public double EmergencyDelay { get; }
        public double EmergencyWindow { get; }
        public double RepairDuration { get; }
        public const double RepairRelief = 18;
        public const double MissedPressure = 8;
        public const int MissedWater = 6;

        public CrisisRules(double stableThreshold = 35, double stableDuration = 10,
            double emergencyDelay = 12, double emergencyWindow = 45, double repairDuration = 2)
        {
            RunNumbers.InRange(stableThreshold, 0, 100, nameof(stableThreshold));
            RunNumbers.NonNegative(emergencyDelay, nameof(emergencyDelay));
            Positive(stableDuration, nameof(stableDuration));
            Positive(emergencyWindow, nameof(emergencyWindow));
            Positive(repairDuration, nameof(repairDuration));
            if (emergencyWindow < repairDuration) throw new ArgumentException("Repair must fit in the emergency window.");
            StableThreshold = stableThreshold; StableDuration = stableDuration;
            EmergencyDelay = emergencyDelay; EmergencyWindow = emergencyWindow; RepairDuration = repairDuration;
        }

        private static void Positive(double value, string name)
        {
            RunNumbers.NonNegative(value, name);
            if (value == 0) throw new ArgumentOutOfRangeException(name);
        }
    }
}
