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
        public string Advantage { get; }
        public string Tradeoff { get; }
        public string NextEffect { get; }
        public string Feedback { get; }

        public DecisionOption(string id, string title, string description, int cost,
            int energyDelta, int waterDelta, int emissionsDelta,
            DecisionEffect effect = DecisionEffect.Standard, int requiredEnergy = 0, int requiredWater = 0,
            string advantage = "", string tradeoff = "", string nextEffect = "", string feedback = "")
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
            Advantage = advantage;
            Tradeoff = tradeoff;
            NextEffect = nextEffect;
            Feedback = feedback;
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
                "Temiz enerji kapasitesi, yüksek ilk yatırım.", 60, 35, 0, 0,
                advantage: "Enerji kapasitesi +35; emisyon artışı yok.",
                tradeoff: "60 bütçe gider; sonraki yatırımlara az pay kalır.",
                nextEffect: "Pompa için güçlü kapasite sağlar.", feedback: "Güneş panelleri kuruldu."),
            new DecisionOption(Diesel, "B. Dizel jeneratör kullan",
                "Düşük ilk maliyetle yüksek kapasite, yüksek emisyon etkisi.", 25, 50, -3, 18,
                advantage: "25 bütçeyle enerji kapasitesi +50.",
                tradeoff: "3 su harcar; emisyon etkisi +18.",
                nextEffect: "Su yatırımına para kalır; emisyon riski sürer.", feedback: "Dizel jeneratör devreye alındı."),
            new DecisionOption(Conservation, "C. Enerji tasarrufu uygula",
                "Yatırım gerektirmez, enerji kapasitesinden ödün verir.", 0, -5, 8, 0,
                advantage: "Bütçe harcamaz; 8 su korunur.",
                tradeoff: "Enerji kapasitesi 5 azalır.",
                nextEffect: "Düşük enerji pompayı sınırlayıp pahalılaştırabilir.", feedback: "Enerji tasarrufu önlemleri alındı."));

        public static readonly DecisionPointDefinition WaterStation = new DecisionPointDefinition(
            WaterStationId,
            new DecisionOption(Pump, "A. Arıtma ve pompalama",
                "Enerjiye bağlı olarak en çok 50 su kazandırır. Enerji 25'in altındaysa işletme maliyeti 5 artar.",
                25, 0, 0, 0, DecisionEffect.Pump, requiredEnergy: 10,
                advantage: "Enerjiye bağlı yüksek su kazanımı.",
                tradeoff: "25–30 bütçe ister; en az 10 enerji gerekir.",
                nextEffect: "Tarım için su artar, yatırım bütçesi azalır.", feedback: "Arıtma ve pompalama altyapısı kuruldu."),
            new DecisionOption(Efficiency, "B. Su verimliliği",
                "Kaçakları onarır: 18 su geri kazanılır. Bu ve sonraki turda su kullanımı 8 yerine 4 olur.",
                10, 0, 18, 0, DecisionEffect.WaterEfficiency,
                advantage: "18 su geri kazanılır; tüketim yarıya iner.",
                tradeoff: "10 bütçe harcar; pompaya göre kazanım daha azdır.",
                nextEffect: "Tarım turunda da olağan su tüketimi 4 olur.", feedback: "Su kaçakları onarıldı."),
            new DecisionOption(Continue, "C. Mevcut düzeni sürdür",
                "Yatırımı erteler, su kaybını sürdürür.", 0, 0, -5, 0,
                advantage: "Bütçe harcamaz; tarıma yatırım payı kalır.",
                tradeoff: "Kaçaklardan 5 su kaybolur; tüketim sürer.",
                nextEffect: "Azalan su, sulama seçeneğini kapatabilir.", feedback: "Su altyapısında mevcut düzen korundu."));

        public static readonly DecisionPointDefinition FarmStation = new DecisionPointDefinition(
            FarmStationId,
            new DecisionOption(Protect, "A. Doğal alanları koru",
                "Kalan suya bağlı olarak bitkilere en çok 22 destek; emisyon etki göstergesi 2 azalır.",
                10, 0, 0, -2, DecisionEffect.ProtectNature,
                advantage: "Kalan suya bağlı bitki desteği; emisyon etkisi -2.",
                tradeoff: "10 bütçe harcar; yeni su üretmez.",
                nextEffect: "Doğal yaşamı ve kuraklık dayanıklılığını destekler.", feedback: "Doğal alanlar koruma altına alındı."),
            new DecisionOption(Irrigate, "B. Tarımsal sulamaya öncelik ver",
                "En az 20 su ister ve 20 su harcar. Kalan suya bağlı olarak en çok 36 bitki desteği sağlar.",
                5, 0, -20, 0, DecisionEffect.Irrigate, requiredWater: 20,
                advantage: "Mevcut kanallarla bitkilere hızlı destek.",
                tradeoff: "5 bütçe ve 20 su harcar; en az 20 su gerekir.",
                nextEffect: "Tarım güçlenir; kuraklık için su rezervi azalır.", feedback: "Mevcut kanallardan tarımsal sulama başlatıldı."),
            new DecisionOption(Store, "C. Kurak dönem için su depola",
                "Yatırım gerektirmez; bu tur yalnızca 2 su kullanılır. Sulama ertelendiği için bitki desteği -5.",
                0, 0, 0, 0, DecisionEffect.StoreWater,
                advantage: "Ücretsiz; bu tur su tüketimi en fazla 2.",
                tradeoff: "Sulama ertelenir; doğrudan bitki desteği -5.",
                nextEffect: "Su rezervi kalır; tarım ve doğal yaşam zorlanabilir.", feedback: "Su, gelecek kurak dönem için ayrıldı."));

        public static readonly ReadOnlyCollection<DecisionPointDefinition> All =
            Array.AsReadOnly(new[] { EnergyStation, WaterStation, FarmStation });
    }
}
