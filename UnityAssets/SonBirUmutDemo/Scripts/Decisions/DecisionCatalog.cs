using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SonBirUmutDemo.Decisions
{
    public sealed class DecisionOption
    {
        public string Id { get; }
        public string Title { get; }
        public string Description { get; }
        public int Cost { get; }
        public int EnergyDelta { get; }
        public int WaterDelta { get; }
        public int EmissionsDelta { get; }

        public DecisionOption(string id, string title, string description, int cost,
            int energyDelta, int waterDelta, int emissionsDelta)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Option id is required.", nameof(id));
            if (cost < 0 || cost > 999) throw new ArgumentOutOfRangeException(nameof(cost));
            if (Math.Abs((long)energyDelta) > 100 || Math.Abs((long)waterDelta) > 100 ||
                Math.Abs((long)emissionsDelta) > 100) throw new ArgumentOutOfRangeException("delta");
            Id = id;
            Title = title;
            Description = description;
            Cost = cost;
            EnergyDelta = energyDelta;
            WaterDelta = waterDelta;
            EmissionsDelta = emissionsDelta;
        }
    }

    public sealed class DecisionPointDefinition
    {
        public string Id { get; }
        public ReadOnlyCollection<DecisionOption> Options { get; }

        public DecisionPointDefinition(string id, params DecisionOption[] options)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Point id is required.", nameof(id));
            if (options == null || options.Length == 0) throw new ArgumentException("Options are required.", nameof(options));
            var ids = new HashSet<string>(StringComparer.Ordinal);
            foreach (var option in options)
                if (option == null || !ids.Add(option.Id)) throw new ArgumentException("Options must have unique ids.", nameof(options));
            Id = id;
            Options = Array.AsReadOnly((DecisionOption[])options.Clone());
        }
    }

    public static class DecisionCatalog
    {
        public const string EnergyStationId = "energy-station";
        public const string Solar = "solar";
        public const string Diesel = "diesel";
        public const string Conservation = "conservation";

        public static readonly DecisionPointDefinition EnergyStation = new DecisionPointDefinition(
            EnergyStationId,
            new DecisionOption(Solar, "A. Güneş enerjisi kur",
                "Yüksek ilk yatırım; gelecekteki su pompaları için daha fazla kapasite.", 60, 35, 0, 1),
            new DecisionOption(Diesel, "B. Dizel jeneratör kullan",
                "Daha düşük ilk maliyet, yüksek kapasite; daha fazla emisyon ve su gideri.", 25, 50, -3, 18),
            new DecisionOption(Conservation, "C. Enerji tasarrufu uygula",
                "Düşük maliyet; suyu korur fakat kullanılabilir enerji kapasitesini azaltır.", 10, -5, 8, 0));
    }
}
