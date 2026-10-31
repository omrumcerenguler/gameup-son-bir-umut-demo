using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SonBirUmutDemo.Decisions;
using SonBirUmutDemo.Simulation;

internal static class CrisisDemoTests
{
    private static readonly string[] Stations = { DecisionCatalog.EnergyStationId, DecisionCatalog.WaterStationId, DecisionCatalog.FarmStationId };
    public static void Register(Action<string, Action> run)
    {
        run("Crisis: three unique locked tasks at the correct stations", () => {
            var state = Create(); Check(state.Crisis.Tasks.Count == 3);
            var ids = new HashSet<string>();
            for (int i = 0; i < 3; i++)
            {
                var task = state.Crisis.Tasks[i];
                Check(ids.Add(task.Id) && task.StationId == Stations[i] && task.Status == MaintenanceStatus.Locked);
                Check(task.Title.Length > 0 && task.Description.Length > 0 && task.Effects.PressureRelief > 0);
            }
        });
        run("Crisis: no task reward during intro or preparation", () => {
            var state = Create(); var original = state.CurrentResources;
            Check(!state.TryCompleteMaintenance(Stations[0], out _)); state.Start();
            Check(!state.TryCompleteMaintenance(Stations[1], out _));
            Check(ReferenceEquals(original, state.CurrentResources)); Near(state.CrisisPressure, 60);
        });
        run("Crisis: tasks unlock after third strategic decision", () => {
            var state = Prepared(); Check(state.Phase == GameRunPhase.CrisisManagement && !state.IsEnded);
            foreach (var task in state.Crisis.Tasks) Check(task.Status == MaintenanceStatus.Available);
        });
        run("Crisis: one-shot pressure and bounded resource rewards", () => {
            var state = Prepared(); var strategic = state.Session.State;
            Complete(state, 0); Near(state.CrisisPressure, 48);
            Complete(state, 1); Check(state.CurrentResources.Water == strategic.Water + 4);
            Complete(state, 2); Check(state.CurrentResources.PlantHealth == strategic.PlantHealth + 4);
            Near(state.CrisisPressure, 20); Check(state.Crisis.CompletedCount == 3);
            var current = state.CurrentResources;
            for (int i = 0; i < 3; i++) Check(!state.TryCompleteMaintenance(Stations[i], out _));
            Near(state.CrisisPressure, 20); Check(ReferenceEquals(current, state.CurrentResources));
            Check(ReferenceEquals(strategic, state.Session.State) && state.Session.History.Count == 3);
            Check(current.Budget == strategic.Budget && current.Energy == strategic.Energy && current.Emissions == strategic.Emissions);
        });
        run("Crisis: all six maintenance orders agree", () => {
            string expected = null;
            foreach (var order in new[] { new[]{0,1,2},new[]{0,2,1},new[]{1,0,2},new[]{1,2,0},new[]{2,0,1},new[]{2,1,0} })
            {
                var state = Prepared(); foreach (int i in order) Complete(state, i);
                string actual = Fingerprint(state); if (expected == null) expected = actual; Check(actual == expected);
            }
        });
        run("Crisis: invalid station cannot change resources", () => {
            var state = Prepared(); var before = state.CurrentResources;
            Check(!state.TryCompleteMaintenance(null, out _) && !state.TryCompleteMaintenance("missing", out _));
            Check(ReferenceEquals(before, state.CurrentResources) && state.Crisis.CompletedCount == 0);
        });
        run("Crisis: event triggers exactly once at deterministic time", () => {
            var state = Prepared(); state.Tick(11.9, false); Check(state.Crisis.Emergency.Status == EmergencyStatus.Waiting);
            state.Tick(.1, false); Check(state.Crisis.Emergency.Status == EmergencyStatus.Active);
            state.Tick(2, false, true, true); Check(state.Crisis.Emergency.Status == EmergencyStatus.Repaired);
            double pressure = state.CrisisPressure; state.Tick(20, false, true, true);
            Check(state.Crisis.Emergency.Status == EmergencyStatus.Repaired && state.CrisisPressure > pressure);
            Near(state.Crisis.Emergency.RepairSeconds, 2);
        });
        run("Crisis: late preparation still schedules event before global timeout", () => {
            var state = Create(20); state.Start(); state.Tick(18, false); Prepare(state);
            state.Tick(.5, false); Check(state.Crisis.Emergency.Status == EmergencyStatus.Active);
            state.Tick(1.5, false); Check(state.IsEnded && state.Result.Summary != null);
        });
        run("Crisis: event delay does not count toward repair hold", () => {
            var state = Prepared(); state.Tick(12, false, true, true);
            Near(state.Crisis.Emergency.RepairSeconds, 0);
            state.Tick(1.99, false, true, true); Check(state.Crisis.Emergency.Status == EmergencyStatus.Active);
            state.Tick(.01, false, true, true); Check(state.Crisis.Emergency.Status == EmergencyStatus.Repaired);
        });
        run("Crisis: release and leaving range cancel repair", () => {
            var state = Active(); state.Tick(1.5, false, true, true);
            state.Tick(0, false, false, true); Near(state.Crisis.Emergency.RepairSeconds, 0);
            state.Tick(1.5, false, true, true); state.Tick(.1, false, true, false);
            Near(state.Crisis.Emergency.RepairSeconds, 0);
            state.Tick(1.9, false, true, true); Check(state.Crisis.Emergency.Status == EmergencyStatus.Active);
            state.Tick(.1, false, true, true); Check(state.Crisis.Emergency.Status == EmergencyStatus.Repaired);
        });
        run("Crisis: modal focus and native pause cancel repair without consuming time", () => {
            var state = Active(); state.Tick(1, false, true, true); double remaining = state.RemainingSeconds;
            state.Tick(100, true, true, true); Near(state.RemainingSeconds, remaining); Near(state.Crisis.Emergency.RepairSeconds, 0);
            state.Tick(1, false, true, true); state.SetFocus(false); Near(state.Crisis.Emergency.RepairSeconds, 0);
            state.Tick(100, false, true, true); state.SetFocus(true); state.Tick(100, false, true, true);
            Near(state.RemainingSeconds, remaining - 1);
            state.SetApplicationPaused(true); state.Tick(100, false); state.SetApplicationPaused(false); state.Tick(100, false);
            Near(state.RemainingSeconds, remaining - 1);
        });
        run("Crisis: one deadline penalty without lock or duplicate loss", () => {
            var state = Active(); int water = state.CurrentResources.Water;
            double start = state.CrisisPressure; double rate = state.PressureRiseRate;
            Check(!state.TryCompleteMaintenance(Stations[1], out _));
            state.Tick(45, false);
            Check(state.Crisis.Emergency.Status == EmergencyStatus.Expired);
            Near(state.CrisisPressure, start + rate * 45 + CrisisRules.MissedPressure);
            Check(state.CurrentResources.Water == Math.Max(0, water - CrisisRules.MissedWater));
            Complete(state, 1); int after = state.CurrentResources.Water;
            state.Tick(10, false); Check(state.CurrentResources.Water == after);
        });
        run("Crisis: repair finishing on deadline is accepted", () => {
            var state = Active(); state.Tick(43, false); state.Tick(2, false, true, true);
            Check(state.Crisis.Emergency.Status == EmergencyStatus.Repaired);
        });
        run("Crisis: late hold cannot borrow time beyond incident deadline", () => {
            var state = Active(); state.Tick(44, false); int water = state.CurrentResources.Water;
            state.Tick(10, false, true, true);
            Check(state.Crisis.Emergency.Status == EmergencyStatus.Expired);
            Check(state.CurrentResources.Water == Math.Max(0, water - CrisisRules.MissedWater));
            Near(state.Crisis.Emergency.RepairSeconds, 0);
        });
        run("Crisis: two tasks meet the minimum without awarding the third", () => {
            var state = Prepared(); Complete(state,0); Complete(state,1);
            state.Tick(12,false); state.Tick(2,false,true,true); state.Tick(10,false);
            Check(state.IsEnded && state.Result.CrisisStabilized && state.Crisis.CompletedCount == 2);
            Check(state.Crisis.Tasks[2].Status == MaintenanceStatus.Available);
        });
        run("Crisis: single interaction owner with modal and emergency priority", () => {
            Check(InteractionPolicy.Resolve(true,true,true,true) == InteractionKind.Modal);
            Check(InteractionPolicy.Resolve(false,true,true,true) == InteractionKind.Emergency);
            Check(InteractionPolicy.Resolve(false,false,true,true) == InteractionKind.StrategicDecision);
            Check(InteractionPolicy.Resolve(false,false,false,true) == InteractionKind.Maintenance);
            Check(InteractionPolicy.Resolve(false,false,false,false) == InteractionKind.None);
        });
        run("Crisis: one task plus successful repair cannot win", () => {
            var state = Prepared(); Complete(state, 1); state.Tick(12, false); state.Tick(2, false, true, true);
            state.Tick(20, false); Check(!state.IsEnded && state.Crisis.StableSeconds == 0);
        });
        run("Crisis: three tasks cannot win before event evaluation", () => {
            var state = Prepared(); foreach (int i in new[]{0,1,2}) Complete(state,i);
            state.Tick(11, false); Check(!state.IsEnded && state.Crisis.StableSeconds == 0);
        });
        run("Crisis: stability needs ten continuous active seconds", () => {
            var state = ReadyForStability(); state.Tick(9.9, false); Check(!state.IsEnded);
            double stable = state.Crisis.StableSeconds; state.Tick(100, true); Near(state.Crisis.StableSeconds, stable);
            state.Tick(.1, false); Check(state.IsEnded && state.Result.CrisisStabilized && state.MovementLocked);
            Check(state.Result.Reason == RunEndReason.CrisisStabilized && state.RemainingSeconds > 0);
        });
        run("Crisis: pressure crossing threshold resets stability", () => {
            var state = Create(settings: new GameRunSettings(baseRise: 1, maximumRisePerSecond: 1),
                rules: new CrisisRules(stableThreshold: 22, stableDuration: 10));
            state.Start(); Prepare(state); Complete(state,0); Complete(state,1); Complete(state,2);
            state.Tick(12, false); state.Tick(2, false, true, true); // 60 - 40 + 14 - 18 = 16
            state.Tick(5, false); Near(state.Crisis.StableSeconds,5);
            state.Tick(2, false); Near(state.Crisis.StableSeconds,0); Check(!state.IsEnded);
        });
        run("Crisis: current resources and fresh final replace stale strategic result", () => {
            var state = ReadyForStability(); var old = state.Session.Summary; int strategicWater = state.Session.State.Water;
            state.Tick(10, false); var current = state.CurrentResources;
            Check(current.Water == strategicWater + 4 && !ReferenceEquals(old, state.Result.Summary));
            Check(state.Result.Resources == current && state.Result.Summary.DrinkingWater.Contains(current.Water.ToString()));
            Check(state.Result.Category == EndingResult.Evaluate(current).Category);
            Check(ReferenceEquals(old,state.Session.Summary));
        });
        for (int count = 0; count <= 3; count++)
        {
            int decisions = count;
            run("Crisis: global timeout safe with " + count + " strategic choices", () => {
                var state = Create(5); state.Start();
                foreach (string id in FirstChoices(decisions)) Select(state,id);
                state.Tick(5,false); Check(state.IsEnded && state.Result != null && state.Result.Summary != null);
                Check(!state.Result.CrisisStabilized && state.Result.Category != EndingCategory.Rebirth);
                Check(state.Result.CompletedDecisions == decisions && state.Result.ManagementSummary.Length > 0);
            });
        }
        run("Crisis: complete maintenance but unresolved event times out safely", () => {
            var state = Create(15); state.Start(); Prepare(state);
            for(int i=0;i<3;i++) Complete(state,i);
            state.Tick(15,false); Check(state.IsEnded && state.Crisis.Emergency.Status == EmergencyStatus.Active);
            Check(!state.Result.CrisisStabilized && state.Result.Summary != null);
        });
        run("Crisis: expired event remains playable and can stabilize", () => {
            var state = Prepared(); for(int i=0;i<3;i++) Complete(state,i);
            state.Tick(57,false); Check(state.Crisis.Emergency.Status == EmergencyStatus.Expired);
            state.Tick(10,false); Check(state.IsEnded && state.Result.CrisisStabilized);
        });
        run("Crisis: pressure and resource ceilings remain bounded", () => {
            var state = Create(initial: new ResourceState(999,100,100,100,100),
                settings: new GameRunSettings(initialPressure: 0, baseRise:0, waterRiskRise:0,plantRiskRise:0,energyRiskRise:0));
            state.Start(); Select(state,DecisionCatalog.Conservation); Select(state,DecisionCatalog.Efficiency); Select(state,DecisionCatalog.Protect);
            for(int i=0;i<3;i++) Complete(state,i);
            Near(state.CrisisPressure,0); Check(state.CurrentResources.Water<=100 && state.CurrentResources.PlantHealth<=100);
            state.Tick(12,false); state.Tick(45,false); Check(state.CurrentResources.Water>=0 && state.CrisisPressure<=100);
        });
        run("Crisis: two consecutive replay cycles reset every mutable value", () => {
            var state = Create();
            for(int cycle=0;cycle<2;cycle++)
            {
                state.Start(); Prepare(state); Complete(state,0); Complete(state,1); Complete(state,2);
                state.Tick(12,false); state.Tick(2,false,true,true); state.Tick(10,false); Check(state.IsEnded);
                var old = state.Crisis; state.Reset(); Check(!ReferenceEquals(old,state.Crisis));
                Check(state.Phase==GameRunPhase.Intro && state.Result==null && state.Crisis.CompletedCount==0);
                Check(state.Crisis.Emergency.Status==EmergencyStatus.Waiting && state.Session.History.Count==0);
                Near(state.CrisisPressure,60); Near(state.RemainingSeconds,240); Near(state.Crisis.StableSeconds,0);
                Check(state.CurrentResources.Water==40 && state.CurrentResources.PlantHealth==55);
            }
        });
        run("Crisis: end state is immutable to further task and event calls", () => {
            var state=ReadyForStability(); state.Tick(10,false); var result=state.Result; var resources=state.CurrentResources;
            double pressure=state.CrisisPressure; state.Tick(500,false,true,true);
            Check(!state.TryCompleteMaintenance(Stations[0],out _) && !state.TryReducePressure(100));
            Check(ReferenceEquals(result,state.Result) && ReferenceEquals(resources,state.CurrentResources)); Near(state.CrisisPressure,pressure);
        });
        run("Crisis: interval partition gives the same repair and final", () => {
            var a=Prepared(); var b=Prepared();
            for(int i=0;i<3;i++) { Complete(a,i); Complete(b,i); }
            a.Tick(12,false); for(int i=0;i<120;i++) b.Tick(.1,false);
            a.Tick(12,false,true,true); for(int i=0;i<120 && !b.IsEnded;i++) b.Tick(.1,false,true,true);
            Check(a.IsEnded && b.IsEnded && a.Result.Category==b.Result.Category);
            Near(a.RemainingSeconds,b.RemainingSeconds,.00001); Near(a.CrisisPressure,b.CrisisPressure,.00001);
        });
        run("Crisis: configured threshold at pressure ceiling permits stability", () => {
            var state=Create(settings:new GameRunSettings(initialPressure:100,baseRise:1,maximumRisePerSecond:1),
                rules:new CrisisRules(stableThreshold:100,stableDuration:2));
            state.Start(); Prepare(state); for(int i=0;i<3;i++)Complete(state,i);
            state.Tick(57,false); Near(state.CrisisPressure,100);
            state.Tick(2,false); Check(state.IsEnded && state.Result.CrisisStabilized);
        });
        run("Crisis: invalid tuning rejected", () => {
            Reject(()=>new CrisisRules(stableDuration:0)); Reject(()=>new CrisisRules(stableThreshold:101));
            Reject(()=>new CrisisRules(emergencyDelay:double.NaN)); Reject(()=>new CrisisRules(emergencyWindow:1,repairDuration:2));
        });
        run("Crisis: all 27 strategies complete full-session simulations", () => SimulateAll(null));
        run("Crisis: diesel stabilizes without best sustainability category", () => {
            var state = Simulate(DecisionCatalog.Diesel,DecisionCatalog.Pump,DecisionCatalog.Irrigate,true);
            Check(state.Result.CrisisStabilized && state.Result.Category==EndingCategory.PartialTransition);
        });
        run("Crisis: poor strategy and missing interventions have valid bad ending", () => {
            var state = Simulate(DecisionCatalog.Diesel,DecisionCatalog.Continue,DecisionCatalog.Store,false);
            Check(state.Result.Reason==RunEndReason.TimeExpired && !state.Result.CrisisStabilized);
            Check(state.Result.Category==EndingCategory.LostFuture && state.Crisis.CompletedCount==1);
        });
    }

