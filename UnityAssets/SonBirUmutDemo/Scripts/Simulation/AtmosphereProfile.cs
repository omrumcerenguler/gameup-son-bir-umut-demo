using System;

namespace SonBirUmutDemo.Simulation
{
    // Gameplay visibility metaphor. No emissions input or physical air-quality calculation.
    public sealed class AtmosphereProfile
    {
        public double Haze { get; }
        public double FogStart { get; }
        public double FogEnd { get; }
        public AtmosphereProfile(double pressure, double playerCameraDistance)
        {
            RunNumbers.Finite(pressure, nameof(pressure));
            RunNumbers.NonNegative(playerCameraDistance, nameof(playerCameraDistance));
            double t = Math.Max(0, Math.Min(1, pressure / 100));
            Haze = t * t * (3 - 2 * t);
            // Player and at least seven world units behind the player remain before linear fog.
            FogStart = playerCameraDistance + 30 - 23 * Haze;
            FogEnd = FogStart + 75 - 54 * Haze;
        }
        public static double Urgency(double secondsRemaining)
        {
            RunNumbers.NonNegative(secondsRemaining, nameof(secondsRemaining));
            return Math.Max(0, 1 - secondsRemaining / 12);
        }
    }
}
