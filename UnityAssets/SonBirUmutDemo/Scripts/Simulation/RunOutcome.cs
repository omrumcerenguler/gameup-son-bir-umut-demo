namespace SonBirUmutDemo.Simulation
{
    public enum RunEndReason { StrategicDecisionsCompleted, TimeExpired, CrisisStabilized }

    public sealed class RunOutcome
    {
        public RunEndReason Reason { get; }
        public EndingCategory Category { get; }
        public string Title { get; }
        public string Explanation { get; }
        public EndingSummary Summary { get; }
        public int CompletedDecisions { get; }
        public int RequiredDecisions { get; }
        public bool CrisisStabilized { get; }
        public string ManagementSummary { get; } = "";
        public ResourceState Resources { get; }

        internal RunOutcome(GameSession session, RunEndReason reason, ResourceState current,
            CrisisProgress progress, double pressure)
        {
            Reason = reason; Resources = current;
            CompletedDecisions = session.CompletedCount; RequiredDecisions = session.PointCount;
            CrisisStabilized = reason == RunEndReason.CrisisStabilized;
            var village = EndingResult.Evaluate(current);
            Category = village.Category == EndingCategory.Rebirth && !CrisisStabilized
                ? EndingCategory.PartialTransition : village.Category;
            Title = Category == EndingCategory.Rebirth ? "Yeniden Doğuş" :
                Category == EndingCategory.PartialTransition ? "Eksik Dönüşüm" : "Kaybedilen Gelecek";
            Summary = new EndingSummary(current, session.History);
            string incident = progress.Emergency.Status == EmergencyStatus.Repaired ? "onarıldı" :
                progress.Emergency.Status == EmergencyStatus.Expired ? "süresi doldu" :
                progress.Emergency.Status == EmergencyStatus.Active ? "çözülmedi" : "henüz oluşmadı";
            ManagementSummary = (CrisisStabilized ? "Kriz nöbeti istikrara kavuştu." : "Süre doldu; nöbet istikrara kavuşmadı.") +
                $" Karar {CompletedDecisions}/{RequiredDecisions}, bakım {progress.CompletedCount}/{progress.Tasks.Count}, baskı {pressure:0.0}. Arıza: {incident}.";
            Explanation = (!session.IsComplete ? "Stratejik hazırlık eksik kaldı. " : "") + village.Explanation;
            if (Category != village.Category) Explanation += " En iyi genel sonuç için kriz nöbeti de istikrara kavuşmalı.";
            Explanation += " Kuraklığa hazırlık sürer; iklim değişikliği ortadan kalkmadı.";
        }

        internal RunOutcome(GameSession session, RunEndReason reason)
        {
            Resources = session.State;
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
