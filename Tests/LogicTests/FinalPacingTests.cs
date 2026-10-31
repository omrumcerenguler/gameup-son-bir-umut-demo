using System;
using System.Collections.Generic;
using System.Linq;
using SonBirUmutDemo.Decisions;
using SonBirUmutDemo.Simulation;

internal static class FinalPacingTests
{
    public static void Register(Action<string,Action> test)
    {
        test("Pacing: ten unique tasks and distinct physical locations",()=>{
            var all=FinalPacingCatalog.All;Check(all.Count==10);
            Check(all.Select(t=>t.TaskId).Distinct().Count()==10 && all.Select(t=>t.LocationId).Distinct().Count()==10);
            foreach(var t in all) {Check(PacingRouteSimulation.Clear(t.Position));Check(FinalPacingCatalog.At(t.LocationId)==t);}
            for(int i=0;i<10;i++) for(int j=0;j<i;j++) Check(all[i].Position.DistanceTo(all[j].Position)>3.3);
        });
        test("Pacing: 54 distinct balanced six-task sessions",()=>{
            var sets=new HashSet<string>();
            for(int seed=0;seed<54;seed++)
            {
                var tasks=FinalPacingCatalog.Select(seed);Check(tasks.Count==6 && tasks.Select(t=>t.LocationId).Distinct().Count()==6);
                foreach(TaskRegion region in Enum.GetValues(typeof(TaskRegion))) Check(tasks.Count(t=>t.Region==region)==2);
                sets.Add(string.Join(",",tasks.Select(t=>t.TaskId).OrderBy(s=>s)));
                Check(tasks.Sum(t=>t.Effects.WaterProtection)==4 && tasks.Sum(t=>t.Effects.PlantSupport)==4);
            }
            Check(sets.Count==54 && FinalPacingCatalog.Select(-1).Count==6);
        });
        test("Pacing: node profile has six tasks and four minimum, RC still has five",()=>{
            var r=Crisis();Check(r.Crisis.Tasks.Count==6 && r.Crisis.RequiredCount==4 && r.Crisis.ActiveCount==2);
            Check(r.Crisis.Tasks.All(t=>t.StationId==null && t.LocationId!=null));
            var old=new GameRunState(GameMode.PlaytestDemo);Check(old.Crisis.Tasks.Count==5 && old.Crisis.RequiredCount==4 && !old.UsesTaskNodes);
        });
        test("Pacing: wrong TaskId, LocationId, range and modal cannot award",()=>{
            var r=Crisis();var t=Available(r);var state=r.CurrentResources;
            Check(!r.TryCompleteMaintenance(DecisionCatalog.EnergyStationId,out _));
            Check(!r.TryCompleteTask("wrong",t.LocationId,true,false,out _));
            Check(!r.TryCompleteTask(t.Id,"wrong",true,false,out _));
            var other=r.Crisis.Tasks.First(x=>x.LocationId!=t.LocationId);
            Check(!r.TryCompleteTask(t.Id,other.LocationId,true,false,out _));
            Check(!r.TryCompleteTask(t.Id,t.LocationId,false,false,out _));
            Check(!r.TryCompleteTask(t.Id,t.LocationId,true,true,out _));
            Check(ReferenceEquals(state,r.CurrentResources) && r.Crisis.CompletedCount==0);
        });
        test("Pacing: only active tasks award once and resources stay bounded",()=>{
            var r=Crisis();var locked=r.Crisis.Tasks.First(t=>t.Status==MaintenanceStatus.Locked);
            Check(!r.TryCompleteTask(locked.Id,locked.LocationId,true,false,out _));
            var t=Available(r);Check(r.TryCompleteTask(t.Id,t.LocationId,true,false,out _));
            var state=r.CurrentResources;double p=r.CrisisPressure;
            Check(!r.TryCompleteTask(t.Id,t.LocationId,true,false,out _));
            Check(ReferenceEquals(state,r.CurrentResources) && p==r.CrisisPressure);
            Check(state.Water<=100 && state.PlantHealth<=100);
        });
        test("Pacing: soft dispatch times, only two active, no individual expiry",()=>{
            var r=Crisis();var first=Available(r);Complete(r,first);Complete(r,Available(r));
            Check(r.Crisis.ActiveCount==0);Near(r.Crisis.NextDispatchIn,25);
            r.Tick(12,false);r.Tick(2,false,true,true);r.Tick(10.9,false);Check(r.Crisis.ActiveCount==0);
            r.Tick(.1,false);Check(r.Crisis.ActiveCount==1);Near(Available(r).AvailableSince,25);
            r.Tick(60,false);Check(r.Crisis.ActiveCount==2 && first.Status==MaintenanceStatus.Completed);
            Check(r.Crisis.Tasks.Count(t=>t.Status==MaintenanceStatus.Available)==2);
        });
        test("Pacing: task promotion respects completion order and dates",()=>{
            for(int seed=0;seed<54;seed++)
            {
                var r=Crisis(seed);var second=r.Crisis.Tasks[1];Complete(r,second);
                Check(r.Crisis.Tasks[0].Status==MaintenanceStatus.Available && r.Crisis.Tasks[2].Status==MaintenanceStatus.Locked);
                r.Tick(25,false);Check(r.Crisis.Tasks[2].Status==MaintenanceStatus.Available && r.Crisis.ActiveCount==2);
            }
        });
        test("Pacing: emergency takes priority and cancels task rewards",()=>{
            var r=Crisis();var t=Available(r);r.Tick(12,false);Check(r.Crisis.Emergency.Status==EmergencyStatus.Active);
            Check(!r.TryCompleteTask(t.Id,t.LocationId,true,false,out _));
            Check(InteractionPolicy.Resolve(false,true,false,true)==InteractionKind.Emergency);
            Check(InteractionPolicy.Resolve(true,true,true,true)==InteractionKind.Modal);
            r.Tick(2,false,true,true);Complete(r,t);
        });
        test("Pacing: pause card focus stop dispatch and emergency clocks",()=>{
            var f=Flow();Prepare(f.Run);f.Tick(12,false);f.Tick(1,false,true,true);
            f.Command(ReleaseCommand.Pause);double time=f.Run.RemainingSeconds,elapsed=f.Run.Crisis.ElapsedSeconds;
            f.Tick(100,false,true,true);Near(f.Run.RemainingSeconds,time);Near(f.Run.Crisis.ElapsedSeconds,elapsed);
            Near(f.Run.Crisis.Emergency.RepairSeconds,0);
            f.Command(ReleaseCommand.Resume);f.Tick(100,false);f.Tick(100,true);
            Near(f.Run.RemainingSeconds,time);f.SetFocus(false);f.Tick(100,false);f.SetFocus(true);f.Tick(100,false);
            Near(f.Run.RemainingSeconds,time);f.Tick(.25,false);Near(f.Run.RemainingSeconds,time-.25);
        });
        test("Pacing: main menu settings and Escape preserve the release shell",()=>{
            var f=new ReleaseFlow(()=>new GameRunState(GameMode.FinalPacing));f.Tick(500,false);
            Near(f.Run.RemainingSeconds,240);f.Command(ReleaseCommand.Settings);f.Escape(false);
            Check(f.Screen==ReleaseScreen.MainMenu);f.Command(ReleaseCommand.NewGame);f.Command(ReleaseCommand.StartGameplay);
            Check(f.Escape(true)==EscapeResult.CloseCards && f.Screen==ReleaseScreen.Playing);
            f.Escape(false);Check(f.Run.UserPaused);f.Command(ReleaseCommand.Settings);f.Escape(false);
            Check(f.Screen==ReleaseScreen.Paused && f.Run.UserPaused);
        });
        test("Pacing: timeout from partial preparations and missed emergency is valid",()=>{
            for(int choices=0;choices<=3;choices++)
            {
                var r=new GameRunState(GameMode.FinalPacing);r.Start();
                if(choices>0) Check(r.TryChoose(DecisionCatalog.EnergyStationId,DecisionCatalog.Solar,out _));
                if(choices>1) Check(r.TryChoose(DecisionCatalog.WaterStationId,DecisionCatalog.Efficiency,out _));
                if(choices>2) Check(r.TryChoose(DecisionCatalog.FarmStationId,DecisionCatalog.Protect,out _));
                r.Tick(300,false);Check(r.IsEnded && r.Result!=null && !r.Result.CrisisStabilized);
            }
        });
        test("Pacing: reset clears selection statuses, time, resources and repair",()=>{
            var r=Crisis();Complete(r,Available(r));r.Tick(12,false);r.Tick(1,false,true,true);r.Reset();
            Check(r.Phase==GameRunPhase.Intro && r.Crisis.CompletedCount==0 && r.Crisis.ActiveCount==0);
            Check(r.Crisis.Tasks.Count==6 && r.Session.CompletedCount==0);Near(r.RemainingSeconds,240);Near(r.CrisisPressure,60);
            Near(r.Crisis.ElapsedSeconds,0);Near(r.Crisis.Emergency.RepairSeconds,0);
        });
        test("Pacing: restart and main-new cycle vary tasks without reusing state",()=>{
            int seed=0;var f=new ReleaseFlow(()=>new GameRunState(GameMode.FinalPacing,taskSeed:seed++));
            for(int i=0;i<3;i++)
            {
                Check(f.Command(ReleaseCommand.NewGame));Check(f.Command(ReleaseCommand.StartGameplay));Prepare(f.Run);
                var before=f.Run;Complete(before,Available(before));Check(f.Command(ReleaseCommand.Pause));
                Check(f.Command(ReleaseCommand.Restart) && !ReferenceEquals(before,f.Run));
                Check(f.Run.Crisis.CompletedCount==0 && f.Run.Crisis.Tasks.Count==6);Check(f.Command(ReleaseCommand.StartGameplay));
                Check(f.Command(ReleaseCommand.Pause));Check(f.Command(ReleaseCommand.MainMenu));
            }
        });
        test("Pacing: route graph connects every point with conservative obstacles",()=>{
            var locations=FinalPacingCatalog.All.Select(t=>t.Position).Concat(new[]{FinalPacingCatalog.Spawn,FinalPacingCatalog.EnergyDecision,FinalPacingCatalog.WaterDecision,FinalPacingCatalog.FarmDecision}).ToList();
            foreach(var a in locations) foreach(var b in locations)
            {double d=PacingRouteSimulation.Distance(a,b);Check(d>=a.DistanceTo(b)-1e-6 && d<100);}
        });
        test("Pacing: all 54 sets have a routed new-player success for every energy",()=>{
            foreach(var energy in new[]{DecisionCatalog.Solar,DecisionCatalog.Diesel,DecisionCatalog.Conservation})
            for(int seed=0;seed<54;seed++)
            {
                var result=PacingRouteSimulation.Simulate(seed,PacingRouteSimulation.Profiles[1],energy,DecisionCatalog.Efficiency,DecisionCatalog.Protect);
                Check(result.Run.Result.CrisisStabilized,$"{energy} seed {seed}: {result.ActiveSeconds:0.0}s, {result.Visits} tasks, pressure {result.Run.CrisisPressure:0.0}");
                Check(result.Visits>=4 && result.Distance>60 && result.EmergencyTravel+5+2<45);
            }
        });
        test("Pacing: routed fast player beats normal, delayed player can lose",()=>{
            var fast=PacingRouteSimulation.Simulate(0,PacingRouteSimulation.Profiles[0],DecisionCatalog.Solar,DecisionCatalog.Efficiency,DecisionCatalog.Protect);
            var normal=PacingRouteSimulation.Simulate(0,PacingRouteSimulation.Profiles[1],DecisionCatalog.Solar,DecisionCatalog.Efficiency,DecisionCatalog.Protect);
            var slow=PacingRouteSimulation.Simulate(0,PacingRouteSimulation.Profiles[2],DecisionCatalog.Solar,DecisionCatalog.Efficiency,DecisionCatalog.Protect);
            Check(fast.Run.Result.CrisisStabilized && normal.Run.Result.CrisisStabilized && fast.ActiveSeconds<normal.ActiveSeconds);
            Check(slow.Run.IsEnded && !slow.Run.Result.CrisisStabilized);
        });
        test("Pacing: 27 routed strategies retain all three final categories",()=>{
            var endings=new HashSet<EndingCategory>();int n=0;
            foreach(var e in DecisionCatalog.EnergyStation.Options)
            foreach(var w in DecisionCatalog.WaterStation.Options)
            foreach(var f in DecisionCatalog.FarmStation.Options)
            {
                var result=PacingRouteSimulation.Simulate(0,PacingRouteSimulation.Profiles[1],e.Id,w.Id,f.Id);
                Check(result.Run.IsEnded && result.Run.Result!=null);endings.Add(result.Run.Result.Category);n++;
                if(e.Id==DecisionCatalog.Diesel) Check(result.Run.CurrentResources.Emissions==result.Run.Session.State.Emissions && result.Run.CurrentResources.Emissions>=16 && result.Run.Result.Category!=EndingCategory.Rebirth);
            }
            Check(n==27 && endings.Count==3);
        });
        test("Pacing: all six rewards are capped and never alter budget energy emissions",()=>{
            var r=new GameRunState(GameMode.FinalPacing,initial:new ResourceState(999,100,100,100,0),crisisRules:new CrisisRules(stableDuration:500));
            r.Start();Prepare(r);var before=r.CurrentResources;r.Tick(12,false);r.Tick(2,false,true,true);
            while(r.Crisis.CompletedCount<6)
            {
                foreach(var task in r.Crisis.Tasks.Where(t=>t.Status==MaintenanceStatus.Available).ToArray()) Complete(r,task);
                if(r.Crisis.CompletedCount<6) r.Tick(r.Crisis.NextDispatchIn,false);
            }
            Check(r.CurrentResources.Water==Math.Min(100,before.Water+4));
            Check(r.CurrentResources.PlantHealth==Math.Min(100,before.PlantHealth+4));
            Check(r.CurrentResources.Budget==before.Budget && r.CurrentResources.Energy==before.Energy && r.CurrentResources.Emissions==before.Emissions);
        });
        test("Pacing: four tasks alone cannot bypass pressure and stability",()=>{
            var r=new GameRunState(GameMode.FinalPacing,new GameRunSettings(initialPressure:100));r.Start();Prepare(r);
            r.Tick(12,false);r.Tick(2,false,true,true);
            while(r.Crisis.CompletedCount<4)
            {
                foreach(var task in r.Crisis.Tasks.Where(t=>t.Status==MaintenanceStatus.Available).ToArray()) Complete(r,task);
                if(r.Crisis.CompletedCount<4) r.Tick(r.Crisis.NextDispatchIn,false);
            }
            r.Tick(10,false);Check(!r.IsEnded && r.CrisisPressure>35);Near(r.Crisis.StableSeconds,0);
        });
        test("Pacing: routed final replays and returns to menu with a fresh six-task run",()=>{
            var result=PacingRouteSimulation.Simulate(0,PacingRouteSimulation.Profiles[1],DecisionCatalog.Solar,DecisionCatalog.Efficiency,DecisionCatalog.Protect);
            var f=result.Flow;Check(f.Screen==ReleaseScreen.Ending && f.Run.Result.CrisisStabilized);
            var previous=f.Run;Check(f.Command(ReleaseCommand.Restart));Check(!ReferenceEquals(previous,f.Run));
            Check(f.Run.Crisis.Tasks.Count==6 && f.Run.Crisis.CompletedCount==0 && f.Run.Phase==GameRunPhase.Intro);
            Check(f.Command(ReleaseCommand.StartGameplay));Check(f.Command(ReleaseCommand.Pause));Check(f.Command(ReleaseCommand.MainMenu));
            Check(f.Screen==ReleaseScreen.MainMenu);Near(f.Run.RemainingSeconds,240);f.Tick(500,false);Near(f.Run.RemainingSeconds,240);
        });
        test("Pacing: finite emergency and task dispatch independent of frame size",()=>{
            foreach(var fps in new[]{30,60,120})
            {
                var r=Crisis();for(int i=0;i<13*fps;i++) r.Tick(1d/fps,false);
                Check(r.Crisis.Emergency.Status==EmergencyStatus.Active);Near(r.Crisis.Emergency.RemainingSeconds,44);
                for(int i=0;i<2*fps;i++) r.Tick(1d/fps,false,true,true);
                Check(r.Crisis.Emergency.Status==EmergencyStatus.Repaired);
            }
        });
    }
    private static ReleaseFlow Flow()
    {var f=new ReleaseFlow(()=>new GameRunState(GameMode.FinalPacing));Check(f.Command(ReleaseCommand.NewGame));Check(f.Command(ReleaseCommand.StartGameplay));return f;}
    private static GameRunState Crisis(int seed=0) {var r=new GameRunState(GameMode.FinalPacing,taskSeed:seed);r.Start();Prepare(r);return r;}
    private static void Prepare(GameRunState r)
    {
        Check(r.TryChoose(DecisionCatalog.EnergyStationId,DecisionCatalog.Solar,out _));
        Check(r.TryChoose(DecisionCatalog.WaterStationId,DecisionCatalog.Efficiency,out _));
        Check(r.TryChoose(DecisionCatalog.FarmStationId,DecisionCatalog.Protect,out _));
    }
    private static MaintenanceTask Available(GameRunState r)=>r.Crisis.Tasks.First(t=>t.Status==MaintenanceStatus.Available);
    private static void Complete(GameRunState r,MaintenanceTask t)=>Check(r.TryCompleteTask(t.Id,t.LocationId,true,false,out _));
    private static void Near(double a,double b)=>Check(Math.Abs(a-b)<1e-6,$"{a} != {b}");
    private static void Check(bool ok,string message="Pacing assertion failed") {if(!ok)throw new Exception(message);}
}
