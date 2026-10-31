using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SonBirUmutDemo.Decisions;
using SonBirUmutDemo.Simulation;

// Conservative planar route estimate, NOT Unity collision/navigation validation.
internal static class PacingRouteSimulation
{
    internal sealed class Profile
    {
        public string Name;
        public double SprintFraction, Detour, Planning, Interaction, Reaction, Distraction;
        public double Speed => 3.5*(1+.9*SprintFraction)/Detour;
    }
    internal static readonly Profile[] Profiles = {
        new Profile{Name="Hızlı",SprintFraction=.85,Detour=1.1,Planning=1.5,Interaction=.8,Reaction=1},
        new Profile{Name="Yeni oyuncu",SprintFraction=.3,Detour=1.3,Planning=9,Interaction=3,Reaction=5},
        new Profile{Name="Geciken",SprintFraction=.1,Detour=1.5,Planning=21,Interaction=5,Reaction=12,Distraction=50}
    };
    internal sealed class Result
    {
        public GameRunState Run;
        public ReleaseFlow Flow;
        public double ActiveSeconds, Distance, EmergencyTravel;
        public int Visits;
        public string Route="";
    }
    private const double Step=.25, Cell=.5;
    private static readonly Dictionary<string,List<TaskPosition>> routes=new Dictionary<string,List<TaskPosition>>();
    // Expanded by capsule radius + clearance. Source footprints from existing builders.
    private static readonly double[,] solids={
        {-9.15,-4.85,.85,5.15}, {-9.15,-4.85,-6.15,-1.85}, {4.85,9.15,4.85,9.15},
        {6,10,-.5,2.5}, {-9.5,-6.5,6.5,9.5}, {-6.2,-3.8,-9.2,-6.8},
        {-1.4,1.4,5.9,7.5}, {4.75,6.25,-9.1,-7.9},
        {-13.7,-12.3,6.3,7.7}, {-10.7,-9.3,-8.7,-7.3},
        {12.3,13.7,-11.7,-10.3}, {12.3,13.7,-3.7,-2.3}, {.3,1.7,10.3,11.7},
        // Visual water/pond is avoided even though its current geometry is non-solid.
        {-5.95,-1.45,5.95,9.25}, {-11.65,-10.35,-6.5,7.1}
    };
    internal static bool Clear(TaskPosition p)
    {
        if(p.X < -15 || p.X >15 || p.Z< -13 || p.Z>13) return false;
        for(int i=0;i<solids.GetLength(0);i++)
            if(p.X>=solids[i,0] && p.X<=solids[i,1] && p.Z>=solids[i,2] && p.Z<=solids[i,3]) return false;
        return true;
    }
    internal static List<TaskPosition> Path(TaskPosition from,TaskPosition to)
    {
        // Round to the same half-metre graph; include short exact endpoint segments.
        var a=((int)Math.Round(from.X/Cell),(int)Math.Round(from.Z/Cell));
        var b=((int)Math.Round(to.X/Cell),(int)Math.Round(to.Z/Cell));
        string key=a+":"+b;
        if(!routes.TryGetValue(key,out var cached))
        {
            var queue=new Queue<(int,int)>(); var previous=new Dictionary<(int,int),(int,int)>();
            queue.Enqueue(a);previous[a]=a;
            while(queue.Count>0 && !previous.ContainsKey(b))
            {
                var p=queue.Dequeue();
                foreach(var d in new[]{(1,0),(-1,0),(0,1),(0,-1)})
                {
                    var next=(p.Item1+d.Item1,p.Item2+d.Item2);
                    if(previous.ContainsKey(next) || !Clear(new TaskPosition(next.Item1*Cell,next.Item2*Cell))) continue;
                    previous[next]=p;queue.Enqueue(next);
                }
            }
            if(!previous.ContainsKey(b)) throw new Exception("No model route: "+key);
            cached=new List<TaskPosition>();var current=b;
            while(current!=a) { cached.Add(new TaskPosition(current.Item1*Cell,current.Item2*Cell)); current=previous[current]; }
            cached.Add(new TaskPosition(a.Item1*Cell,a.Item2*Cell));cached.Reverse();routes[key]=cached;
        }
        var path=new List<TaskPosition>{from};path.AddRange(cached);path.Add(to);return path;
    }
    internal static double Distance(TaskPosition a,TaskPosition b)
    { var path=Path(a,b);double n=0;for(int i=1;i<path.Count;i++) n+=path[i-1].DistanceTo(path[i]);return n; }

