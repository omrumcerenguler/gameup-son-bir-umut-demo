using System;

namespace SonBirUmutDemo.Simulation
{
    public enum SoundCue { Menu, Decision, Maintenance, Emergency, Success, Failure }

    public static class SoundSynthesis
    {
        // Original short sine/chord cues; no downloaded recording, music or middleware.
        public static float[] Generate(SoundCue cue, int sampleRate = 22050)
        {
            if (!Enum.IsDefined(typeof(SoundCue), cue)) throw new ArgumentOutOfRangeException(nameof(cue));
            if (sampleRate < 8000 || sampleRate > 96000) throw new ArgumentOutOfRangeException(nameof(sampleRate));
            double duration = cue == SoundCue.Menu ? .09 : cue == SoundCue.Success ? .6 : cue == SoundCue.Failure ? .45 : .24;
            int count = (int)(duration * sampleRate);
            var samples = new float[count];
            double frequency = cue == SoundCue.Menu ? 660 : cue == SoundCue.Decision ? 523.25 :
                cue == SoundCue.Maintenance ? 783.99 : cue == SoundCue.Emergency ? 440 : cue == SoundCue.Success ? 523.25 : 196;
            for (int i = 0; i < count; i++)
            {
                double t = i / (double)sampleRate;
                double envelope = Math.Pow(Math.Sin(Math.PI * i / (count - 1)), 2);
                double tone = Math.Sin(2 * Math.PI * frequency * t);
                if (cue == SoundCue.Success) tone = (tone + Math.Sin(2*Math.PI*659.25*t) + Math.Sin(2*Math.PI*783.99*t)) / 3;
                if (cue == SoundCue.Failure) tone = (tone + Math.Sin(2*Math.PI*146.83*t)) / 2;
                if (cue == SoundCue.Emergency) tone *= .65 + .35 * Math.Cos(2*Math.PI*8*t);
                samples[i] = (float)(.16 * envelope * tone);
            }
            return samples;
        }
    }
}
