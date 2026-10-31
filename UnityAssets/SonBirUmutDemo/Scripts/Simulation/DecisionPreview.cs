using SonBirUmutDemo.Decisions;

namespace SonBirUmutDemo.Simulation
{
    // UI preview and application share the same calculation; invalid previews never mutate a session.
    public sealed class DecisionPreview
    {
        public bool CanChoose => After != null;
        public string Reason { get; }
        public int Cost { get; }
        public ResourceState Before { get; }
        public ResourceState After { get; }
        public TurnBreakdown Breakdown { get; }

        public DecisionPreview(ResourceState before, ResourceState after, int cost, string reason, TurnBreakdown breakdown = null)
        {
            Before = before;
            After = after;
            Cost = cost;
            Reason = reason;
            Breakdown = breakdown;
        }

        public static DecisionPreview Blocked(ResourceState before, string reason, int cost = 0) =>
            new DecisionPreview(before, null, cost, reason);
    }

    public sealed class DecisionRecord
    {
        public string PointId { get; }
        public DecisionOption Option { get; }
        public DecisionPreview Result { get; }

        public DecisionRecord(string pointId, DecisionOption option, DecisionPreview result)
        {
            PointId = pointId;
            Option = option;
            Result = result;
        }
    }
}
