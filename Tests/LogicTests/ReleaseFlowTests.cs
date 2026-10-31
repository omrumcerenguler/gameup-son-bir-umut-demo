using System;
using SonBirUmutDemo.Decisions;
using SonBirUmutDemo.Simulation;

internal static class ReleaseFlowTests
{
    public static void Register(Action<string,Action> run)
    {
        run("RC: main menu stays at a clean intro without starting clocks",()=>{
            var flow=new ReleaseFlow(); flow.Tick(500,false,true,true);
            Check(flow.Screen==ReleaseScreen.MainMenu && flow.MenuVisible && flow.MainMenuBackground);
            Check(flow.Run.Phase==GameRunPhase.Intro && flow.Run.MovementLocked && flow.Run.Session.History.Count==0);
            Near(flow.Run.RemainingSeconds,240); Near(flow.Run.CrisisPressure,60);
        });
        run("RC: New Game creates exactly one fresh run then starts once",()=>{
            int created=0; var flow=new ReleaseFlow(()=>{created++;return new GameRunState(GameMode.PlaytestDemo);});
            var menuRun=flow.Run; Command(flow,ReleaseCommand.NewGame);
            Check(created==2 && !ReferenceEquals(menuRun,flow.Run) && flow.Run.Phase==GameRunPhase.Intro);
            Check(!flow.Command(ReleaseCommand.NewGame) && created==2);
            Command(flow,ReleaseCommand.StartGameplay); Check(!flow.Command(ReleaseCommand.StartGameplay));
            Check(flow.Run.Phase==GameRunPhase.Preparation && !flow.MenuVisible);
        });
        run("RC: pause freezes both timer and pressure and prevents decisions",()=>{
            var flow=Playing(); flow.Tick(2,false); var resources=flow.Run.CurrentResources;
            Command(flow,ReleaseCommand.Pause); double time=flow.Run.RemainingSeconds,p=flow.Run.CrisisPressure;
            flow.Tick(500,false); Near(flow.Run.RemainingSeconds,time); Near(flow.Run.CrisisPressure,p);
            Check(flow.Run.MovementLocked && flow.Run.UserPaused);
            Check(!flow.Run.TryChoose(DecisionCatalog.EnergyStationId,DecisionCatalog.Solar,out _));
            Check(ReferenceEquals(resources,flow.Run.CurrentResources));
            var foundation=new GameRunState(GameMode.CrisisFoundation); foundation.Start(); Prepare(foundation);
            foundation.SetUserPaused(true); Check(!foundation.TryReducePressure(10)); Near(foundation.CrisisPressure,60);
        });
        run("RC: pause cancels repair and freezes incident without task rewards",()=>{
            var flow=Crisis(); flow.Tick(12,false); flow.Tick(1,false,true,true);
            var before=flow.Run.CurrentResources; double time=flow.Run.Crisis.Emergency.RemainingSeconds;
            Command(flow,ReleaseCommand.Pause); Near(flow.Run.Crisis.Emergency.RepairSeconds,0);
            flow.Tick(500,false,true,true); Near(flow.Run.Crisis.Emergency.RemainingSeconds,time);
            Check(!flow.Run.TryCompleteMaintenance(DecisionCatalog.EnergyStationId,out _));
            Check(ReferenceEquals(before,flow.Run.CurrentResources));
            Command(flow,ReleaseCommand.Resume); flow.Tick(500,false,true,true); // guarded first delta
            Near(flow.Run.Crisis.Emergency.RepairSeconds,0);
            flow.Tick(1.9,false,true,true); Check(flow.Run.Crisis.Emergency.Status==EmergencyStatus.Active);
            flow.Tick(.1,false,true,true); Check(flow.Run.Crisis.Emergency.Status==EmergencyStatus.Repaired);
        });
        run("RC: Escape closes cards before opening pause",()=>{
            var flow=Playing(); Check(flow.Escape(true)==EscapeResult.CloseCards);
            Check(flow.Screen==ReleaseScreen.Playing && !flow.Run.UserPaused);
            Check(flow.Escape(false)==EscapeResult.MenuChanged && flow.Screen==ReleaseScreen.Paused);
            Check(flow.Escape(false)==EscapeResult.MenuChanged && flow.Screen==ReleaseScreen.Playing);
        });
        run("RC: settings Escape returns to pause without resuming",()=>{
            var flow=Playing(); Command(flow,ReleaseCommand.Pause); Command(flow,ReleaseCommand.Settings);
            flow.Tick(500,false); Check(flow.Screen==ReleaseScreen.Settings && flow.Run.UserPaused);
            Check(flow.Escape(false)==EscapeResult.MenuChanged && flow.Screen==ReleaseScreen.Paused);
            Check(flow.Run.UserPaused); flow.Escape(false); Check(!flow.Run.UserPaused);
        });
        run("RC: How To and main settings never start or replace a session",()=>{
            var flow=new ReleaseFlow(); var before=flow.Run;
            Command(flow,ReleaseCommand.HowToPlay); flow.Tick(500,false); Check(flow.MainMenuBackground);
            Check(flow.Escape(false)==EscapeResult.MenuChanged && flow.Screen==ReleaseScreen.MainMenu);
            Command(flow,ReleaseCommand.Settings); flow.Tick(500,false); Command(flow,ReleaseCommand.Back);
            Check(flow.Screen==ReleaseScreen.MainMenu && ReferenceEquals(before,flow.Run)); Near(flow.Run.RemainingSeconds,240);
        });
        run("RC: user resume cannot cancel native suspension",()=>{
            var flow=Playing(); Command(flow,ReleaseCommand.Pause); flow.SetNativePaused(true);
            Command(flow,ReleaseCommand.Resume); flow.Tick(500,false); Near(flow.Run.RemainingSeconds,240);
            Check(!flow.Run.UserPaused); flow.SetNativePaused(false); flow.Tick(500,false); Near(flow.Run.RemainingSeconds,240);
            flow.Tick(1,false); Near(flow.Run.RemainingSeconds,239);
        });
        run("RC: native resume cannot cancel user pause",()=>{
            var flow=Playing(); flow.SetNativePaused(true); Command(flow,ReleaseCommand.Pause);
            flow.SetNativePaused(false); flow.Tick(500,false); Near(flow.Run.RemainingSeconds,240);
            Check(flow.Run.UserPaused); Command(flow,ReleaseCommand.Resume); flow.Tick(500,false);
            flow.Tick(1,false); Near(flow.Run.RemainingSeconds,239);
        });
        run("RC: focus cards and user pause compose without clock jumps",()=>{
            var flow=Playing(); Command(flow,ReleaseCommand.Pause); flow.SetFocus(false);
            flow.Tick(500,true); flow.SetFocus(true); flow.Tick(500,true);
            Command(flow,ReleaseCommand.Resume); flow.Tick(500,true); flow.Tick(500,false);
            Near(flow.Run.RemainingSeconds,240); flow.Tick(1,false); Near(flow.Run.RemainingSeconds,239);
        });
        run("RC: return to main menu clears choices tasks resources pressure and repair",()=>{
            var flow=Crisis(); Complete(flow.Run,DecisionCatalog.EnergyStationId); flow.Tick(12,false); flow.Tick(1,false,true,true);
            Command(flow,ReleaseCommand.Pause); var before=flow.Run; Command(flow,ReleaseCommand.MainMenu);
            Check(flow.Screen==ReleaseScreen.MainMenu && !ReferenceEquals(before,flow.Run)); CheckFresh(flow.Run);
            Check(!flow.Command(ReleaseCommand.MainMenu)); flow.Tick(500,false); CheckFresh(flow.Run);
        });
        run("RC: restart from pause creates fresh orientation instead of resuming old state",()=>{
            var flow=Crisis(); Complete(flow.Run,DecisionCatalog.EnergyStationId); Command(flow,ReleaseCommand.Pause);
            var old=flow.Run; Command(flow,ReleaseCommand.Restart); Check(!ReferenceEquals(old,flow.Run));
            CheckFresh(flow.Run); Check(flow.Screen==ReleaseScreen.Playing && !flow.Run.UserPaused);
            Check(!flow.Command(ReleaseCommand.Restart)); Command(flow,ReleaseCommand.StartGameplay);
        });
        run("RC: new session preserves external focus and native pause flags",()=>{
            var flow=Playing(); Command(flow,ReleaseCommand.Pause); flow.SetFocus(false); flow.SetNativePaused(true);
            Command(flow,ReleaseCommand.Restart); Command(flow,ReleaseCommand.StartGameplay); flow.Tick(500,false);
            Near(flow.Run.RemainingSeconds,240); flow.SetFocus(true); flow.Tick(500,false); Near(flow.Run.RemainingSeconds,240);
            flow.SetNativePaused(false); flow.Tick(500,false); flow.Tick(1,false); Near(flow.Run.RemainingSeconds,239);
        });
        run("RC: successful ending replays and returns via menu with no retained task state",()=>{
            var flow=Win(); Check(flow.Screen==ReleaseScreen.Ending && flow.Run.Result.CrisisStabilized);
            Command(flow,ReleaseCommand.Restart); CheckFresh(flow.Run); Command(flow,ReleaseCommand.StartGameplay);
            flow.Tick(240,false); Check(flow.Screen==ReleaseScreen.Ending && !flow.Run.Result.CrisisStabilized);
            Command(flow,ReleaseCommand.MainMenu); CheckFresh(flow.Run); Command(flow,ReleaseCommand.NewGame); CheckFresh(flow.Run);
        });
        run("RC: repeated consecutive sessions remain independent",()=>{
            var flow=new ReleaseFlow();
            for(int i=0;i<3;i++)
            {
                Command(flow,ReleaseCommand.NewGame); Command(flow,ReleaseCommand.StartGameplay); Prepare(flow.Run);
                Complete(flow.Run,DecisionCatalog.EnergyStationId); flow.Tick(240,false);
                Check(flow.Screen==ReleaseScreen.Ending && flow.Run.Result.Summary!=null);
                Command(flow,ReleaseCommand.MainMenu); CheckFresh(flow.Run);
            }
        });
        run("RC: unchanged five-task four-minimum and 45/2 balance",()=>{
            var flow=Crisis(); Check(flow.Run.Crisis.Tasks.Count==5 && flow.Run.Crisis.RequiredCount==4);
            Near(flow.Run.CrisisRules.EmergencyWindow,45); Near(flow.Run.CrisisRules.RepairDuration,2);
            Near(flow.Run.CrisisRules.StableDuration,10); Near(flow.Run.CrisisRules.StableThreshold,35);
        });
        run("RC: all 27 strategies preserve crisis success and the three village endings",()=>{
            int count=0,blocked=0;var endings=new System.Collections.Generic.HashSet<EndingCategory>();
            foreach(var e in DecisionCatalog.EnergyStation.Options)
            foreach(var w in DecisionCatalog.WaterStation.Options)
            foreach(var f in DecisionCatalog.FarmStation.Options)
            {
                var flow=Playing();
                foreach(string choice in new[]{e.Id,w.Id,f.Id})
                {
                    string point=flow.Run.Session.CurrentPoint.Id;
                    if(!flow.Run.TryChoose(point,choice,out _)){blocked++;Check(flow.Run.TryChoose(point,DecisionCatalog.Store,out _));}
                }
                foreach(string station in new[]{DecisionCatalog.EnergyStationId,DecisionCatalog.WaterStationId,
                    DecisionCatalog.FarmStationId,DecisionCatalog.EnergyStationId,DecisionCatalog.WaterStationId}) Complete(flow.Run,station);
                flow.Tick(12,false);flow.Tick(2,false,true,true);flow.Tick(10,false);
                Check(flow.Screen==ReleaseScreen.Ending && flow.Run.Result.CrisisStabilized);
                Check(flow.Run.Result.Category==EndingResult.Evaluate(flow.Run.CurrentResources).Category);
                endings.Add(flow.Run.Result.Category);count++;
            }
            Check(count==27 && blocked==2 && endings.Count==3);
        });
        run("RC: invalid menu commands do not mutate the run",()=>{
            var flow=new ReleaseFlow(); var initial=flow.Run;
            Check(!flow.Command(ReleaseCommand.Resume) && !flow.Command(ReleaseCommand.Restart) && !flow.Command(ReleaseCommand.StartGameplay));
            Check(!flow.Command((ReleaseCommand)99) && flow.Escape(false)==EscapeResult.None && ReferenceEquals(initial,flow.Run));
            Reject(()=>new ReleaseFlow(()=>new GameRunState(GameMode.LegacyDecisions)));
            Reject(()=>new ReleaseFlow(()=>null));
        });
        run("RC: master volume clamps finite values and rejects invalid input",()=>{
            var audio=new SoundPreferences(); Near(audio.Volume,.65); audio.SetVolume(-2); Near(audio.EffectiveVolume,0);
            audio.SetVolume(2); Near(audio.Volume,1); audio.SetVolume(.3); Near(audio.EffectiveVolume,.3);
            Reject(()=>audio.SetVolume(double.NaN)); Reject(()=>audio.SetVolume(double.PositiveInfinity)); Near(audio.Volume,.3);
        });
        run("RC: mute keeps the chosen volume across gameplay resets",()=>{
            var audio=new SoundPreferences(.4,true); var flow=Playing(); Near(audio.EffectiveVolume,0);
            Command(flow,ReleaseCommand.Pause); Command(flow,ReleaseCommand.Restart); Near(audio.Volume,.4); Check(audio.Muted);
            audio.SetMuted(false); Near(audio.EffectiveVolume,.4); audio.SetMuted(true); audio.SetVolume(.8);
            Near(audio.EffectiveVolume,0); audio.SetMuted(false); Near(audio.EffectiveVolume,.8);
            var restored=new SoundPreferences(audio.Volume,true); Check(restored.Muted); Near(restored.Volume,.8);
        });
        run("RC: generated cues are short finite bounded mono sample data",()=>{
            foreach(SoundCue cue in Enum.GetValues(typeof(SoundCue)))
            {
                var samples=SoundSynthesis.Generate(cue); Check(samples.Length>1000 && samples.Length<=22050);
                double energy=0;
                foreach(float x in samples){Check(!float.IsNaN(x) && !float.IsInfinity(x) && Math.Abs(x)<=.16001);energy+=x*x;}
                Check(energy>0); Near(samples[0],0); Near(samples[samples.Length-1],0);
            }
            Reject(()=>SoundSynthesis.Generate((SoundCue)99)); Reject(()=>SoundSynthesis.Generate(SoundCue.Menu,1));
        });
    }
    private static ReleaseFlow Playing(){var f=new ReleaseFlow();Command(f,ReleaseCommand.NewGame);Command(f,ReleaseCommand.StartGameplay);return f;}
    private static ReleaseFlow Crisis(){var f=Playing();Prepare(f.Run);return f;}
    private static ReleaseFlow Win()
    {
        var f=Crisis(); foreach(string id in new[]{DecisionCatalog.EnergyStationId,DecisionCatalog.WaterStationId,DecisionCatalog.FarmStationId,DecisionCatalog.EnergyStationId})Complete(f.Run,id);
        f.Tick(12,false);f.Tick(2,false,true,true);f.Tick(10,false);return f;
    }
    private static void Prepare(GameRunState r){foreach(string id in new[]{DecisionCatalog.Solar,DecisionCatalog.Efficiency,DecisionCatalog.Protect})Check(r.TryChoose(r.Session.CurrentPoint.Id,id,out _));}
    private static void Complete(GameRunState r,string station)=>Check(r.TryCompleteMaintenance(station,out _));
    private static void Command(ReleaseFlow flow,ReleaseCommand command)=>Check(flow.Command(command));
    private static void CheckFresh(GameRunState r)
    {
        Check(r.Phase==GameRunPhase.Intro && r.Result==null && r.Session.History.Count==0 && r.Crisis.CompletedCount==0);
        Check(r.Crisis.ActiveCount==0 && r.Crisis.Emergency.Status==EmergencyStatus.Waiting);
        Near(r.RemainingSeconds,240);Near(r.CrisisPressure,60);Near(r.Crisis.Emergency.RepairSeconds,0);Near(r.Crisis.StableSeconds,0);
        Check(r.CurrentResources.Budget==100 && r.CurrentResources.Water==40 && r.CurrentResources.PlantHealth==55);
    }
    private static void Check(bool condition){if(!condition)throw new Exception("Assertion failed");}
    private static void Near(double a,double b)=>Check(Math.Abs(a-b)<.000001);
    private static void Reject(Action action){try{action();}catch(ArgumentException){return;}throw new Exception("Expected validation failure");}
}
