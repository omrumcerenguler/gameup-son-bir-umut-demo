using System;

namespace SonBirUmutDemo.Simulation
{
    public sealed class SessionTimer
    {
        public double DurationSeconds { get; }
        public double RemainingSeconds { get; private set; }
        public bool IsExpired => RemainingSeconds <= 0;

        public SessionTimer(double durationSeconds = 240)
        {
            RunNumbers.NonNegative(durationSeconds, nameof(durationSeconds));
            if (durationSeconds == 0) throw new ArgumentOutOfRangeException(nameof(durationSeconds));
            DurationSeconds = durationSeconds;
            Reset();
        }

        public double Advance(double seconds, bool running)
        {
            RunNumbers.NonNegative(seconds, nameof(seconds));
            if (!running || IsExpired) return 0;
            double used = Math.Min(seconds, RemainingSeconds);
            RemainingSeconds = Math.Max(0, RemainingSeconds - used);
            return used;
        }

        public void Reset() => RemainingSeconds = DurationSeconds;
    }
}
