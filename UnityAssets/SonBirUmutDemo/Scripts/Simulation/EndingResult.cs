namespace SonBirUmutDemo.Simulation
{
    public enum EndingCategory { Rebirth, PartialTransition, LostFuture }

    public sealed class EndingResult
    {
        public EndingCategory Category { get; }
        public string Title { get; }
        public string Explanation { get; }

        private EndingResult(EndingCategory category, string title, string explanation)
        {
            Category = category;
            Title = title;
            Explanation = explanation;
        }

        public static EndingResult Evaluate(ResourceState state)
        {
            if (state == null) throw new System.ArgumentNullException(nameof(state));
            // Resilience needs water, plants, usable energy, low emissions AND a resource buffer.
            bool buffer = state.Budget >= 10 || state.Water >= 50;
            if (state.Water >= 35 && state.PlantHealth >= 55 && state.Energy >= 15 && state.Emissions <= 12 && buffer)
                return new EndingResult(EndingCategory.Rebirth, "Yeniden Doğuş",
                    "Su ve bitki yaşamı korunurken enerji kapasitesi ve bir kaynak tamponu kaldı. Düşük emisyon etkisi uzun vadeli dayanıklılığı destekliyor.");
            bool critical = (state.Water < 20 && state.PlantHealth < 30) ||
                (state.PlantHealth < 15 && state.Energy < 20) || (state.Water < 10 && state.Budget < 10);
            if (critical)
                return new EndingResult(EndingCategory.LostFuture, "Kaybedilen Gelecek",
                    "Su, bitki yaşamı ve kalan kaynakların birleşimi kurak dönemi karşılamaya yetmedi. Başka bir kaynak dağılımını yeniden deneyebilirsin.");
            return new EndingResult(EndingCategory.PartialTransition, "Eksik Dönüşüm",
                "Bazı ihtiyaçlar karşılandı; ancak su, bitki yaşamı, enerji, emisyon veya kaynak tamponunda açıklar kaldı. Köyün dönüşümü sürmeli.");
        }
    }
}
