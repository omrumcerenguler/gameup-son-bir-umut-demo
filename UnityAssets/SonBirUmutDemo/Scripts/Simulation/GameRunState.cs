using System;
using System.Collections.Generic;
using SonBirUmutDemo.Decisions;

namespace SonBirUmutDemo.Simulation
{
    public enum GameMode { LegacyDecisions, CrisisFoundation }
    public enum GameRunPhase { Intro, Preparation, CrisisManagement, Ending }

    public sealed class GameRunState
    {
        private readonly GameRunSettings settings;
        private readonly ResourceState initial;
        private readonly DecisionPointDefinition[] definitions;
        private readonly SessionTimer timer;
        private readonly CrisisPressure pressure;
        private bool hasFocus = true, applicationPaused, discardNextDelta;
        public GameMode Mode { get; }
        public GameSession Session { get; private set; }
        public GameRunPhase Phase { get; private set; }
        public RunOutcome Result { get; private set; }
        public bool IsEnded => Phase == GameRunPhase.Ending;
        public bool MovementLocked => Phase == GameRunPhase.Intro || IsEnded;
        public bool UsesCrisisMode => Mode == GameMode.CrisisFoundation;
        public double RemainingSeconds => timer.RemainingSeconds;
        public double CrisisPressure => pressure.Value;
        public double PressureRiseRate => settings.RiseRate(Session.State);

        public GameRunState(GameMode mode = GameMode.LegacyDecisions, GameRunSettings settings = null,
            ResourceState initial = null, IEnumerable<DecisionPointDefinition> definitions = null)
        {
            if (!Enum.IsDefined(typeof(GameMode), mode)) throw new ArgumentOutOfRangeException(nameof(mode));
            Mode = mode;
            this.settings = settings ?? new GameRunSettings();
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
        }

        public void SetApplicationPaused(bool paused)
        {
            if (applicationPaused == paused) return;
            applicationPaused = paused; discardNextDelta = true;
        }

        public void Tick(double seconds, bool cardsOpen)
        {
            RunNumbers.NonNegative(seconds, nameof(seconds));
            if (!UsesCrisisMode || IsEnded || Phase == GameRunPhase.Intro || cardsOpen || !hasFocus || applicationPaused) return;
            // Native focus/pause callbacks arm this guard even when no background frames execute.
            if (discardNextDelta) { discardNextDelta = false; return; }
            double used = timer.Advance(seconds, true);
            pressure.Change(used * PressureRiseRate);
            if (timer.IsExpired) Finish(RunEndReason.TimeExpired);
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
                if (UsesCrisisMode) Phase = GameRunPhase.CrisisManagement;
                else Finish(RunEndReason.StrategicDecisionsCompleted);
            }
            return true;
        }

        // Extension point only. No maintenance task or pressure-reduction UI exists in stage 1.
        public bool TryReducePressure(double amount)
        {
            RunNumbers.NonNegative(amount, nameof(amount));
            if (!UsesCrisisMode || Phase != GameRunPhase.CrisisManagement) return false;
            pressure.Change(-amount);
            return true;
        }

        private void Finish(RunEndReason reason)
        {
            if (IsEnded) return;
            Result = new RunOutcome(Session, reason);
            Phase = GameRunPhase.Ending;
        }
    }
}
