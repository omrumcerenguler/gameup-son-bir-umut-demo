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
        public MaintenanceStatus Status { get; private set; } = MaintenanceStatus.Locked;
        public MaintenanceEffects Effects { get; }
        internal MaintenanceTask(string id, string title, string description, string station, MaintenanceEffects effects)
        { Id = id; Title = title; Description = description; StationId = station; Effects = effects; }
        internal void Unlock() { if (Status == MaintenanceStatus.Locked) Status = MaintenanceStatus.Available; }
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
        public int RequiredCount => varied ? 4 : 2;
        public int ActiveCount
        { get { int n = 0; foreach (var task in Tasks) if (task.Status == MaintenanceStatus.Available) n++; return n; } }
        public int CompletedCount
        {
            get { int count = 0; foreach (var task in Tasks) if (task.Status == MaintenanceStatus.Completed) count++; return count; }
        }
        public CrisisProgress(CrisisRules rules, bool varied = false)
        {
            if (rules == null) throw new System.ArgumentNullException(nameof(rules));
            this.varied = varied;
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
            MaintenanceTask first = null;
            foreach (var task in Tasks)
                if (task.StationId == stationId)
                {
                    if (task.Status == MaintenanceStatus.Available) return task;
                    if (first == null) first = task;
                }
            return first;
        }
        internal void Begin(double remainingTime)
        {
            if (varied) Promote();
            else foreach (var task in Tasks) task.Unlock();
            Emergency.Schedule(remainingTime);
        }
        internal void Promote()
        {
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
