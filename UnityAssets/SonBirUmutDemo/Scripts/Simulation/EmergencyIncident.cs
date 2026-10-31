using System;
using SonBirUmutDemo.Decisions;

namespace SonBirUmutDemo.Simulation
{
    public enum EmergencyStatus { Waiting, Active, Repaired, Expired }

    // Only GameRunState advances this clock; there is no independent Unity Update.
    public sealed class EmergencyIncident
    {
        private readonly CrisisRules rules;
        private double triggerRemaining = double.PositiveInfinity;
        public string StationId => DecisionCatalog.WaterStationId;
        public EmergencyStatus Status { get; private set; }
        public double RemainingSeconds { get; private set; }
        public double RepairSeconds { get; private set; }
        public double RepairFraction => RepairSeconds / rules.RepairDuration;
        public bool IsResolved => Status == EmergencyStatus.Repaired || Status == EmergencyStatus.Expired;
        internal EmergencyIncident(CrisisRules rules) { this.rules = rules; RemainingSeconds = rules.EmergencyWindow; }
        internal void Schedule(double sessionRemaining)
        {
            // Even a late preparation exposes the incident before the global clock ends.
            triggerRemaining = Math.Min(rules.EmergencyDelay, sessionRemaining / 4);
            if (triggerRemaining == 0) Status = EmergencyStatus.Active;
        }
        internal void CancelRepair() { if (Status == EmergencyStatus.Active) RepairSeconds = 0; }
        internal double NextBoundary(bool held, bool inRange)
        {
            if (Status == EmergencyStatus.Waiting) return triggerRemaining;
            if (Status != EmergencyStatus.Active) return double.PositiveInfinity;
            return held && inRange ? Math.Min(RemainingSeconds, rules.RepairDuration - RepairSeconds) : RemainingSeconds;
        }
        internal void Advance(double seconds, bool held, bool inRange)
        {
            if (Status == EmergencyStatus.Waiting)
            {
                triggerRemaining = Math.Max(0, triggerRemaining - seconds);
                if (triggerRemaining <= 1e-9) Status = EmergencyStatus.Active;
                return; // No repair credit for time before activation.
            }
            if (Status != EmergencyStatus.Active) return;
            RemainingSeconds = Math.Max(0, RemainingSeconds - seconds);
            RepairSeconds = held && inRange ? Math.Min(rules.RepairDuration, RepairSeconds + seconds) : 0;
            // A repair finishing exactly on the deadline succeeds.
            if (RepairSeconds >= rules.RepairDuration - 1e-9) Status = EmergencyStatus.Repaired;
            else if (RemainingSeconds <= 1e-9) { Status = EmergencyStatus.Expired; RepairSeconds = 0; }
        }
    }
}
