using System;
using System.Collections.Generic;
using SonBirUmutDemo.Core;
using SonBirUmutDemo.Decisions;
using SonBirUmutDemo.Simulation;

internal static class FinalPolishTests
{
    public static void Register(Action<string,Action> test)
    {
        test("Polish: independent music/effects finite bounds preserve legacy master",()=>{
            var s=new SoundPreferences();Near(s.Volume,.65);Near(s.EffectsVolume,1);Near(s.MusicVolume,.55);
            s.SetMusicVolume(-1);s.SetEffectsVolume(4);Near(s.MusicVolume,0);Near(s.EffectsVolume,1);
            Reject(()=>s.SetMusicVolume(double.NaN));Reject(()=>s.SetEffectsVolume(double.PositiveInfinity));
            Near(s.Volume,.65);Near(s.EffectsVolume,1);s.SetMusicVolume(1);s.SetEffectsVolume(0);
            Near(s.EffectiveMusic,.65);Near(s.EffectiveEffects,0);
        });
        test("Polish: all three gains and mute round trip independently",()=>{
            var s=new SoundPreferences(.7,true,.2,.6);
            Near(s.EffectiveVolume,0);Near(s.EffectiveMusic,0);Near(s.EffectiveEffects,0);
            s.SetMuted(false);Near(s.EffectiveMusic,.14);Near(s.EffectiveEffects,.42);
            s.SetMuted(true);s.SetMusicVolume(.4);s.SetEffectsVolume(.3);
            var restored=new SoundPreferences(s.Volume,s.Muted,s.MusicVolume,s.EffectsVolume);
            restored.SetMuted(false);Near(restored.EffectiveMusic,.28);Near(restored.EffectiveEffects,.21);
        });
        test("Polish: loop transport starts once across hundreds of updates",()=>{
            var transport=new LoopPlayback();Check(transport.SetActive(true)==LoopAction.Start);
            for(int i=0;i<1000;i++)Check(transport.SetActive(true)==LoopAction.None);
            Check(transport.SetActive(false)==LoopAction.Stop);Check(transport.SetActive(false)==LoopAction.None);
            Check(transport.SetActive(true)==LoopAction.Start);
        });
        test("Polish: menu intro play pause cards focus native map to bounded mix",()=>{
            var flow=new ReleaseFlow(()=>new GameRunState(GameMode.FinalPacing));
            Check(Mix(flow).Mood==AudioMood.Menu && Mix(flow).Music==1);
            flow.Command(ReleaseCommand.NewGame);Check(Mix(flow).Mood==AudioMood.Menu);
            flow.Command(ReleaseCommand.StartGameplay);Check(Mix(flow).Mood==AudioMood.Playing && Mix(flow).Music==0);
            Check(AudioMix.Evaluate(flow.Run,false,true,true,false).Mood==AudioMood.Cards);
            flow.Command(ReleaseCommand.Pause);Check(Mix(flow).Mood==AudioMood.Paused && Mix(flow).Tension==0);
            Check(AudioMix.Evaluate(flow.Run,false,false,false,false).Mood==AudioMood.Silent);
            Check(AudioMix.Evaluate(flow.Run,false,false,true,true).Mood==AudioMood.Silent);
            Check(AudioMix.Evaluate(null,false,false,true,false).Mood==AudioMood.Silent);
        });
        test("Polish: restart keeps preferences and transport but clears old crisis mood",()=>{
            var prefs=new SoundPreferences(.6,true,.2,.4);var flow=Playing();Prepare(flow.Run);
            flow.Tick(100,false);flow.Command(ReleaseCommand.Pause);var transport=new LoopPlayback();transport.SetActive(true);
            Check(flow.Command(ReleaseCommand.Restart));Check(Mix(flow).Mood==AudioMood.Menu);
            Check(transport.SetActive(true)==LoopAction.None);Near(prefs.MusicVolume,.2);Near(prefs.EffectsVolume,.4);Check(prefs.Muted);
            flow.Command(ReleaseCommand.StartGameplay);flow.Command(ReleaseCommand.Pause);flow.Command(ReleaseCommand.MainMenu);
            Check(Mix(flow).Mood==AudioMood.Menu && flow.Run.Crisis.CompletedCount==0);
        });
        test("Polish: pressure sound is bounded monotonic and not an emission input",()=>{
            double previous=-1;
            for(int p=0;p<=100;p+=5)
            {
                var r=new GameRunState(GameMode.FinalPacing,new GameRunSettings(initialPressure:p));r.Start();
                var mix=AudioMix.Evaluate(r,false,false,true,false);
                Check(mix.Tension>=previous && mix.Tension<=1 && mix.Ambience==1);previous=mix.Tension;
            }
            var clean=new GameRunState(GameMode.FinalPacing);clean.Start();
            var dirty=new GameRunState(GameMode.FinalPacing,initial:new ResourceState(100,20,40,55,100));dirty.Start();
            Near(AudioMix.Evaluate(clean,false,false,true,false).Tension,AudioMix.Evaluate(dirty,false,false,true,false).Tension);
        });
        test("Polish: effect gate rejects same click and caps two concurrent voices",()=>{
            var gate=new SoundEventGate();Check(gate.TryBegin(SoundCue.Menu,1,0,.1,out _));
            Check(!gate.TryBegin(SoundCue.Menu,1,.01,.1,out _));
            Check(!gate.TryBegin(SoundCue.Menu,2,.02,.1,out _));
            Check(gate.TryBegin(SoundCue.Decision,2,.02,.24,out _));
            Check(!gate.TryBegin(SoundCue.Maintenance,3,.03,.24,out _));
            Check(gate.TryBegin(SoundCue.Maintenance,4,.11,.24,out _));
        });
        test("Polish: emergency and ending replace older cues without a burst",()=>{
            var gate=new SoundEventGate();gate.TryBegin(SoundCue.Menu,1,1,.1,out _);gate.TryBegin(SoundCue.Maintenance,1,1,.24,out _);
            Check(gate.TryBegin(SoundCue.Emergency,2,1.01,.24,out bool interrupt) && interrupt);
            Check(!gate.TryBegin(SoundCue.Emergency,2,1.01,.24,out _));
            Check(gate.TryBegin(SoundCue.Success,3,1.02,.6,out interrupt) && interrupt);
            gate.Reset();Check(gate.TryBegin(SoundCue.Menu,3,1.02,.1,out interrupt) && !interrupt);
        });
        test("Polish: effect gate invalid values fail without corrupting future cues",()=>{
            var gate=new SoundEventGate();Reject(()=>gate.TryBegin((SoundCue)99,1,0,.1,out _));
            Reject(()=>gate.TryBegin(SoundCue.Menu,1,double.NaN,.1,out _));
            Reject(()=>gate.TryBegin(SoundCue.Menu,1,0,-1,out _));Check(gate.TryBegin(SoundCue.Menu,1,0,.1,out _));
        });
        test("Polish: final minute and final seconds use exact warning boundaries",()=>{
            Check(PresentationRules.Warning(60.01)==TimeWarning.None);Check(PresentationRules.Warning(60)==TimeWarning.LastMinute);
            Check(PresentationRules.Warning(20.01)==TimeWarning.LastMinute);Check(PresentationRules.Warning(20)==TimeWarning.FinalSeconds);
            Check(PresentationRules.Warning(0)==TimeWarning.FinalSeconds);Reject(()=>PresentationRules.Warning(-1));
        });
        test("Polish: How To states real ten-six-four rules and legacy counts",()=>{
            var pacing=new GameRunState(GameMode.FinalPacing);string text=DemoStory.HowTo(pacing);
            foreach(string expected in new[]{"10 ayrı","6 bakım","En az 4","240","45","2 sn","WASD","Shift","ENERJİ","Esc"})Check(text.Contains(expected),expected);
            Check(text.Length<620 && text.Split('\n').Length<=15);
            var old=new GameRunState(GameMode.PlaytestDemo);Check(DemoStory.HowTo(old).Contains("5 bakım görevi") && !DemoStory.HowTo(old).Contains("10 ayrı"));
        });
        test("Polish: 27 routed strategies keep all endings with correct sound and reason",()=>{
            var categories=new HashSet<EndingCategory>();int count=0;
            foreach(var e in DecisionCatalog.EnergyStation.Options)
            foreach(var w in DecisionCatalog.WaterStation.Options)
            foreach(var f in DecisionCatalog.FarmStation.Options)
            {
                var r=PacingRouteSimulation.Simulate(0,PacingRouteSimulation.Profiles[1],e.Id,w.Id,f.Id);
                var mix=AudioMix.Evaluate(r.Run,false,false,true,false);categories.Add(r.Run.Result.Category);count++;
                Check(mix.Mood== (r.Run.Result.Category==EndingCategory.Rebirth ? AudioMood.PositiveEnding : r.Run.Result.Category==EndingCategory.PartialTransition ? AudioMood.PartialEnding : AudioMood.NegativeEnding));
                Check(PresentationRules.EndingSound(r.Run.Result.Category)==(r.Run.Result.Category==EndingCategory.Rebirth ? SoundCue.Success : SoundCue.Failure));
                Check(!string.IsNullOrEmpty(PresentationRules.EndingReason(r.Run.Result)));
                if(r.Run.Result.Category==EndingCategory.Rebirth)Near(mix.Tension,0);
            }
            Check(count==27 && categories.Count==3);
        });
        test("Polish: reason reflects actual diesel emissions and failed management",()=>{
            var diesel=PacingRouteSimulation.Simulate(0,PacingRouteSimulation.Profiles[1],DecisionCatalog.Diesel,DecisionCatalog.Efficiency,DecisionCatalog.Protect);
            Check(diesel.Run.Result.Category!=EndingCategory.Rebirth && PresentationRules.EndingReason(diesel.Run.Result).Contains("Emisyon"));
            var r=new GameRunState(GameMode.FinalPacing);r.Start();r.Tick(240,false);
            Check(!r.Result.CrisisStabilized && PresentationRules.EndingReason(r.Result).Contains("Nöbet"));
        });
        test("Polish: focus stop then restore never removes user pause or advances time",()=>{
            var f=Playing();Prepare(f.Run);f.Tick(12,false);f.Tick(1,false,true,true);f.Command(ReleaseCommand.Pause);
            var transport=new LoopPlayback();transport.SetActive(true);f.SetFocus(false);
            Check(transport.SetActive(false)==LoopAction.Stop);f.Tick(500,false,true,true);
            f.SetFocus(true);Check(f.Run.UserPaused && Mix(f).Mood==AudioMood.Paused);
            Check(transport.SetActive(true)==LoopAction.Start);double before=f.Run.RemainingSeconds;
            f.Tick(500,false);Near(f.Run.RemainingSeconds,before);
        });
    }
    private static AudioMix Mix(ReleaseFlow f)=>AudioMix.Evaluate(f.Run,f.MainMenuBackground,false,true,false);
    private static ReleaseFlow Playing(){var f=new ReleaseFlow(()=>new GameRunState(GameMode.FinalPacing));f.Command(ReleaseCommand.NewGame);f.Command(ReleaseCommand.StartGameplay);return f;}
    private static void Prepare(GameRunState r){foreach(string id in new[]{DecisionCatalog.Solar,DecisionCatalog.Efficiency,DecisionCatalog.Protect})Check(r.TryChoose(r.Session.CurrentPoint.Id,id,out _));}
    private static void Near(double a,double b)=>Check(Math.Abs(a-b)<1e-6,$"{a} != {b}");
    private static void Check(bool ok,string message="Assertion failed"){if(!ok)throw new Exception(message);}
    private static void Reject(Action a){try{a();}catch(ArgumentException){return;}throw new Exception("Expected validation error");}
}
