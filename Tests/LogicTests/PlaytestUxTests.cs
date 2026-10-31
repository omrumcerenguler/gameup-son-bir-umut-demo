using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using SonBirUmutDemo.Decisions;
using SonBirUmutDemo.Simulation;

internal static class PlaytestUxTests
{
    public static void Register(Action<string,Action> run)
    {
        run("UX: five finite tasks retain the original three", () => {
            var state=Prepared(); Check(state.Crisis.Tasks.Count==5 && state.Crisis.RequiredCount==4);
            var ids=new HashSet<string>();
            foreach(var task in state.Crisis.Tasks) Check(ids.Add(task.Id));
            Check(state.Crisis.Tasks[0].Id=="energy-check" && state.Crisis.Tasks[1].Id=="water-seal" && state.Crisis.Tasks[2].Id=="farm-valve");
            Check(state.Crisis.ActiveCount==2); AssertActiveStationsUnique(state);
        });
        run("UX: promotion never exceeds two active tasks", () => {
            var state=Prepared();
            while(state.Crisis.CompletedCount<5)
            {
                var task=FirstAvailable(state); Check(task!=null); Complete(state,task.StationId);
                Check(state.Crisis.ActiveCount<=2); AssertActiveStationsUnique(state);
            }
            Check(state.Crisis.ActiveCount==0);
        });
        run("UX: locked variations do not pay out", () => {
            var state=Prepared(); var before=state.CurrentResources;
            Check(!state.TryCompleteMaintenance(DecisionCatalog.FarmStationId,out _));
            Check(ReferenceEquals(before,state.CurrentResources) && state.Crisis.CompletedCount==0);
        });
        run("UX: station lookup selects available variation after completed task", () => {
            var state=Prepared(); Complete(state,DecisionCatalog.EnergyStationId); Complete(state,DecisionCatalog.WaterStationId);
            Check(state.Crisis.AtStation(DecisionCatalog.EnergyStationId).Id=="energy-audit");
            Complete(state,DecisionCatalog.EnergyStationId);
            Check(!state.TryCompleteMaintenance(DecisionCatalog.EnergyStationId,out _));
        });
        run("UX: alternate available order remains playable", () => {
            var state=Prepared(); Complete(state,DecisionCatalog.WaterStationId); Complete(state,DecisionCatalog.FarmStationId);
            Complete(state,DecisionCatalog.WaterStationId); Complete(state,DecisionCatalog.EnergyStationId); Complete(state,DecisionCatalog.EnergyStationId);
            Check(state.Crisis.CompletedCount==5 && state.Crisis.ActiveCount==0);
        });
        run("UX: no repeated or unlimited variation reward", () => {
            var state=Prepared(); while(FirstAvailable(state)!=null)Complete(state,FirstAvailable(state).StationId);
            var before=state.CurrentResources; double pressure=state.CrisisPressure;
            foreach(string id in new[]{DecisionCatalog.EnergyStationId,DecisionCatalog.WaterStationId,DecisionCatalog.FarmStationId})
                for(int i=0;i<20;i++)Check(!state.TryCompleteMaintenance(id,out _));
            Check(ReferenceEquals(before,state.CurrentResources)); Near(state.CrisisPressure,pressure);
        });
        run("UX: four interventions required rather than a premature two-task ending", () => {
            var state=Prepared(); Complete(state,DecisionCatalog.EnergyStationId); Complete(state,DecisionCatalog.WaterStationId);
            state.Tick(12,false); state.Tick(2,false,true,true); state.Tick(15,false);
            Check(!state.IsEnded && state.Crisis.StableSeconds==0);
            Complete(state,DecisionCatalog.FarmStationId); state.Tick(15,false); Check(!state.IsEnded);
            Complete(state,DecisionCatalog.EnergyStationId); state.Tick(10,false);
            Check(state.IsEnded && state.Result.CrisisStabilized && state.Result.ManagementSummary.Contains("4/5"));
        });
        run("UX: fifth task remains optional and cannot restart an ended run", () => {
            var state=Prepared(); foreach(string id in new[]{DecisionCatalog.EnergyStationId,DecisionCatalog.WaterStationId,DecisionCatalog.FarmStationId,DecisionCatalog.EnergyStationId}) Complete(state,id);
            state.Tick(12,false); state.Tick(2,false,true,true); state.Tick(10,false);
            Check(state.IsEnded && !state.TryCompleteMaintenance(DecisionCatalog.WaterStationId,out _));
        });
        run("UX: existing CrisisDemo keeps three independent tasks and two-task win", () => {
            var state=new GameRunState(GameMode.CrisisDemo); state.Start(); Prepare(state);
            Check(state.Crisis.Tasks.Count==3 && state.Crisis.RequiredCount==2 && state.Crisis.ActiveCount==3);
        });
        run("UX: replay restores five locked tasks, initial pressure and event", () => {
            var state=Prepared(); Complete(state,DecisionCatalog.EnergyStationId); state.Tick(12,false); state.Tick(1,false,true,true);
            state.Reset(); Check(state.Phase==GameRunPhase.Intro && state.Crisis.CompletedCount==0 && state.Crisis.ActiveCount==0);
            Check(state.Crisis.Tasks.Count==5 && state.Crisis.Emergency.Status==EmergencyStatus.Waiting);
            Near(state.RemainingSeconds,240); Near(state.CrisisPressure,60); Near(state.Crisis.Emergency.RepairSeconds,0);
            state.Start(); Prepare(state); Check(state.Crisis.ActiveCount==2);
        });
        run("UX: sprint 1.9 target, release and modal lock", () => {
            Near(MovementPolicy.TargetSpeed(3.5f,true,false,1.9f),6.65,.00001);
            Near(MovementPolicy.TargetSpeed(3.5f,false,false,1.9f),3.5);
            Near(MovementPolicy.TargetSpeed(3.5f,true,true,1.9f),0);
        });
        foreach(int fps in new[]{30,60,120})
        {
            int frequency=fps;
            run("UX: 45-second event and 2-second repair at "+fps+" FPS",()=>{
                var state=Prepared(); for(int i=0;i<12*frequency;i++)state.Tick(1d/frequency,false);
                Check(state.Crisis.Emergency.Status==EmergencyStatus.Active); Near(state.Crisis.Emergency.RemainingSeconds,45,.00001);
                for(int i=0;i<2*frequency-1;i++)state.Tick(1d/frequency,false,true,true);
                Check(state.Crisis.Emergency.Status==EmergencyStatus.Active);
                state.Tick(1d/frequency,false,true,true); Check(state.Crisis.Emergency.Status==EmergencyStatus.Repaired);
                Near(state.RemainingSeconds,226,.00001);
                var missed=Prepared(); missed.Tick(12,false); int water=missed.CurrentResources.Water;
                for(int i=0;i<45*frequency-1;i++)missed.Tick(1d/frequency,false);
                Check(missed.Crisis.Emergency.Status==EmergencyStatus.Active);
                missed.Tick(1d/frequency,false); Check(missed.Crisis.Emergency.Status==EmergencyStatus.Expired);
                Check(missed.CurrentResources.Water==water-6);
                missed.Tick(1,false); Check(missed.CurrentResources.Water==water-6);
            });
        }
        run("UX: cards, explicit simulation pause and focus protect both clocks", () => {
            var state=Prepared(); state.Tick(12,false); state.Tick(1,false,true,true);
            double time=state.RemainingSeconds, incident=state.Crisis.Emergency.RemainingSeconds;
            state.Tick(100,true,true,true); Near(state.RemainingSeconds,time); Near(state.Crisis.Emergency.RemainingSeconds,incident);
            Near(state.Crisis.Emergency.RepairSeconds,0);
            state.SetApplicationPaused(true); state.Tick(100,false); state.SetApplicationPaused(false); state.Tick(100,false);
            state.SetFocus(false); state.Tick(100,false); state.SetFocus(true); state.Tick(100,false);
            Near(state.RemainingSeconds,time); Near(state.Crisis.Emergency.RemainingSeconds,incident);
        });
        run("UX: unrepaired event penalty remains one-shot and bounded", () => {
            var state=Prepared(); state.Tick(12,false); int water=state.CurrentResources.Water;
            state.Tick(45,false); Check(state.Crisis.Emergency.Status==EmergencyStatus.Expired);
            Check(state.CurrentResources.Water==Math.Max(0,water-6)); state.Tick(20,false);
            Check(state.CurrentResources.Water==Math.Max(0,water-6));
        });
        run("UX: visibility profile is monotonic and protects the near player", () => {
            foreach(double distance in new[]{22d,38d,50d})
            {
                double previous=0;
                for(int p=0;p<=100;p++)
                {
                    var sample=new AtmosphereProfile(p,distance);
                    Check(sample.Haze>=previous && sample.Haze>=0 && sample.Haze<=1);
                    Check(sample.FogStart>=distance+7-1e-9 && sample.FogEnd>sample.FogStart);
                    previous=sample.Haze;
                }
                Check(new AtmosphereProfile(100,distance).FogEnd<new AtmosphereProfile(60,distance).FogEnd);
            }
        });
        run("UX: atmosphere input bounds and urgency", () => {
            Near(new AtmosphereProfile(-1,22).Haze,0); Near(new AtmosphereProfile(200,22).Haze,1);
            Near(AtmosphereProfile.Urgency(45),0); Near(AtmosphereProfile.Urgency(12),0);
            Near(AtmosphereProfile.Urgency(6),.5); Near(AtmosphereProfile.Urgency(0),1);
            Reject(()=>new AtmosphereProfile(double.NaN,22)); Reject(()=>new AtmosphereProfile(80,-1));
            Reject(()=>AtmosphereProfile.Urgency(-1));
        });
        run("UX: all 27 strategies win with a five-task route within 240 active seconds", ()=>SimulateAll(null));
        run("UX: missing interventions retain partial and lost endings", ()=>{
            var partial=Simulate(DecisionCatalog.Solar,DecisionCatalog.Efficiency,DecisionCatalog.Protect,false);
            var poor=Simulate(DecisionCatalog.Diesel,DecisionCatalog.Continue,DecisionCatalog.Store,false);
            Check(partial.Result.Category==EndingCategory.PartialTransition && !partial.Result.CrisisStabilized);
            Check(poor.Result.Category==EndingCategory.LostFuture && !poor.Result.CrisisStabilized);
        });
    }

