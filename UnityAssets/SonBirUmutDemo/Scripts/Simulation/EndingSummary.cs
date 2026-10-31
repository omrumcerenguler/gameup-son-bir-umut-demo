using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SonBirUmutDemo.Decisions;

namespace SonBirUmutDemo.Simulation
{
    public sealed class EndingSummary
    {
        public string DrinkingWater { get; }
        public string Agriculture { get; }
        public string Nature { get; }
        public string RemainingRisk { get; }
        public ReadOnlyCollection<string> KeyDecisions { get; }
        public const string ProxyNote = "Su güvenliği stokla, tarım ve doğa ortak bitki göstergesiyle yorumlanır; su kalitesi veya ayrı ürün verimi ölçülmez.";

        public EndingSummary(ResourceState state, IReadOnlyList<DecisionRecord> history)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            if (history == null) throw new ArgumentNullException(nameof(history));
            DecisionRecord energy = null, water = null, farm = null;
            foreach (var record in history)
            {
                if (record.PointId == DecisionCatalog.EnergyStationId) energy = record;
                else if (record.PointId == DecisionCatalog.WaterStationId) water = record;
                else if (record.PointId == DecisionCatalog.FarmStationId) farm = record;
            }
            string stock = state.Water >= 35 ? "kısa dönem payı var" : state.Water >= 20 ? "rezerv dar" : "stok kritik";
            DrinkingWater = $"İçme suyu için ayrılabilir stok: {state.Water}; {stock}.";
            string plants = state.PlantHealth >= 55 ? "dayanıklı" : state.PlantHealth >= 30 ? "zorlanıyor" : "kritik";
            string farmChoice = farm?.Option.Id;
            Agriculture = (farmChoice == DecisionCatalog.Irrigate ? "Sulamaya öncelik verildi" :
                farmChoice == DecisionCatalog.Store ? "Sulama ertelendi" : "Tarım doğrudan sulanmadı") +
                $"; ortak bitki göstergesi {state.PlantHealth}, {plants}.";
            Nature = (farmChoice == DecisionCatalog.Protect ? "Doğal alan koruması seçildi" : "Doğal alan koruması öncelik olmadı") +
                $"; ortak bitki göstergesi {state.PlantHealth}, {plants}.";

            var risks = new List<string>();
            if (state.Water < 35) risks.Add("kuraklık için su rezervi");
            if (state.PlantHealth < 55) risks.Add("bitki dayanıklılığı");
            if (state.Energy < 15) risks.Add("enerji kapasitesi");
            if (state.Emissions > 12) risks.Add("uzun vadeli emisyon etkisi");
            if (state.Budget < 10 && state.Water < 50) risks.Add("yedek kaynak payı");
            RemainingRisk = risks.Count == 0 ? "Temel ihtiyaçlar için pay kaldı; uzun kuraklıklara karşı izleme sürmeli." :
                "Süren risk: " + string.Join(", ", risks) + ".";

            // Transparent narrative priority, not a new score: water plus the risk-relevant decision.
            // Energy/emissions risk highlights energy; otherwise show the farming tradeoff.
            var highlights = new List<string>();
            if (water != null) highlights.Add(Highlight(water));
            var second = state.Emissions > 12 || state.Energy < 15 ? energy : farm;
            if (second == null) second = energy ?? farm;
            if (second != null) highlights.Add(Highlight(second));
            KeyDecisions = highlights.AsReadOnly();
        }

        private static string Highlight(DecisionRecord record)
        {
            var b = record.Result.Breakdown;
            if (record.PointId == DecisionCatalog.WaterStationId)
                return record.Option.Title + $": {record.Result.Cost} bütçe, seçimden {DecisionMessaging.Signed(b.DecisionWater)} su; tur tüketimi {b.WaterUse}.";
            if (record.PointId == DecisionCatalog.EnergyStationId)
                return record.Option.Title + $": enerji {DecisionMessaging.Signed(b.DecisionEnergy)}, uzun vadeli emisyon {DecisionMessaging.Signed(b.DecisionEmissions)}.";
            return record.Option.Title + $": seçimden su {DecisionMessaging.Signed(b.DecisionWater)}, bitki desteği {DecisionMessaging.Signed(b.DecisionPlants)}.";
        }
    }
}
