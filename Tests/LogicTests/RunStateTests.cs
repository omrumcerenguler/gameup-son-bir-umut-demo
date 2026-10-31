using System;
using SonBirUmutDemo.Decisions;
using SonBirUmutDemo.Simulation;

internal static class RunStateTests
{
    public static void Register(Action<string, Action> run)
    {
        run("Run: sprint intent and release", () => {
            Near(MovementPolicy.TargetSpeed(3.5f, true, false), 5.6, .00001);
            Near(MovementPolicy.TargetSpeed(3.5f, false, false), 3.5);
        });
        run("Run: modal movement lock overrides sprint", () => {
            Check(MovementPolicy.TargetSpeed(3.5f, true, true) == 0);
            Check(MovementPolicy.TargetSpeed(3.5f, false, true) == 0);
        });
        run("Run: invalid movement configuration", () => {
            Reject(() => MovementPolicy.TargetSpeed(float.NaN, true, false));
            Reject(() => MovementPolicy.TargetSpeed(-1, false, false));
            Reject(() => MovementPolicy.TargetSpeed(3, true, false, .5f));
        });
        run("Run: timer pause fractional advance and reset", () => {
            var timer = new SessionTimer();
            Near(timer.Advance(60, false), 0); Near(timer.RemainingSeconds, 240);
            Near(timer.Advance(.125, true), .125); Near(timer.RemainingSeconds, 239.875);
            timer.Reset(); Near(timer.RemainingSeconds, 240);
        });
        run("Run: timer consumes only remaining time", () => {
            var timer = new SessionTimer(2);
            Near(timer.Advance(100, true), 2); Check(timer.IsExpired);
            Near(timer.Advance(1, true), 0); Near(timer.RemainingSeconds, 0);
        });
        run("Run: invalid clocks rejected", () => {
            Reject(() => new SessionTimer(0)); Reject(() => new SessionTimer(double.NaN));
            Reject(() => new SessionTimer().Advance(-1, false));
            Reject(() => new SessionTimer().Advance(double.PositiveInfinity, true));
        });
        run("Run: pressure clamps and resets", () => {
            var pressure = new CrisisPressure(60);
            pressure.Change(100); Near(pressure.Value, 100);
            pressure.Change(-200); Near(pressure.Value, 0);
            pressure.Reset(); Near(pressure.Value, 60);
            Reject(() => pressure.Change(double.NaN));
        });
        run("Run: pressure rates reflect energy water and plants", () => {
            var settings = new GameRunSettings();
            var good = new ResourceState(100, 100, 100, 100, 0);
            double best = settings.RiseRate(good);
            Check(settings.RiseRate(new ResourceState(100, 0, 100, 100, 0)) > best);
            Check(settings.RiseRate(new ResourceState(100, 100, 0, 100, 0)) > best);
            Check(settings.RiseRate(new ResourceState(100, 100, 100, 0, 0)) > best);
            Near(best, settings.RiseRate(new ResourceState(0, 100, 100, 100, 100)));
        });
        run("Run: configured pressure ceiling and invalid settings", () => {
            var settings = new GameRunSettings(baseRise: 1, waterRiskRise: 1, maximumRisePerSecond: .1);
            Near(settings.RiseRate(ResourceState.Initial()), .1);
            Reject(() => new GameRunSettings(initialPressure: 101));
            Reject(() => new GameRunSettings(baseRise: -1));
            Reject(() => new GameRunSettings(durationSeconds: double.PositiveInfinity));
            Reject(() => new GameRunState((GameMode)42));
        });
        run("Run: intro pauses time pressure and decisions", () => {
            var state = NewRun(); state.Tick(120, false);
            Check(state.MovementLocked && state.Phase == GameRunPhase.Intro);
            Near(state.RemainingSeconds, 240); Near(state.CrisisPressure, 60);
            Check(!state.TryChoose(DecisionCatalog.EnergyStationId, DecisionCatalog.Solar, out _));
            Check(state.Start() && !state.Start() && !state.MovementLocked);
        });
        run("Run: preparation timer and modal pause", () => {
            var state = NewRun(); state.Start(); state.Tick(10, true);
            Near(state.RemainingSeconds, 240); Near(state.CrisisPressure, 60);
            state.Tick(10, false); Near(state.RemainingSeconds, 230); Check(state.CrisisPressure > 60);
        });
        run("Run: focus return discards oversized first delta", () => {
            var state = NewRun(); state.Start(); state.Tick(1, false);
            state.SetFocus(false); state.Tick(500, false); Near(state.RemainingSeconds, 239);
            state.SetFocus(true); state.Tick(500, false); Near(state.RemainingSeconds, 239);
            state.Tick(1, false); Near(state.RemainingSeconds, 238);
        });
        run("Run: focus guard survives card pause", () => {
            var state = NewRun(); state.Start(); state.SetFocus(false); state.SetFocus(true);
            state.Tick(500, true); state.Tick(500, false); Near(state.RemainingSeconds, 240);
            state.Tick(1, false); Near(state.RemainingSeconds, 239);
        });
        run("Run: native pause independent of focus", () => {
            var state = NewRun(); state.Start(); state.SetApplicationPaused(true);
            state.SetFocus(false); state.SetFocus(true); state.Tick(500, false);
            Near(state.RemainingSeconds, 240);
            state.SetApplicationPaused(false); state.Tick(500, false); Near(state.RemainingSeconds, 240);
            state.Tick(2, false); Near(state.RemainingSeconds, 238);
        });
        run("Run: three decisions enter crisis without ending", () => {
            var state = NewRun(); state.Start(); Prepare(state);
            Check(state.Session.IsComplete && !state.IsEnded && state.Result == null);
            Check(state.Phase == GameRunPhase.CrisisManagement && !state.MovementLocked);
            Check(!state.TryChoose(DecisionCatalog.EnergyStationId, DecisionCatalog.Solar, out _));
            state.Tick(1, false); Near(state.RemainingSeconds, 239);
        });
        run("Run: failed decision is phase and resource atomic", () => {
            var state = NewRun(); state.Start(); var before = state.Session.State;
            Check(!state.TryChoose(DecisionCatalog.WaterStationId, DecisionCatalog.Pump, out _));
            Check(ReferenceEquals(before, state.Session.State) && state.Phase == GameRunPhase.Preparation);
        });
        run("Run: legacy mode stays untimed and ends at three", () => {
            var state = new GameRunState(); state.Start(); state.Tick(10000, false);
            Near(state.RemainingSeconds, 240); Near(state.CrisisPressure, 0);
            Prepare(state); Check(state.IsEnded && state.MovementLocked);
            Check(state.Result.Reason == RunEndReason.StrategicDecisionsCompleted);
            Check(state.Result.Category == state.Session.Ending.Category);
            Check(ReferenceEquals(state.Result.Summary, state.Session.Summary));
        });
        run("Run: legacy one station compatibility", () => {
            var state = new GameRunState(definitions: new[] { DecisionCatalog.EnergyStation }); state.Start();
            Select(state, DecisionCatalog.Solar); Check(state.IsEnded && state.Session.CompletedCount == 1);
        });
        for (int count = 0; count < 3; count++)
        {
            int selected = count;
            run("Run: safe timeout after " + count + " decisions", () => {
                var state = NewRun(2); state.Start();
                if (selected > 0) Select(state, DecisionCatalog.Solar);
                if (selected > 1) Select(state, DecisionCatalog.Efficiency);
                state.Tick(2, false);
                Check(state.IsEnded && state.MovementLocked && !state.Session.IsComplete);
                Check(state.Session.Ending == null && state.Result.Summary != null);
                Check(state.Result.CompletedDecisions == selected && state.Result.RequiredDecisions == 3);
                Check(state.Result.Explanation.Contains("eksik") && state.Result.Category != EndingCategory.Rebirth);
            });
        }
        run("Run: timeout after preparation retains strategic outcome", () => {
            var state = NewRun(2); state.Start(); Prepare(state); state.Tick(2, false);
            Check(state.Result.Reason == RunEndReason.TimeExpired && state.IsEnded);
            Check(state.Result.Category == state.Session.Ending.Category);
        });
        run("Run: no pressure overcount or repeated finalization", () => {
            var state = NewRun(2); state.Start(); double rate = state.PressureRiseRate;
            state.Tick(1000, false); Near(state.CrisisPressure, 60 + 2 * rate);
            var result = state.Result; state.Tick(1000, false);
            Check(ReferenceEquals(result, state.Result)); Near(state.CrisisPressure, 60 + 2 * rate);
            Check(!state.TryChoose(DecisionCatalog.EnergyStationId, DecisionCatalog.Solar, out _));
        });
        run("Run: future reduction API obeys phase and bounds", () => {
            var state = NewRun(); Check(!state.TryReducePressure(10)); state.Start();
            Check(!state.TryReducePressure(10)); Prepare(state);
            Check(state.TryReducePressure(1000)); Near(state.CrisisPressure, 0);
            Reject(() => state.TryReducePressure(-1));
            state.Tick(240, false); Check(!state.TryReducePressure(10));
        });
        run("Run: replay clears all session state", () => {
            var state = NewRun(2); state.Start(); Prepare(state); state.TryReducePressure(10); state.Tick(2, false);
            var old = state.Session; state.SetFocus(false); state.SetApplicationPaused(true); state.Reset();
            Check(!ReferenceEquals(old, state.Session) && state.Result == null && state.MovementLocked);
            Check(state.Session.CompletedCount == 0 && state.Session.History.Count == 0);
            Near(state.RemainingSeconds, 2); Near(state.CrisisPressure, 60);
            state.Start(); state.Tick(1, false); Near(state.RemainingSeconds, 1);
        });
        run("Run: legacy wrapper preserves all 27 decision paths", () => {
            foreach (var e in DecisionCatalog.EnergyStation.Options)
            foreach (var w in DecisionCatalog.WaterStation.Options)
            foreach (var f in DecisionCatalog.FarmStation.Options)
            {
                var state = new GameRunState(); state.Start(); var direct = new GameSession();
                foreach (string id in new[] { e.Id, w.Id, f.Id })
                {
                    string point = direct.CurrentPoint.Id;
                    bool expected = direct.TryChoose(point, id, out _);
                    Check(state.TryChoose(point, id, out _) == expected);
                    if (!expected)
                    {
                        foreach (var option in direct.CurrentPoint.Options)
                            if (direct.TryChoose(point, option.Id, out _))
                            { Check(state.TryChoose(point, option.Id, out _)); break; }
                    }
                }
                var a = state.Session.State; var b = direct.State;
                Check(a.Budget == b.Budget && a.Energy == b.Energy && a.Water == b.Water &&
                    a.PlantHealth == b.PlantHealth && a.Emissions == b.Emissions);
                Check(state.Result.Category == direct.Ending.Category);
            }
        });
    }

    private static GameRunState NewRun(double seconds = 240) =>
        new GameRunState(GameMode.CrisisFoundation, new GameRunSettings(durationSeconds: seconds));
    private static void Prepare(GameRunState state)
    { Select(state, DecisionCatalog.Solar); Select(state, DecisionCatalog.Efficiency); Select(state, DecisionCatalog.Protect); }
    private static void Select(GameRunState state, string id) => Check(state.TryChoose(state.Session.CurrentPoint.Id, id, out _));
    private static void Check(bool condition) { if (!condition) throw new Exception("Assertion failed"); }
    private static void Near(double a, double b, double epsilon = .0000001) => Check(Math.Abs(a - b) < epsilon);
    private static void Reject(Action action)
    {
        try { action(); } catch (ArgumentException) { return; }
        throw new Exception("Expected invalid argument rejection");
    }
}
