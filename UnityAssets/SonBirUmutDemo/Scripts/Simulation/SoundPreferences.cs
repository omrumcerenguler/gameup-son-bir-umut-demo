using System;

namespace SonBirUmutDemo.Simulation
{
    public sealed class SoundPreferences
    {
        public double Volume { get; private set; }
        public bool Muted { get; private set; }
        public double EffectiveVolume => Muted ? 0 : Volume;
        public SoundPreferences(double volume = .65, bool muted = false) { SetVolume(volume); Muted = muted; }
        public void SetVolume(double volume)
        { RunNumbers.Finite(volume, nameof(volume)); Volume = Math.Max(0, Math.Min(1, volume)); }
        public void SetMuted(bool muted) => Muted = muted;
    }
}
