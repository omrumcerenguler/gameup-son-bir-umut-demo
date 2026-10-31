namespace SonBirUmutDemo.Simulation
{
    public enum RunEndReason { StrategicDecisionsCompleted, TimeExpired }

    public sealed class RunOutcome
    {
        public RunEndReason Reason { get; }
        public EndingCategory Category { get; }
        public string Title { get; }
        public string Explanation { get; }
        public EndingSummary Summary { get; }
        public int CompletedDecisions { get; }
        public int RequiredDecisions { get; }

        internal RunOutcome(GameSession session, RunEndReason reason)
        {
            Reason = reason;
            CompletedDecisions = session.CompletedCount;
            RequiredDecisions = session.PointCount;
            var strategic = session.Ending ?? EndingResult.Evaluate(session.State);
            // Early timeout cannot claim a successful preparation from untouched initial resources.
            Category = !session.IsComplete && strategic.Category == EndingCategory.Rebirth
                ? EndingCategory.PartialTransition : strategic.Category;
            Title = Category == strategic.Category ? strategic.Title : "Eksik Dönüşüm";
            Summary = session.Summary ?? new EndingSummary(session.State, session.History);
            if (!session.IsComplete)
                Explanation = $"Nöbet süresi doldu. {CompletedDecisions}/{RequiredDecisions} stratejik karar tamamlandı; hazırlık eksik kaldı. Son durum mevcut kaynaklara göre değerlendirildi.";
            else Explanation = reason == RunEndReason.TimeExpired
                ? "Nöbet süresi doldu. Stratejik hazırlığın sonucu: " + strategic.Explanation : strategic.Explanation;
        }
    }
}
