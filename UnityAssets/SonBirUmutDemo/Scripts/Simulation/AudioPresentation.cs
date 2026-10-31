using System;
using System.Collections.Generic;

namespace SonBirUmutDemo.Simulation
{
    public enum AudioMood { Silent, Menu, Playing, Cards, Paused, PositiveEnding, PartialEnding, NegativeEnding }
    public enum LoopAction { None, Start, Stop }

    // Owns a transport transition, never creates a source/clip. Session reset keeps loop phase.
    public sealed class LoopPlayback
    {
        private bool running;
        public LoopAction SetActive(bool active)
        {
            if(active==running) return LoopAction.None;
            running=active;return active ? LoopAction.Start : LoopAction.Stop;
        }
    }

    public readonly struct AudioMix
    {
        public AudioMood Mood { get; }
        public double Music { get; }
        public double Ambience { get; }
        public double Tension { get; }
        private AudioMix(AudioMood mood,double music,double ambience,double tension)
        { Mood=mood;Music=music;Ambience=ambience;Tension=tension; }
        public static AudioMix Evaluate(GameRunState run,bool menuBackground,bool cards,bool focused,bool nativePaused)
        {
            if(run==null || !focused || nativePaused) return new AudioMix(AudioMood.Silent,0,0,0);
            double pressure=Math.Max(0,Math.Min(1,(run.CrisisPressure-55)/45));
            if(menuBackground || run.Phase==GameRunPhase.Intro) return new AudioMix(AudioMood.Menu,1,0,0);
            if(run.UserPaused) return new AudioMix(AudioMood.Paused,.35,.12,0);
            if(run.IsEnded)
            {
                if(run.Result.Category==EndingCategory.Rebirth) return new AudioMix(AudioMood.PositiveEnding,.7,.25,0);
                if(run.Result.Category==EndingCategory.PartialTransition) return new AudioMix(AudioMood.PartialEnding,.2,.5,.12*pressure);
                return new AudioMix(AudioMood.NegativeEnding,0,.6,.4*pressure);
            }
            if(cards) return new AudioMix(AudioMood.Cards,0,.3,0);
            return new AudioMix(AudioMood.Playing,0,1,pressure*pressure);
        }
    }

    public sealed class SoundEventGate
    {
        private readonly List<double> ends=new List<double>();
        private readonly Dictionary<SoundCue,(int frame,double time)> last=new Dictionary<SoundCue,(int,double)>();
        public bool TryBegin(SoundCue cue,int frame,double time,double duration,out bool interrupt)
        {
            RunNumbers.NonNegative(time,nameof(time));RunNumbers.NonNegative(duration,nameof(duration));
            if(!Enum.IsDefined(typeof(SoundCue),cue)) throw new ArgumentOutOfRangeException(nameof(cue));
            interrupt=false;
            if(last.TryGetValue(cue,out var before) && (before.frame==frame || time-before.time<.075)) return false;
            ends.RemoveAll(end=>end<=time);
            bool priority=cue==SoundCue.Emergency || cue==SoundCue.Success || cue==SoundCue.Failure;
            if(priority && ends.Count>0) { interrupt=true;ends.Clear(); }
            if(ends.Count>=2) return false;
            last[cue]=(frame,time);ends.Add(time+duration);return true;
        }
        public void Reset() {ends.Clear();last.Clear();}
    }

    public enum TimeWarning { None, LastMinute, FinalSeconds }
    public static class PresentationRules
    {
        public static TimeWarning Warning(double seconds)
        {
            RunNumbers.NonNegative(seconds,nameof(seconds));
            return seconds<=20 ? TimeWarning.FinalSeconds : seconds<=60 ? TimeWarning.LastMinute : TimeWarning.None;
        }
        public static string EndingReason(RunOutcome result)
        {
            if(result==null) throw new ArgumentNullException(nameof(result));
            var state=result.Resources;
            if(result.Category==EndingCategory.Rebirth) return "Korunan su ve yaşam, köyü kurak döneme hazırladı.";
            if(state.Water<20) return "Su rezervi kritik düzeyde kaldı.";
            if(state.PlantHealth<30) return "Bitki yaşamı yeterince korunamadı.";
            if(state.Emissions>12) return "Emisyon yükü uzun vadeli dönüşümü sınırlıyor.";
            if(state.Water<35) return "Kurak dönem için su rezervi yetersiz kaldı.";
            if(state.PlantHealth<55) return "Bitki sağlığı hâlâ kırılgan.";
            if(state.Energy<15) return "Enerji kapasitesi yetersiz kaldı.";
            if(state.Budget<10 && state.Water<50) return "Kaynak tamponu yetersiz kaldı.";
            return "Nöbet istikrara kavuşmadı; müdahalelerini yeniden planlayabilirsin.";
        }
        public static SoundCue EndingSound(EndingCategory category)
        {
            if(!Enum.IsDefined(typeof(EndingCategory),category)) throw new ArgumentOutOfRangeException(nameof(category));
            return category==EndingCategory.Rebirth ? SoundCue.Success : SoundCue.Failure;
        }
    }
}
