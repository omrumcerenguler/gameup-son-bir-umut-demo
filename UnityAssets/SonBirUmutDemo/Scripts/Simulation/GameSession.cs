using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SonBirUmutDemo.Decisions;

namespace SonBirUmutDemo.Simulation
{
    public sealed class GameSession
    {
        private readonly List<DecisionPointDefinition> orderedPoints = new List<DecisionPointDefinition>();
        private readonly Dictionary<string, DecisionPointDefinition> points = new Dictionary<string, DecisionPointDefinition>(StringComparer.Ordinal);
        private readonly Dictionary<string, string> choices = new Dictionary<string, string>(StringComparer.Ordinal);
        private readonly List<DecisionRecord> history = new List<DecisionRecord>();
        private readonly SimulationRules rules;
        public ResourceState State { get; private set; }
        public int CompletedCount => choices.Count;
        public int PointCount => orderedPoints.Count;
        public bool IsComplete => CompletedCount == PointCount;
        public DecisionPointDefinition CurrentPoint => IsComplete ? null : orderedPoints[CompletedCount];
        public ReadOnlyCollection<DecisionRecord> History { get; }
        public EndingResult Ending { get; private set; }
        public EndingSummary Summary { get; private set; }

        public GameSession(ResourceState initial = null, SimulationRules rules = null,
            IEnumerable<DecisionPointDefinition> definitions = null)
        {
            State = initial ?? ResourceState.Initial();
            this.rules = rules ?? new SimulationRules();
            foreach (var point in definitions ?? DecisionCatalog.All)
            {
                if (point == null) throw new ArgumentException("Null decision point.", nameof(definitions));
                points.Add(point.Id, point);
                orderedPoints.Add(point);
            }
            if (orderedPoints.Count == 0) throw new ArgumentException("At least one point is required.", nameof(definitions));
            History = history.AsReadOnly();
        }

        public string GetChoice(string pointId) => pointId != null && choices.TryGetValue(pointId, out var choice) ? choice : null;

        public DecisionPreview GetPreview(string pointId, string optionId)
        {
            if (pointId == null || !points.TryGetValue(pointId, out var point))
                return DecisionPreview.Blocked(State, "Karar noktası bulunamadı.");
            if (choices.ContainsKey(pointId)) return DecisionPreview.Blocked(State, "Bu karar noktası zaten tamamlandı.");
            if (CurrentPoint == null || CurrentPoint.Id != pointId)
                return DecisionPreview.Blocked(State, "Önce sıradaki karar noktasını tamamla.");
            var option = FindOption(point, optionId);
            if (option == null) return DecisionPreview.Blocked(State, "Seçenek bulunamadı.");
            return rules.Preview(State, option, GetChoice(DecisionCatalog.WaterStationId) == DecisionCatalog.Efficiency);
        }

        public bool TryChoose(string pointId, string optionId, out string message)
        {
            var preview = GetPreview(pointId, optionId);
            if (!preview.CanChoose) { message = preview.Reason; return false; }
            var selected = FindOption(points[pointId], optionId);
            State = preview.After;
            choices.Add(pointId, optionId);
            history.Add(new DecisionRecord(pointId, selected, preview));
            if (IsComplete)
            {
                Ending = EndingResult.Evaluate(State);
                Summary = new EndingSummary(State, History);
            }
            message = selected.Title + " — karar uygulandı.";
            return true;
        }

        private static DecisionOption FindOption(DecisionPointDefinition point, string optionId)
        {
            foreach (var option in point.Options) if (option.Id == optionId) return option;
            return null;
        }
    }
}
