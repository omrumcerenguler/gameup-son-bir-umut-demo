using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SonBirUmutDemo.Decisions
{
    public enum DecisionEffect { Standard, Pump, WaterEfficiency, ProtectNature, Irrigate, StoreWater }

    public sealed class DecisionOption
    {
        public string Id { get; }
        public string Title { get; }
        public string Description { get; }
        public int Cost { get; }
        public int EnergyDelta { get; }
        public int WaterDelta { get; }
        public int EmissionsDelta { get; }
        public int RequiredEnergy { get; }
        public int RequiredWater { get; }
        public DecisionEffect Effect { get; }

        public DecisionOption(string id, string title, string description, int cost,
            int energyDelta, int waterDelta, int emissionsDelta,
            DecisionEffect effect = DecisionEffect.Standard, int requiredEnergy = 0, int requiredWater = 0)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Option id is required.", nameof(id));
            if (cost < 0 || cost > 999) throw new ArgumentOutOfRangeException(nameof(cost));
            if (Math.Abs((long)energyDelta) > 100 || Math.Abs((long)waterDelta) > 100 ||
                Math.Abs((long)emissionsDelta) > 100) throw new ArgumentOutOfRangeException("delta");
            if (requiredEnergy < 0 || requiredEnergy > 100 || requiredWater < 0 || requiredWater > 100)
                throw new ArgumentOutOfRangeException("requirement");
            if (!Enum.IsDefined(typeof(DecisionEffect), effect)) throw new ArgumentOutOfRangeException(nameof(effect));
            Id = id;
            Title = title;
            Description = description;
            Cost = cost;
            EnergyDelta = energyDelta;
            WaterDelta = waterDelta;
            EmissionsDelta = emissionsDelta;
            Effect = effect;
            RequiredEnergy = requiredEnergy;
            RequiredWater = requiredWater;
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
        public const string WaterStationId = "water-station";
        public const string Pump = "pump";
        public const string Efficiency = "efficiency";
        public const string Continue = "continue";
        public const string FarmStationId = "farm-station";
        public const string Protect = "protect";
        public const string Irrigate = "irrigate";
        public const string Store = "store";

        public static readonly DecisionPointDefinition EnergyStation = new DecisionPointDefinition(
            EnergyStationId,
            new DecisionOption(Solar, "A. Güneş enerjisi kur",
                "Yüksek ilk yatırım; su pompası için daha fazla kapasite sağlar.", 60, 35, 0, 1),
            new DecisionOption(Diesel, "B. Dizel jeneratör kullan",
                "Daha düşük ilk maliyet, yüksek kapasite; daha fazla emisyon ve su gideri.", 25, 50, -3, 18),
            new DecisionOption(Conservation, "C. Enerji tasarrufu uygula",
                "Yatırım gerektirmez; suyu korur fakat kullanılabilir enerji kapasitesini azaltır.", 0, -5, 8, 0));

        public static readonly DecisionPointDefinition WaterStation = new DecisionPointDefinition(
            WaterStationId,
            new DecisionOption(Pump, "A. Arıtma ve pompalama",
                "Enerjiye bağlı olarak en çok 50 su kazandırır. Enerji 25'in altındaysa işletme maliyeti 5 artar.",
                25, 0, 0, 0, DecisionEffect.Pump, requiredEnergy: 10),
            new DecisionOption(Efficiency, "B. Su verimliliği",
                "Kaçakları onarır: 18 su geri kazanılır. Bu ve sonraki turda su kullanımı 8 yerine 4 olur.",
                10, 0, 18, 0, DecisionEffect.WaterEfficiency),
            new DecisionOption(Continue, "C. Mevcut düzeni sürdür",
                "Bütçe harcamaz. Kaçaklardan 5 su kaybedilir; olağan tur tüketimi sürer.", 0, 0, -5, 0));

        public static readonly DecisionPointDefinition FarmStation = new DecisionPointDefinition(
            FarmStationId,
            new DecisionOption(Protect, "A. Doğal alanları koru",
                "Kalan suya bağlı olarak bitkilere en çok 22 destek; emisyon etki göstergesi 2 azalır.",
                10, 0, 0, -2, DecisionEffect.ProtectNature),
            new DecisionOption(Irrigate, "B. Tarımsal sulamaya öncelik ver",
                "En az 20 su ister ve 20 su harcar. Kalan suya bağlı olarak en çok 36 bitki desteği sağlar.",
                15, 0, -20, 0, DecisionEffect.Irrigate, requiredWater: 20),
            new DecisionOption(Store, "C. Kurak dönem için su depola",
                "Yatırım gerektirmez; bu tur yalnızca 2 su kullanılır. Sulama ertelendiği için bitki desteği -5.",
                0, 0, 0, 0, DecisionEffect.StoreWater));

        public static readonly ReadOnlyCollection<DecisionPointDefinition> All =
            Array.AsReadOnly(new[] { EnergyStation, WaterStation, FarmStation });
    }
}