    internal static Result Simulate(int seed,Profile profile,string energy,string water,string farm,bool repair=true)
    {
        var flow=new ReleaseFlow(()=>new GameRunState(GameMode.FinalPacing,taskSeed:seed));
        flow.Command(ReleaseCommand.NewGame);flow.Command(ReleaseCommand.StartGameplay);
        var result=new Result{Run=flow.Run,Flow=flow};
        var run=result.Run;TaskPosition position=FinalPacingCatalog.Spawn;
        void Wait(double time,bool held=false)
        { while(time>1e-8 && !run.IsEnded) { double d=Math.Min(Step,time);flow.Tick(d,false,held,held);time-=d; } }
        void WalkWhole(TaskPosition destination)
        { double distance=Distance(position,destination);result.Distance+=distance;Wait(distance/profile.Speed);position=destination; }
        string[] ids={DecisionCatalog.EnergyStationId,DecisionCatalog.WaterStationId,DecisionCatalog.FarmStationId};
        string[] choices={energy,water,farm};
        var stations=new[]{FinalPacingCatalog.EnergyDecision,FinalPacingCatalog.WaterDecision,FinalPacingCatalog.FarmDecision};
        for(int i=0;i<3 && !run.IsEnded;i++)
        {
            WalkWhole(stations[i]);Wait(profile.Planning+profile.Interaction);
            if(run.IsEnded) break;
            if(!run.TryChoose(ids[i],choices[i],out _))
            {
                if(i!=2 || !run.TryChoose(ids[i],DecisionCatalog.Store,out _)) throw new Exception("Invalid strategy");
            }
        }
        if(profile.Distraction>0) Wait(profile.Distraction);
        List<TaskPosition> route=null;int point=0;string target=null;
        double delay=0;bool emergency=false;
        while(!run.IsEnded)
        {
            bool urgent=run.Crisis.Emergency.Status==EmergencyStatus.Active;
            MaintenanceTask task=null;
            if(!urgent)
                foreach(var candidate in run.Crisis.Tasks)
                    if(candidate.Status==MaintenanceStatus.Available && (task==null || position.DistanceTo(candidate.Position.Value)<position.DistanceTo(task.Position.Value))) task=candidate;
            string next=urgent ? "emergency" : task?.Id;
            if(next!=target || urgent!=emergency)
            {
                target=next;emergency=urgent;
                route=next==null ? null : Path(position,urgent ? FinalPacingCatalog.WaterDecision : task.Position.Value);
                point=1;delay=urgent ? profile.Reaction : profile.Planning;
                if(next!=null) result.Route+=(result.Route.Length>0 ? " → " : "")+(urgent ? "ARIZA" : task.LocationId);
            }
            if(delay>0) { double d=Math.Min(Step,delay);Wait(d);delay-=d;continue; }
            if(route==null) { Wait(Step);continue; }
            if(point<route.Count)
            {
                var destination=route[point];double distance=position.DistanceTo(destination);
                if(distance<1e-8) {point++;continue;}
                double moved=Math.Min(distance,profile.Speed*Step), dt=moved/profile.Speed;
                double f=moved/distance;
                position=new TaskPosition(position.X+(destination.X-position.X)*f,position.Z+(destination.Z-position.Z)*f);
                result.Distance+=moved;if(urgent) result.EmergencyTravel+=dt;
                Wait(dt);continue;
            }
            if(urgent)
            {
                if(repair) Wait(run.CrisisRules.RepairDuration,true);
                else Wait(run.Crisis.Emergency.RemainingSeconds+.01);
            }
            else if(task!=null)
            {
                Wait(profile.Interaction);
                if(!run.IsEnded && run.Crisis.Emergency.Status!=EmergencyStatus.Active)
                {
                    bool inRange=position.DistanceTo(task.Position.Value)<=FinalPacingCatalog.InteractionRadius;
                    if(!run.TryCompleteTask(task.Id,task.LocationId,inRange,false,out _)) throw new Exception("Route award failed");
                    result.Visits++;
                }
            }
            target=null;route=null;
        }
        result.ActiveSeconds=240-run.RemainingSeconds;return result;
    }
    internal static void WriteReport(string path)
    {
        var report=new StringBuilder("# Final Pacing — rota ve aktif süre tahmini\n\n");
        report.AppendLine("**Model bazlı tahmindir; gerçek Unity hareket, fizik, görüş veya oyuncu süresi ölçümü değildir.**\n");
        report.AppendLine("3.5 m/sn yürüyüş, 1.9× sprint; 0.5 m dört yönlü rota grafiği. Ev/tesis/kuyu/bank/gövde sınırları kapsül payıyla genişletilir; görsel havuz ve kanal da kaçınılır. Gerçek collider/navmesh kullanılmaz. Noktalar katalogla ortaktır.\n");
        report.AppendLine("| Profil | Koşulan zaman payı | Yol/hız güvenlik katsayısı | Hedef başına yön bulma | Etkileşim | Acile tepki | Ek gecikme |\n|---|---:|---:|---:|---:|---:|---:|");
        foreach(var p in Profiles) report.AppendLine($"| {p.Name} | %{p.SprintFraction*100:0} | {p.Detour:0.00} | {p.Planning:0.0} sn | {p.Interaction:0.0} sn | {p.Reaction:0.0} sn | {p.Distraction:0} sn |");
        report.AppendLine("\nKart okuma/Intro/menü/pause aktif sürede yoktur; toplam gerçek süre daha uzundur. Yön bulma ve gecikme serbest oyun süresine eklenir. Acil çağrı yolu keser ve su deposuna rota yeniden kurulur; yol/tepki boyunca 45 sn işler, onarım 2 sn gerçek model süresi tüketir.\n");
        report.AppendLine("| Profil | Enerji (su=verimlilik, tarım=koruma) | 54 setten kriz başarısı | Aktif süre min–maks | Mesafe min–maks | Acil yolculuk en çok |\n|---|---|---:|---:|---:|---:|");
        foreach(var p in Profiles) foreach(var energy in new[]{DecisionCatalog.Solar,DecisionCatalog.Diesel,DecisionCatalog.Conservation})
        {
            var results=Enumerable.Range(0,54).Select(seed=>Simulate(seed,p,energy,DecisionCatalog.Efficiency,DecisionCatalog.Protect)).ToList();
            report.AppendLine($"| {p.Name} | {energy} | {results.Count(r=>r.Run.Result.CrisisStabilized)}/54 | {results.Min(r=>r.ActiveSeconds):0.0}–{results.Max(r=>r.ActiveSeconds):0.0} sn | {results.Min(r=>r.Distance):0.0}–{results.Max(r=>r.Distance):0.0} m | {results.Max(r=>r.EmergencyTravel):0.0} sn |");
        }
        report.AppendLine("\n## Seed 0 örnekleri\n\n| Profil | Aktif sn | Bakım | Arıza | Baskı | Genel final | Rota |\n|---|---:|---:|---|---:|---|---|");
        foreach(var p in Profiles)
        { var r=Simulate(0,p,DecisionCatalog.Solar,DecisionCatalog.Efficiency,DecisionCatalog.Protect);report.AppendLine($"| {p.Name} | {r.ActiveSeconds:0.0} | {r.Visits}/6 | {r.Run.Crisis.Emergency.Status} | {r.Run.CrisisPressure:0.0} | {r.Run.Result.Title} | {r.Route} |"); }
        report.AppendLine("\n## 27 strateji — seed 0, yeni oyuncu\n\n| İstenen seçimler | Uygulanan tarım | Aktif sn | Bakım | Kriz başarısı | Genel final |\n|---|---|---:|---:|---|---|");
        foreach(var e in DecisionCatalog.EnergyStation.Options)
        foreach(var w in DecisionCatalog.WaterStation.Options)
        foreach(var f in DecisionCatalog.FarmStation.Options)
        {
            var r=Simulate(0,Profiles[1],e.Id,w.Id,f.Id);
            report.AppendLine($"| {e.Id}/{w.Id}/{f.Id} | {r.Run.Session.GetChoice(DecisionCatalog.FarmStationId)} | {r.ActiveSeconds:0.0} | {r.Visits}/6 | {(r.Run.Result.CrisisStabilized ? "Evet" : "Hayır")} | {r.Run.Result.Title} |");
        }
        report.AppendLine("\nYetersiz su yüzünden engellenen iki sulama isteğinde mevcut ücretsiz depolama alternatifi kullanılır. Bakım bütçe/enerji/emisyonu silmez; kaynakları kötü bırakılmış stratejiler kriz yönetimi başarılı olsa da iyi genel final garantilemez.");
        report.AppendLine("\nDenge: altı işten en az dört, arıza değerlendirilmiş, baskı ≤35 ve 10 aktif sn istikrar. Her iş -6 baskı; iki su işi toplam +4 su, iki doğa işi toplam +4 bitki; bütçe/enerji/emisyon değişmez. Çağrılar kriz başından 0/0/25/55/85/115 sn önce açılmaz; boş aktif yer ve tamamlanma sırası da gerekir. İşlerin ayrı son tarihi yoktur.\n");
        report.AppendLine("[Windows fizik/oynanış kabul listesi](FINAL_PACING_UNITY_TEST.md). Haritada gerçek dolaşma, yön bulma, ses ve erişim Hakan tarafından ölçülmelidir. Bu tablo keyfî Inspector değerleri için garanti değildir.");
        File.WriteAllText(path,report.ToString());
    }
}
