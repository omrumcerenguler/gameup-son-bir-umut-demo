using System.Collections.ObjectModel;
using SonBirUmutDemo.Decisions;

namespace SonBirUmutDemo.Simulation
{
    public enum MaintenanceStatus { Locked, Available, Completed }

    public sealed class MaintenanceEffects
    {
        public double PressureRelief { get; }
        public int WaterProtection { get; }
        public int PlantSupport { get; }
        internal MaintenanceEffects(double pressure, int water = 0, int plants = 0)
        { PressureRelief = pressure; WaterProtection = water; PlantSupport = plants; }
    }

    public sealed class MaintenanceTask
    {
        public string Id { get; }
        public string Title { get; }
        public string Description { get; }
        public string StationId { get; }
        public string LocationId { get; }
        public TaskRegion? Region { get; }
        public TaskPosition? Position { get; }
        public double AvailableSince { get; private set; }
        public MaintenanceStatus Status { get; private set; } = MaintenanceStatus.Locked;
        public MaintenanceEffects Effects { get; }
        internal MaintenanceTask(string id, string title, string description, string station, MaintenanceEffects effects)
        { Id = id; Title = title; Description = description; StationId = station; Effects = effects; }
        internal MaintenanceTask(PacingTaskDefinition definition) : this(definition.TaskId,definition.Title,definition.Description,null,definition.Effects)
        { LocationId=definition.LocationId; Region=definition.Region; Position=definition.Position; }
        internal void Unlock(double elapsed = 0)
        { if (Status == MaintenanceStatus.Locked) { Status = MaintenanceStatus.Available; AvailableSince=elapsed; } }
        internal bool Complete()
        {
            if (Status != MaintenanceStatus.Available) return false;
            Status = MaintenanceStatus.Completed;
            return true;
        }
    }

    public sealed class CrisisProgress
    {
        public ReadOnlyCollection<MaintenanceTask> Tasks { get; }
        public EmergencyIncident Emergency { get; }
        public double StableSeconds { get; internal set; }
        private readonly bool varied;
        public bool UsesTaskNodes { get; }
        public double ElapsedSeconds { get; private set; }
        public double NextDispatchIn
        {
            get
            {
                if (!UsesTaskNodes || ActiveCount>=2) return double.PositiveInfinity;
                for(int i=0;i<Tasks.Count;i++) if(Tasks[i].Status==MaintenanceStatus.Locked)
                    return System.Math.Max(0,FinalPacingCatalog.DispatchAt(i)-ElapsedSeconds);
                return double.PositiveInfinity;
            }
        }
        public int RequiredCount => UsesTaskNodes ? FinalPacingCatalog.RequiredCount : varied ? 4 : 2;
        public int ActiveCount
        { get { int n = 0; foreach (var task in Tasks) if (task.Status == MaintenanceStatus.Available) n++; return n; } }
        public int CompletedCount
        {
            get { int count = 0; foreach (var task in Tasks) if (task.Status == MaintenanceStatus.Completed) count++; return count; }
        }
        public CrisisProgress(CrisisRules rules, bool varied = false, bool taskNodes = false, int seed = 0)
        {
            if (rules == null) throw new System.ArgumentNullException(nameof(rules));
            this.varied = varied; UsesTaskNodes=taskNodes;
            if(taskNodes)
            {
                var chosen = new System.Collections.Generic.List<MaintenanceTask>();
                foreach(var definition in FinalPacingCatalog.Select(seed)) chosen.Add(new MaintenanceTask(definition));
                Tasks=chosen.AsReadOnly(); Emergency=new EmergencyIncident(rules); return;
            }
            var tasks = new System.Collections.Generic.List<MaintenanceTask> {
                new MaintenanceTask("energy-check", "Enerji bağlantısını denetle", "Bağlantıları sıkılaştır; sistemi güvenli ve verimli tut.",
                    DecisionCatalog.EnergyStationId, new MaintenanceEffects(12)),
                new MaintenanceTask("water-seal", "Su deposundaki kaybı gider", "Sızan bağlantıyı kapat; kaybolan rezervin küçük bir bölümünü koru.",
                    DecisionCatalog.WaterStationId, new MaintenanceEffects(14, water: 4)),
                new MaintenanceTask("farm-valve", "Sulama vanasını ayarla", "Gereksiz tüketimi sınırla; bitkilerin dayanıklılığını destekle.",
                    DecisionCatalog.FarmStationId, new MaintenanceEffects(14, plants: 4))
            };
            if (varied)
            {
                tasks.Add(new MaintenanceTask("energy-audit", "Tasarruf bağlantısını kontrol et", "Yük paylaşımını denetle; gereksiz kullanımı sınırla.",
                    DecisionCatalog.EnergyStationId, new MaintenanceEffects(4)));
                tasks.Add(new MaintenanceTask("water-filter", "Su filtresini temizle", "Giriş filtresindeki tıkanıklığı aç; mevcut rezervi güvenceye al.",
                    DecisionCatalog.WaterStationId, new MaintenanceEffects(4)));
            }
            Tasks = tasks.AsReadOnly();
            Emergency = new EmergencyIncident(rules);
        }
        public MaintenanceTask AtStation(string stationId)
        {
            if(UsesTaskNodes) return null;
            MaintenanceTask first = null;
            foreach (var task in Tasks)
                if (task.StationId == stationId)
                {
                    if (task.Status == MaintenanceStatus.Available) return task;
                    if (first == null) first = task;
                }
            return first;
        }
        public MaintenanceTask AtLocation(string locationId)
        { foreach(var task in Tasks) if(task.LocationId!=null && task.LocationId==locationId) return task; return null; }
        internal void AdvanceSchedule(double seconds)
        { if(UsesTaskNodes) { ElapsedSeconds+=seconds; Promote(); } }
        internal void Begin(double remainingTime)
        {
            if (varied || UsesTaskNodes) Promote();
            else foreach (var task in Tasks) task.Unlock();
            Emergency.Schedule(remainingTime);
        }
        internal void Promote()
        {
            if(UsesTaskNodes)
            {
                for(int i=0;i<Tasks.Count && ActiveCount<2;i++)
                    if(Tasks[i].Status==MaintenanceStatus.Locked && ElapsedSeconds>=FinalPacingCatalog.DispatchAt(i)-1e-9)
                        Tasks[i].Unlock(ElapsedSeconds);
                return;
            }
            if (!varied) return;
            foreach (var task in Tasks)
            {
                if (ActiveCount >= 2) return;
                if (task.Status != MaintenanceStatus.Locked) continue;
                bool stationBusy = false;
                foreach (var other in Tasks)
                    if (other.Status == MaintenanceStatus.Available && other.StationId == task.StationId) stationBusy = true;
                if (!stationBusy) task.Unlock();
            }
        }
    }
}