    public static void SimulateAll(string reportPath)
    {
        var text=new StringBuilder("# Aşama 3 — beş bakım varyasyonu, tam oturum simülasyonu\n\nUnity hareketi/fiziği/okunabilirlik veya gerçek deneyim süresi ölçümü değildir. Kart okuma hariç aktif zaman girdileri kullanılır.\n\nKararlar 35/70/100 sn; ilk bakım 105; arıza 112; onarım 133–135; su bakımı 145; tarım 165; enerji denetimi 185; isteğe bağlı filtre 190. En az 4 bakım + 1 arıza değerlendirmesi + düşük baskıda 10 aktif sn gerekir. Başlangıç 60, süre 240; toplam bakım azaltımı 48, tamir 18.\n\n");
        text.AppendLine("| İstenen seçimler | Uygulanan seçimler | Bitiş aktif sn | Baskı | Bakım | Kriz başarısı | Genel final |\n| --- | --- | ---: | ---: | ---: | --- | --- |");
        int total=0,fallback=0; var energy=new HashSet<string>(); var endings=new HashSet<EndingCategory>();
        foreach(var e in DecisionCatalog.EnergyStation.Options)
        foreach(var w in DecisionCatalog.WaterStation.Options)
        foreach(var f in DecisionCatalog.FarmStation.Options)
        {
            var state=Simulate(e.Id,w.Id,f.Id,true); total++;
            Check(state.Result.CrisisStabilized && state.RemainingSeconds>0 && state.Crisis.CompletedCount==5);
            string actual=$"{state.Session.History[0].Option.Id}/{state.Session.History[1].Option.Id}/{state.Session.History[2].Option.Id}";
            if(state.Session.History[2].Option.Id!=f.Id)fallback++;
            energy.Add(e.Id); endings.Add(state.Result.Category);
            text.AppendLine(FormattableString.Invariant($"| {e.Id}/{w.Id}/{f.Id} | {actual} | {240-state.RemainingSeconds:0.00} | {state.CrisisPressure:0.00} | 5/5 | Evet | {state.Result.Title} |"));
        }
        Check(total==27 && fallback==2 && energy.Count==3 && endings.Count==3);
        var partial=Simulate(DecisionCatalog.Solar,DecisionCatalog.Efficiency,DecisionCatalog.Protect,false);
        var poor=Simulate(DecisionCatalog.Diesel,DecisionCatalog.Continue,DecisionCatalog.Store,false);
        Check(partial.Result.Category==EndingCategory.PartialTransition && poor.Result.Category==EndingCategory.LostFuture);
        text.AppendLine("\n27/27 başarı; 2 engelli sulamada ücretsiz depolama alternatifi. Her enerji seçimiyle kriz başarısı, üç genel final kategorisi korunur. Dizelin emisyonu en iyi genel finali engeller. Eksik müdahale senaryolarında bir bakım ve kaçırılmış arıza, 240 aktif sn'de kısmi veya kötü final verir.\n");
        text.AppendLine("Üst sınır: varsayılan artış en çok 0.13/sn. 195 aktif sn için 60 + 195×0.13 − 48 − 18 = 19.35 < 35. Dört bakım asgari koşulu erken iki görev finalini engeller. Zamanlama modele verilmiştir; yol uzunluğu, çarpışma ve oyuncunun okuma süresi Windows'ta ölçülmelidir.\n");
        if(reportPath!=null)File.WriteAllText(reportPath,text.ToString().TrimEnd()+Environment.NewLine);
        Console.WriteLine("PLAYTEST MATRIX: 27/27 success, 5 finite tasks, <=2 active, 4 required, 2 valid fallbacks, all 3 endings.");
    }
    private static GameRunState Simulate(string energy,string water,string farm,bool full)
    {
        var state=Create(); state.Start();
        foreach(var step in new[]{(35d,energy),(70d,water),(100d,farm)})
        {
            To(state,step.Item1); string point=state.Session.CurrentPoint.Id;
            if(!state.TryChoose(point,step.Item2,out _))Check(state.TryChoose(point,DecisionCatalog.Store,out _));
        }
        To(state,105); Complete(state,DecisionCatalog.EnergyStationId);
        if(full)
        {
            To(state,133); state.Tick(2,false,true,true); Check(state.Crisis.Emergency.Status==EmergencyStatus.Repaired);
            To(state,145); Complete(state,DecisionCatalog.WaterStationId);
            To(state,165); Complete(state,DecisionCatalog.FarmStationId);
            To(state,185); Complete(state,DecisionCatalog.EnergyStationId);
            To(state,190); Complete(state,DecisionCatalog.WaterStationId);
        }
        while(!state.IsEnded){state.Tick(.25,false); Check(state.Crisis.ActiveCount<=2);}
        return state;
    }
    private static void To(GameRunState state,double seconds) { Check(!state.IsEnded); state.Tick(seconds-(240-state.RemainingSeconds),false); }
    private static GameRunState Create()=>new GameRunState(GameMode.PlaytestDemo);
    private static GameRunState Prepared(){var state=Create();state.Start();Prepare(state);return state;}
    private static void Prepare(GameRunState state)
    { foreach(string id in new[]{DecisionCatalog.Solar,DecisionCatalog.Efficiency,DecisionCatalog.Protect})Check(state.TryChoose(state.Session.CurrentPoint.Id,id,out _)); }
    private static MaintenanceTask FirstAvailable(GameRunState state)
    { foreach(var task in state.Crisis.Tasks)if(task.Status==MaintenanceStatus.Available)return task;return null; }
    private static void Complete(GameRunState state,string station)=>Check(state.TryCompleteMaintenance(station,out _));
    private static void AssertActiveStationsUnique(GameRunState state)
    {var stations=new HashSet<string>();foreach(var task in state.Crisis.Tasks)if(task.Status==MaintenanceStatus.Available)Check(stations.Add(task.StationId));}
    private static void Check(bool condition){if(!condition)throw new Exception("Assertion failed");}
    private static void Near(double a,double b,double epsilon=.000001)=>Check(Math.Abs(a-b)<epsilon);
    private static void Reject(Action action){try{action();}catch(ArgumentException){return;}throw new Exception("Expected validation failure");}
}
