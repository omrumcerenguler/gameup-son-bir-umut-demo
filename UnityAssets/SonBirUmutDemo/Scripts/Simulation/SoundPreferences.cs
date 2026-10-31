using System;

namespace SonBirUmutDemo.Simulation
{
    public sealed class SoundPreferences
    {
        public double Volume { get; private set; }
        public bool Muted { get; private set; }
        public double MusicVolume { get; private set; }
        public double EffectsVolume { get; private set; }
        public double EffectiveMusic => EffectiveVolume*MusicVolume;
        public double EffectiveEffects => EffectiveVolume*EffectsVolume;
        public double EffectiveVolume => Muted ? 0 : Volume;
        public SoundPreferences(double volume = .65, bool muted = false, double music = .55, double effects = 1)
        { SetVolume(volume); SetMusicVolume(music); SetEffectsVolume(effects); Muted = muted; }
        public void SetVolume(double volume)
        { RunNumbers.Finite(volume, nameof(volume)); Volume = Math.Max(0, Math.Min(1, volume)); }
        public void SetMusicVolume(double volume) { RunNumbers.Finite(volume,nameof(volume)); MusicVolume=Math.Max(0,Math.Min(1,volume)); }
        public void SetEffectsVolume(double volume) { RunNumbers.Finite(volume,nameof(volume)); EffectsVolume=Math.Max(0,Math.Min(1,volume)); }
        public void SetMuted(bool muted) => Muted = muted;
    }
}