    public static void SimulateAll(string reportPath)
    {
        var report=new StringBuilder("# Kriz Nöbeti — tam oturum denge simülasyonu\n\nSaf C# simülasyonudur; Unity fizik/erişim/girdi/gerçek deneyim süresi testi değildir.\n\n");
        report.AppendLine("Kararlar aktif saniye 35/70/105; ilk bakım 110; arıza 117; su noktasında basılı onarım 144–146; su bakımı 168; tarım bakımı 175. Dolaşma süreleri model girdisidir, Unity'de ölçülmedi. Kart okuma sayaçtan hariçtir. Engelli stratejiye ücretsiz geçerli alternatif uygulanır.\n");
        report.AppendLine("| İstenen enerji / su / tarım | Uygulanan seçimler | Bitiş aktif sn | Baskı | Bakım | Arıza | Kriz başarısı | Genel final | Bütçe / enerji / su / bitki / emisyon |\n| --- | --- | ---: | ---: | ---: | --- | --- | --- | --- |");
        int count=0, fallback=0; var energyWins=new HashSet<string>(); var categories=new HashSet<EndingCategory>();
        foreach(var e in DecisionCatalog.EnergyStation.Options)
        foreach(var w in DecisionCatalog.WaterStation.Options)
        foreach(var f in DecisionCatalog.FarmStation.Options)
        {
            var state=Simulate(e.Id,w.Id,f.Id,true); count++;
            Check(state.Result.CrisisStabilized && state.RemainingSeconds>0);
            energyWins.Add(e.Id); categories.Add(state.Result.Category);
            string applied=string.Join(" / ", new[]{state.Session.History[0].Option.Id,state.Session.History[1].Option.Id,state.Session.History[2].Option.Id});
            if(state.Session.History[2].Option.Id!=f.Id) fallback++;
            var r=state.CurrentResources;
            report.AppendLine($"| {e.Id} / {w.Id} / {f.Id} | {applied} | {240-state.RemainingSeconds:0.00} | {state.CrisisPressure:0.00} | {state.Crisis.CompletedCount}/3 | {state.Crisis.Emergency.Status} | Evet | {state.Result.Title} | {r.Budget} / {r.Energy} / {r.Water} / {r.PlantHealth} / {r.Emissions} |");
        }
        Check(count==27 && fallback==2 && energyWins.Count==3 && categories.Count==3);
        var good=Simulate(DecisionCatalog.Solar,DecisionCatalog.Efficiency,DecisionCatalog.Protect,true);
        Check(good.Result.Category==EndingCategory.Rebirth);
        var poor=Simulate(DecisionCatalog.Diesel,DecisionCatalog.Continue,DecisionCatalog.Store,false);
        Check(!poor.Result.CrisisStabilized && poor.Result.Category==EndingCategory.LostFuture);
        report.AppendLine($"\n27/27 kriz başarısı; {fallback} sulama dizisinde geçerli alternatif; her enerji seçimiyle başarı; genel sonuçlarda üç kategori de mevcut. İyi güneş/verimlilik/koruma yolu Yeniden Doğuş, dizel/pompa/sulama yolu kriz başarısına rağmen Eksik Dönüşüm. Kötü dizel/mevcut/depolama + yalnız enerji bakımı + kaçırılan arıza: 240 sn'de Kaybedilen Gelecek.\n");
        var badResources=poor.CurrentResources;
        report.AppendLine($"Eksik müdahale örneği: aktif süre {240-poor.RemainingSeconds:0.00} sn, baskı {poor.CrisisPressure:0.00}, bakım {poor.Crisis.CompletedCount}/3, arıza {poor.Crisis.Emergency.Status}, kaynaklar {badResources.Budget}/{badResources.Energy}/{badResources.Water}/{badResources.PlantHealth}/{badResources.Emissions}.\n");
        report.AppendLine("## Denge hesabı\n\nBaşlangıç 60. Üç bakım toplam -40, tamir -18. Varsayılan artış/sn tüm kaynak aralığında en çok 0.13 (yapılandırılmış genel tavan 0.25). 185 sn için kaba üst sınır 60 + 185×0.13 − 58 = 26.05 < 35. Kaynaklar/baskı sınırları gerçek simülasyonda her adımda uygulanır. Tek bakım + tamir en az iki görev koşulunu karşılamaz. Arıza değerlendirilmeden istikrar kazanılmaz. Eşiğin altında 10 kesintisiz aktif saniye gerekir.\n");
        if(reportPath!=null) File.WriteAllText(reportPath,report.ToString().TrimEnd()+Environment.NewLine);
        Console.WriteLine("CRISIS MATRIX: 27/27 stabilized, 2 valid fallbacks, 3 energy paths, 3 sustainability categories; poor incomplete run times out safely.");
    }

