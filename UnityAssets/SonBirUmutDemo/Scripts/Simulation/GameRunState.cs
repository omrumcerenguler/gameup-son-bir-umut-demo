using System;
using System.Collections.Generic;
using SonBirUmutDemo.Decisions;

namespace SonBirUmutDemo.Simulation
{
    public enum GameMode { LegacyDecisions, CrisisFoundation, CrisisDemo }
    public enum GameRunPhase { Intro, Preparation, CrisisManagement, Ending }

    public sealed class GameRunState
    {
        private readonly GameRunSettings settings;
        private readonly ResourceState initial;
        private readonly DecisionPointDefinition[] definitions;
        private readonly SessionTimer timer;
        private readonly CrisisPressure pressure;
        private readonly CrisisRules crisisRules;
        private ResourceState crisisResources;
        private bool hasFocus = true, applicationPaused, discardNextDelta;
        public GameMode Mode { get; }
        public GameSession Session { get; private set; }
        public GameRunPhase Phase { get; private set; }
        public RunOutcome Result { get; private set; }
        public bool IsEnded => Phase == GameRunPhase.Ending;
        public bool MovementLocked => Phase == GameRunPhase.Intro || IsEnded;
        public bool UsesCrisisMode => Mode != GameMode.LegacyDecisions;
        public bool UsesCrisisDemo => Mode == GameMode.CrisisDemo;
        public CrisisProgress Crisis { get; private set; }
        public CrisisRules CrisisRules => crisisRules;
        public ResourceState CurrentResources => crisisResources ?? Session.State;
        public double RemainingSeconds => timer.RemainingSeconds;
        public double CrisisPressure => pressure.Value;
        public double PressureRiseRate => settings.RiseRate(CurrentResources);

        public GameRunState(GameMode mode = GameMode.LegacyDecisions, GameRunSettings settings = null,
            ResourceState initial = null, IEnumerable<DecisionPointDefinition> definitions = null, CrisisRules crisisRules = null)
        {
            if (!Enum.IsDefined(typeof(GameMode), mode)) throw new ArgumentOutOfRangeException(nameof(mode));
            Mode = mode;
            this.settings = settings ?? new GameRunSettings();
            this.crisisRules = crisisRules ?? new CrisisRules();
            this.initial = initial ?? ResourceState.Initial();
            this.definitions = new List<DecisionPointDefinition>(definitions ?? DecisionCatalog.All).ToArray();
            timer = new SessionTimer(this.settings.DurationSeconds);
            pressure = new CrisisPressure(UsesCrisisMode ? this.settings.InitialPressure : 0);
            Reset();
        }

        public void Reset()
        {
            Session = new GameSession(initial, definitions: definitions);
            timer.Reset(); pressure.Reset();
            crisisResources = null; Crisis = UsesCrisisDemo ? new CrisisProgress(crisisRules) : null;
            Phase = GameRunPhase.Intro; Result = null;
            hasFocus = true; applicationPaused = discardNextDelta = false;
        }

        public bool Start()
        {
            if (Phase != GameRunPhase.Intro) return false;
            Phase = GameRunPhase.Preparation;
            return true;
        }

        public void SetFocus(bool focused)
        {
            if (hasFocus == focused) return;
            hasFocus = focused; discardNextDelta = true;
            Crisis?.Emergency.CancelRepair();
        }

        public void SetApplicationPaused(bool paused)
        {
            if (applicationPaused == paused) return;
            applicationPaused = paused; discardNextDelta = true;
            Crisis?.Emergency.CancelRepair();
        }

