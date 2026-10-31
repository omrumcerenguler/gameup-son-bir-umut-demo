using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SonBirUmutDemo.Simulation
{
    public enum TaskRegion { Energy, Water, Nature }

    // X/Z in world metres. The same immutable positions feed the builder and route estimates.
    public readonly struct TaskPosition
    {
        public double X { get; }
        public double Z { get; }
        public TaskPosition(double x, double z) { X = x; Z = z; }
        public double DistanceTo(TaskPosition other)
        { double x = X-other.X, z = Z-other.Z; return Math.Sqrt(x*x+z*z); }
    }

    public sealed class PacingTaskDefinition
    {
        public string TaskId { get; }
        public string LocationId { get; }
        public TaskRegion Region { get; }
        public string Title { get; }
        public string Description { get; }
        public TaskPosition Position { get; }
        public MaintenanceEffects Effects { get; }
        internal PacingTaskDefinition(string id, TaskRegion region, string title, string description, double x, double z)
        {
            TaskId = id; LocationId = "node-"+id; Region = region; Title = title; Description = description;
            Position = new TaskPosition(x,z);
            // Six tasks together never exceed the old +4 water/+4 plants allowance.
            Effects = new MaintenanceEffects(6, region == TaskRegion.Water ? 2 : 0, region == TaskRegion.Nature ? 2 : 0);
        }
    }

    public static class FinalPacingCatalog
    {
        public const int SessionCount = 6, RequiredCount = 4;
        public const double InteractionRadius = 1.65;
        public static readonly TaskPosition Spawn = new TaskPosition(1,-4);
        public static readonly TaskPosition EnergyDecision = new TaskPosition(8,-1.2);
        public static readonly TaskPosition WaterDecision = new TaskPosition(-8,6);
        public static readonly TaskPosition FarmDecision = new TaskPosition(8,-9.5);
        public static readonly ReadOnlyCollection<PacingTaskDefinition> All = Array.AsReadOnly(new[] {
            new PacingTaskDefinition("solar-link",TaskRegion.Energy,"Panel bağlantısını denetle","Yedek panel bağlantısını güvene al.",12,2),
            new PacingTaskDefinition("distribution-box",TaskRegion.Energy,"Dağıtım kutusunu kontrol et","Gevşek dağıtım bağlantısını sıkılaştır.",3,3),
            new PacingTaskDefinition("saving-check",TaskRegion.Energy,"Tasarruf hattını denetle","Gereksiz elektrik yükünü kapat.",4,-4),
            new PacingTaskDefinition("tank-check",TaskRegion.Water,"Ana depoyu kontrol et","Depo çıkış bağlantısını sızdırmaz tut.",-9.5,4.8),
            new PacingTaskDefinition("filter-clean",TaskRegion.Water,"Su filtresini temizle","Kanal girişindeki filtreyi aç.",-9.3,.5),
            new PacingTaskDefinition("pipe-joint",TaskRegion.Water,"Dağıtım borusunu bağla","Köy hattındaki bağlantıyı sıkılaştır.",-3.5,1),
            new PacingTaskDefinition("reserve-line",TaskRegion.Water,"Yedek hattı denetle","Kanalın güneyindeki yedek hattı hazırla.",-9.2,-6.5),
            new PacingTaskDefinition("irrigation-valve",TaskRegion.Nature,"Sulama vanasını ayarla","Tarlaya giden suyu ölçülü dağıt.",4,-9.5),
            new PacingTaskDefinition("soil-check",TaskRegion.Nature,"Tarla nemini kontrol et","Toprağın nemini denetle, kaybı azalt.",11.5,-5),
            new PacingTaskDefinition("tree-care",TaskRegion.Nature,"Ağaçlık alanı koru","Ağaç diplerindeki koruma örtüsünü düzenle.",1,10)
        });

        public static PacingTaskDefinition At(string locationId)
        { foreach (var item in All) if (item.LocationId == locationId) return item; return null; }

        // 54 balanced sets: 2 of 3 energy, 2 of 4 water, 2 of 3 nature; all sets cycle.
        // Seed is local session variety, not gameplay persistence or global Unity random state.
        public static List<PacingTaskDefinition> Select(int seed)
        {
            int index = (int)((uint)seed % 54);
            var candidates = new List<PacingTaskDefinition>();
            AddPair(candidates,0,3,index%3); index/=3;
            AddPair(candidates,3,4,index%6); index/=6;
            AddPair(candidates,7,3,index%3);
            var ordered = new List<PacingTaskDefinition>();
            TaskPosition previous = FarmDecision; TaskRegion? region = null;
            while (candidates.Count>0)
            {
                PacingTaskDefinition nearest = null, alternate = null;
                foreach (var task in candidates)
                {
                    if (nearest == null || previous.DistanceTo(task.Position)<previous.DistanceTo(nearest.Position)) nearest=task;
                    if (task.Region!=region && (alternate==null || previous.DistanceTo(task.Position)<previous.DistanceTo(alternate.Position))) alternate=task;
                }
                // Prefer a region change when it fits a local journey, avoid forcing opposite corners.
                var next=alternate!=null && previous.DistanceTo(alternate.Position)<=18 ? alternate : nearest;
                ordered.Add(next); candidates.Remove(next); previous=next.Position; region=next.Region;
            }
            return ordered;
        }
        private static void AddPair(List<PacingTaskDefinition> items,int start,int count,int pair)
        {
            int n=0;
            for(int a=0;a<count;a++) for(int b=a+1;b<count;b++)
                if(n++==pair) { items.Add(All[start+a]); items.Add(All[start+b]); return; }
        }
        // Soft dispatch times since crisis begins, never individual expiration deadlines.
        public static double DispatchAt(int index) => index<2 ? 0 : 25+(index-2)*30;
    }
}
