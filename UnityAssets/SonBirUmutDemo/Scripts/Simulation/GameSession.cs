using System;
using System.Collections.Generic;
using SonBirUmutDemo.Decisions;

namespace SonBirUmutDemo.Simulation
{
    public sealed class GameSession
    {
        private readonly Dictionary<string, DecisionPointDefinition> points = new Dictionary<string, DecisionPointDefinition>(StringComparer.Ordinal);
        private readonly Dictionary<string, string> choices = new Dictionary<string, string>(StringComparer.Ordinal);
        private readonly SimulationRules rules;
        public ResourceState State { get; private set; }
        public int CompletedCount => choices.Count;

        public GameSession(ResourceState initial = null, SimulationRules rules = null,
            IEnumerable<DecisionPointDefinition> definitions = null)
        {
            State = initial ?? ResourceState.Initial();
            this.rules = rules ?? new SimulationRules();
            foreach (var point in definitions ?? new[] { DecisionCatalog.EnergyStation })
            {
                if (point == null) throw new ArgumentException("Null decision point.", nameof(definitions));
                points.Add(point.Id, point);
            }
        }

        public string GetChoice(string pointId) => pointId != null && choices.TryGetValue(pointId, out var choice) ? choice : null;

        public bool TryChoose(string pointId, string optionId, out string message)
        {
            if (pointId == null || !points.TryGetValue(pointId, out var point))
            {
                message = "Karar noktası bulunamadı.";
                return false;
            }
            if (choices.ContainsKey(pointId))
            {
                message = "Bu karar noktası zaten tamamlandı.";
                return false;
            }
            DecisionOption selected = null;
            foreach (var option in point.Options)
                if (option.Id == optionId) selected = option;
            if (selected == null)
            {
                message = "Seçenek bulunamadı.";
                return false;
            }
            if (State.Budget < selected.Cost)
            {
                message = "Bu seçim için bütçe yetersiz.";
                return false;
            }

            State = rules.ApplyTurn(State, selected);
            choices.Add(pointId, selected.Id);
            message = selected.Title + " — karar uygulandı; kuraklık turu hesaplandı.";
            return true;
        }
    }
}