        public void Tick(double seconds, bool cardsOpen, bool repairHeld = false, bool repairInRange = false)
        {
            RunNumbers.NonNegative(seconds, nameof(seconds));
            if (!UsesCrisisMode || IsEnded || Phase == GameRunPhase.Intro || cardsOpen || !hasFocus || applicationPaused)
            { Crisis?.Emergency.CancelRepair(); return; }
            if (discardNextDelta) { discardNextDelta = false; Crisis?.Emergency.CancelRepair(); return; }
            if (!repairHeld || !repairInRange) Crisis?.Emergency.CancelRepair();
            if (!UsesCrisisDemo || Phase != GameRunPhase.CrisisManagement)
            {
                double used = timer.Advance(seconds, true);
                pressure.Change(used * PressureRiseRate);
                if (timer.IsExpired) Finish(RunEndReason.TimeExpired);
                return;
            }

            // Split only at actual event/stability boundaries, not frame-sized polling steps.
            // This preserves the same result for 30/60/120 FPS and large test deltas.
            double pending = Math.Min(seconds, timer.RemainingSeconds);
            while (pending > 0 && !IsEnded)
            {
                var incident = Crisis.Emergency;
                double rate = PressureRiseRate;
                bool eligible = Crisis.CompletedCount >= 2 && incident.IsResolved &&
                    pressure.Value <= crisisRules.StableThreshold;
                double untilHigh = rate > 0 ? (crisisRules.StableThreshold - pressure.Value) / rate : double.PositiveInfinity;
                if (eligible && untilHigh <= 1e-9) eligible = false;
                if (!eligible) Crisis.StableSeconds = 0;
                double step = Math.Min(pending, incident.NextBoundary(repairHeld, repairInRange));
                if (eligible) step = Math.Min(step, Math.Min(untilHigh, crisisRules.StableDuration - Crisis.StableSeconds));
                double used = timer.Advance(step, true);
                pressure.Change(used * rate);
                if (eligible) Crisis.StableSeconds += used;
                var before = incident.Status;
                incident.Advance(used, repairHeld, repairInRange);
                if (before == EmergencyStatus.Active && incident.Status == EmergencyStatus.Repaired)
                    pressure.Change(-CrisisRules.RepairRelief);
                else if (before == EmergencyStatus.Active && incident.Status == EmergencyStatus.Expired)
                {
                    pressure.Change(CrisisRules.MissedPressure);
                    ChangeResources(-CrisisRules.MissedWater, 0);
                }
                pending = Math.Max(0, pending - used);
                if (eligible && Crisis.StableSeconds >= crisisRules.StableDuration - 1e-9)
                    Finish(RunEndReason.CrisisStabilized);
                else if (timer.IsExpired) Finish(RunEndReason.TimeExpired);
            }
        }

        public bool TryCompleteMaintenance(string stationId, out string message)
        {
            var task = Crisis?.AtStation(stationId);
            if (!UsesCrisisDemo || Phase != GameRunPhase.CrisisManagement || !hasFocus || applicationPaused ||
                task == null || Crisis.Emergency.Status == EmergencyStatus.Active || !task.Complete())
            { message = "Bu bakım şu anda uygulanamaz veya zaten tamamlandı."; return false; }
            pressure.Change(-task.Effects.PressureRelief);
            ChangeResources(task.Effects.WaterProtection, task.Effects.PlantSupport);
            message = task.Title + $" tamamlandı. Baskı -{task.Effects.PressureRelief:0}" +
                (task.Effects.WaterProtection > 0 ? $"; korunan su +{task.Effects.WaterProtection}" : "") +
                (task.Effects.PlantSupport > 0 ? $"; bitki desteği +{task.Effects.PlantSupport}" : "") + ".";
            return true;
        }

        private void ChangeResources(int water, int plants)
        {
            var state = CurrentResources;
            crisisResources = new ResourceState(state.Budget, state.Energy, state.Water + water,
                state.PlantHealth + plants, state.Emissions);
        }

        public bool TryChoose(string pointId, string optionId, out string message)
        {
            if (Phase != GameRunPhase.Preparation)
            {
                message = "Stratejik kararlar yalnız hazırlık aşamasında verilebilir.";
                return false;
            }
            if (!Session.TryChoose(pointId, optionId, out message)) return false;
            if (Session.IsComplete)
            {
                if (UsesCrisisMode)
                {
                    Phase = GameRunPhase.CrisisManagement;
                    if (UsesCrisisDemo) { crisisResources = Session.State; Crisis.Begin(timer.RemainingSeconds); }
                }
                else Finish(RunEndReason.StrategicDecisionsCompleted);
            }
            return true;
        }

        // Foundation compatibility API; the finished demo accepts only one-shot task/event rewards.
        public bool TryReducePressure(double amount)
        {
            RunNumbers.NonNegative(amount, nameof(amount));
            if (!UsesCrisisMode || UsesCrisisDemo || Phase != GameRunPhase.CrisisManagement) return false;
            pressure.Change(-amount);
            return true;
        }

        private void Finish(RunEndReason reason)
        {
            if (IsEnded) return;
            Result = UsesCrisisDemo ? new RunOutcome(Session, reason, CurrentResources, Crisis, CrisisPressure) : new RunOutcome(Session, reason);
            Phase = GameRunPhase.Ending;
        }
    }
}