    private static GameRunState Simulate(string energy,string water,string farm,bool successful)
    {
        var state=Create(); state.Start();
        foreach(var step in new[]{(35d,energy),(70d,water),(105d,farm)})
        { AdvanceTo(state,step.Item1); SelectWithFallback(state,step.Item2); }
        AdvanceTo(state,110); Complete(state,0);
        if(successful)
        {
            AdvanceTo(state,144); Check(state.Crisis.Emergency.Status==EmergencyStatus.Active);
            state.Tick(2,false,true,true); Check(state.Crisis.Emergency.Status==EmergencyStatus.Repaired);
            AdvanceTo(state,168); Complete(state,1); AdvanceTo(state,175); Complete(state,2);
        }
        while(!state.IsEnded) state.Tick(.25,false);
        return state;
    }
    private static void AdvanceTo(GameRunState state,double elapsed)
    {
        double delta=elapsed-(240-state.RemainingSeconds); Check(delta>=-1e-7 && !state.IsEnded);
        state.Tick(Math.Max(0,delta),false);
    }
    private static void SelectWithFallback(GameRunState state,string id)
    {
        string point=state.Session.CurrentPoint.Id;
        if(state.TryChoose(point,id,out _))return;
        foreach(var option in state.Session.CurrentPoint.Options)
            if(option.Cost==0 && state.TryChoose(point,option.Id,out _))return;
        foreach(var option in state.Session.CurrentPoint.Options)
            if(state.TryChoose(point,option.Id,out _))return;
        throw new Exception("No valid strategy fallback");
    }
    private static IEnumerable<string> FirstChoices(int count)
    { var ids=new[]{DecisionCatalog.Solar,DecisionCatalog.Efficiency,DecisionCatalog.Protect}; for(int i=0;i<count;i++)yield return ids[i]; }
    private static GameRunState Create(double seconds=240,ResourceState initial=null,GameRunSettings settings=null,CrisisRules rules=null) =>
        new GameRunState(GameMode.CrisisDemo,settings??new GameRunSettings(durationSeconds:seconds),initial,crisisRules:rules);
    private static GameRunState Prepared() { var state=Create(); state.Start(); Prepare(state); return state; }
    private static GameRunState Active() { var state=Prepared(); state.Tick(12,false); return state; }
    private static GameRunState ReadyForStability()
    { var state=Prepared(); for(int i=0;i<3;i++)Complete(state,i); state.Tick(12,false); state.Tick(2,false,true,true); return state; }
    private static void Prepare(GameRunState state) { foreach(string id in FirstChoices(3))Select(state,id); }
    private static void Select(GameRunState state,string id) => Check(state.TryChoose(state.Session.CurrentPoint.Id,id,out _));
    private static void Complete(GameRunState state,int index) => Check(state.TryCompleteMaintenance(Stations[index],out _));
    private static string Fingerprint(GameRunState state) => $"{state.CurrentResources.Water}/{state.CurrentResources.PlantHealth}/{state.CrisisPressure}";
    private static void Check(bool condition) { if(!condition)throw new Exception("Assertion failed"); }
    private static void Near(double actual,double expected,double epsilon=.000001) => Check(Math.Abs(actual-expected)<epsilon);
    private static void Reject(Action action) { try{action();}catch(ArgumentException){return;}throw new Exception("Expected validation failure"); }
}
