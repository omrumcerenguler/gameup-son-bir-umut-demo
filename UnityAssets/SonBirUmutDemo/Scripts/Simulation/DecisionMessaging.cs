using System;
using System.Collections.Generic;

namespace SonBirUmutDemo.Simulation
{
    // Player-facing explanations use the calculation record, never a second simulation.
    public static class DecisionMessaging
    {
        public static string Signed(int value) => value >= 0 ? "+" + value : value.ToString();

        public static string CriticalResources(ResourceState state, bool choicesRemain = true)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            var risks = new List<string>();
            if (state.Water < 20) risks.Add("Su kritik");
            else if (state.Water < 35) risks.Add("Su rezervi dar");
            if (state.PlantHealth < 30) risks.Add("Bitkiler kritik");
            if (state.Energy < 10) risks.Add("Pompa için enerji yetersiz");
            if (state.Budget < 10) risks.Add(choicesRemain ? "Yatırım bütçesi dar; ücretsiz seçenek var" : "Yatırım bütçesi dar");
            if (state.Emissions > 12) risks.Add("Uzun vadeli emisyon etkisi yüksek");
            return risks.Count == 0 ? "Kaynaklar şu an kritik eşiklerde değil; kuraklık baskısı sürüyor." : string.Join(" · ", risks);
        }

        public static string ChoiceEffects(DecisionPreview preview)
        {
            var b = preview.Breakdown;
            return $"Seçim: Enerji {Signed(b.DecisionEnergy)} | Su {Signed(b.DecisionWater)} | Bitki {Signed(b.DecisionPlants)} | Emisyon {Signed(b.DecisionEmissions)}";
        }

        public static string TurnEffects(DecisionPreview preview)
        {
            var b = preview.Breakdown;
            return $"Tur su kullanımı: -{b.WaterUse} | Su koşulu: Bitki {Signed(b.WaterPlantEffect)}\nKuraklık baskısı: Bitki -{b.DroughtDamage}";
        }

        public static string NetEffects(DecisionPreview preview)
        {
            var a = preview.Before;
            var b = preview.After;
            return $"Net: Enerji {Signed(b.Energy - a.Energy)} | Su {Signed(b.Water - a.Water)} | Bitki {Signed(b.PlantHealth - a.PlantHealth)} | Emisyon {Signed(b.Emissions - a.Emissions)}";
        }

        public static string SelectionFeedback(DecisionRecord record)
        {
            string heading = string.IsNullOrEmpty(record.Option.Feedback) ? record.Option.Title : record.Option.Feedback;
            return heading + $" Bütçe -{record.Result.Cost}.\n" +
                ChoiceEffects(record.Result) + "\n" + TurnEffects(record.Result).Replace("\n", " | ") + "\n" + NetEffects(record.Result);
        }
    }
}
