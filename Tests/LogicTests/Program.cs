using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SonBirUmutDemo.Decisions;
using SonBirUmutDemo.Simulation;

internal static class Program
{
    private static int passed;
    private static int failed;

    private static int Main(string[] args)
    {
        Run("Initial state", () => CheckState(ResourceState.Initial(), 100, 20, 40, 55, 0));
        Run("Solar outcome", () => CheckState(Choose(DecisionCatalog.Solar), 40, 55, 32, 41, 1));
        Run("Diesel outcome", () => CheckState(Choose(DecisionCatalog.Diesel), 75, 70, 29, 41, 18));
        Run("Conservation outcome", () => CheckState(Choose(DecisionCatalog.Conservation), 100, 15, 40, 53, 0));
        Run("Unaffordable choice is atomic", () =>
        {
            var session = new GameSession(new ResourceState(59, 20, 40, 55, 0));
            var original = session.State;
            Check(!session.TryChoose(DecisionCatalog.EnergyStationId, DecisionCatalog.Solar, out _));
            Check(ReferenceEquals(original, session.State));
            Check(session.CompletedCount == 0 && session.GetChoice(DecisionCatalog.EnergyStationId) == null);
            Check(session.TryChoose(DecisionCatalog.EnergyStationId, DecisionCatalog.Conservation, out _));
        });
        Run("Exact budget is accepted", () =>
        {
            var session = new GameSession(new ResourceState(60, 20, 40, 55, 0));
            Check(session.TryChoose(DecisionCatalog.EnergyStationId, DecisionCatalog.Solar, out _));
            Check(session.State.Budget == 0);
        });
        Run("Unknown and null ids do not advance a turn", () =>
        {
            var session = new GameSession();
            var original = session.State;
            Check(!session.TryChoose("unknown", DecisionCatalog.Solar, out _));
            Check(!session.TryChoose(null, DecisionCatalog.Solar, out _));
            Check(!session.TryChoose(DecisionCatalog.EnergyStationId, "unknown", out _));
            Check(!session.TryChoose(DecisionCatalog.EnergyStationId, null, out _));
            Check(ReferenceEquals(original, session.State) && session.CompletedCount == 0);
        });
        Run("Completed point cannot apply a second choice", () =>
        {
            var session = new GameSession();
            Check(session.TryChoose(DecisionCatalog.EnergyStationId, DecisionCatalog.Solar, out _));
            var original = session.State;
            Check(!session.TryChoose(DecisionCatalog.EnergyStationId, DecisionCatalog.Diesel, out _));
            Check(ReferenceEquals(original, session.State) && session.CompletedCount == 1);
            Check(session.GetChoice(DecisionCatalog.EnergyStationId) == DecisionCatalog.Solar);
        });
        Run("Multiple registered points are independent", () =>
        {
            var second = new DecisionPointDefinition("future-point", new DecisionOption("save", "Test", "", 5, 0, 20, 0));
            var session = new GameSession(definitions: new[] { DecisionCatalog.EnergyStation, second });
            Check(session.TryChoose(DecisionCatalog.EnergyStationId, DecisionCatalog.Conservation, out _));
            Check(session.TryChoose(second.Id, "save", out _));
            Check(session.CompletedCount == 2 && session.State.Budget == 95 && session.State.Water == 52);
        });
        Run("Resource bounds", () =>
        {
            CheckState(new ResourceState(-1, -2, -3, -4, -5), 0, 0, 0, 0, 0);
            CheckState(new ResourceState(int.MaxValue, 101, 102, 103, 104), 999, 100, 100, 100, 100);
        });
        Run("Water exhaustion and plant floor", () =>
        {
            var state = new SimulationRules().ApplyTurn(new ResourceState(100, 20, 1, 2, 0), Option(DecisionCatalog.Diesel));
            Check(state.Water == 0 && state.PlantHealth == 0);
        });
        Run("Recovery and resource ceilings", () =>
        {
            var state = new SimulationRules(0, 0).ApplyTurn(new ResourceState(100, 99, 100, 99, 99), Option(DecisionCatalog.Solar));
            Check(state.Energy == 100 && state.Water == 100 && state.PlantHealth == 100 && state.Emissions == 100);
        });
        Run("Water availability affects plant health", () =>
        {
            var rules = new SimulationRules();
            var wet = rules.ApplyTurn(new ResourceState(100, 20, 80, 50, 0), Option(DecisionCatalog.Solar));
            var dry = rules.ApplyTurn(new ResourceState(100, 20, 10, 50, 0), Option(DecisionCatalog.Solar));
            Check(wet.PlantHealth > dry.PlantHealth);
        });
        Run("External drought independently affects plants", () =>
        {
            var initial = ResourceState.Initial();
            var mild = new SimulationRules(0).ApplyTurn(initial, Option(DecisionCatalog.Solar));
            var dry = new SimulationRules(10).ApplyTurn(initial, Option(DecisionCatalog.Solar));
            Check(mild.PlantHealth - dry.PlantHealth == 10 && mild.Water == dry.Water);
        });
        Run("Emissions do not produce immediate heat or plant penalty", () =>
        {
            var initial = ResourceState.Initial();
            var rules = new SimulationRules();
            var clean = rules.ApplyTurn(initial, new DecisionOption("clean", "", "", 0, 0, 0, 0));
            var polluting = rules.ApplyTurn(initial, new DecisionOption("polluting", "", "", 0, 0, 0, 25));
            Check(clean.PlantHealth == polluting.PlantHealth && clean.Water == polluting.Water);
            Check(polluting.Emissions == 25 && clean.Emissions == 0);
        });
        Run("Energy constrains future pump capacity and demand", () =>
        {
            var rules = new SimulationRules();
            Check(rules.PumpCapacity(0, 50) == 0);
            Check(rules.PumpCapacity(10, 50) == 20);
            Check(rules.PumpCapacity(25, 50) == 50);
            Check(rules.PumpCapacity(100, 7) == 7);
        });
        Run("Independent sessions reset state and completion", () =>
        {
            var first = new GameSession();
            Check(first.TryChoose(DecisionCatalog.EnergyStationId, DecisionCatalog.Diesel, out _));
            var restarted = new GameSession();
            CheckState(restarted.State, 100, 20, 40, 55, 0);
            Check(restarted.CompletedCount == 0 && restarted.GetChoice(DecisionCatalog.EnergyStationId) == null);
            Check(first.CompletedCount == 1);
        });
        Run("Definition copies caller array", () =>
        {
            var original = Option(DecisionCatalog.Solar);
            var array = new[] { original };
            var point = new DecisionPointDefinition("copy-test", array);
            array[0] = Option(DecisionCatalog.Diesel);
            Check(ReferenceEquals(point.Options[0], original));
        });
        Run("Invalid configuration is rejected", () =>
        {
            Throws<ArgumentOutOfRangeException>(() => new SimulationRules(-1));
            Throws<ArgumentOutOfRangeException>(() => new SimulationRules(0, 101));
            Throws<ArgumentOutOfRangeException>(() => new SimulationRules().PumpCapacity(-1, 5));
            Throws<ArgumentOutOfRangeException>(() => new SimulationRules().PumpCapacity(1, -1));
            Throws<ArgumentOutOfRangeException>(() => new DecisionOption("bad", "", "", -1, 0, 0, 0));
            Throws<ArgumentOutOfRangeException>(() => new DecisionOption("bad", "", "", 0, int.MinValue, 0, 0));
            Throws<ArgumentException>(() => new DecisionPointDefinition("bad", Option(DecisionCatalog.Solar), Option(DecisionCatalog.Solar)));
            Throws<ArgumentException>(() => new GameSession(definitions: new[] { DecisionCatalog.EnergyStation, DecisionCatalog.EnergyStation }));
        });
        Run("Direct rule call rejects insufficient budget", () =>
            Throws<InvalidOperationException>(() => new SimulationRules().ApplyTurn(new ResourceState(0, 20, 40, 55, 0), Option(DecisionCatalog.Solar))));
        Run("Balance matrix preserves invariants", () =>
        {
            foreach (var option in DecisionCatalog.EnergyStation.Options)
            foreach (int budget in new[] { 0, 9, 10, 24, 25, 59, 60, 100 })
            foreach (int water in new[] { 0, 1, 34, 35, 43, 100 })
            foreach (int energy in new[] { 0, 20, 100 })
            {
                var session = new GameSession(new ResourceState(budget, energy, water, 55, 90));
                var original = session.State;
                bool accepted = session.TryChoose(DecisionCatalog.EnergyStationId, option.Id, out _);
                Check(accepted == (budget >= option.Cost));
                if (!accepted) { Check(ReferenceEquals(original, session.State)); continue; }
                var state = session.State;
                Check(state.Budget == budget - option.Cost);
                Check(state.Energy >= 0 && state.Energy <= 100 && state.Water >= 0 && state.Water <= 100);
                Check(state.PlantHealth >= 0 && state.PlantHealth <= 100 && state.Emissions >= 0 && state.Emissions <= 100);
                Check(original.Budget == budget && original.Water == water);
                Check(!session.TryChoose(DecisionCatalog.EnergyStationId, option.Id, out _));
            }
        });
        Run("Decision order is enforced atomically", () =>
        {
            var session = new GameSession();
            var original = session.State;
            Check(!session.TryChoose(DecisionCatalog.WaterStationId, DecisionCatalog.Continue, out _));
            Check(!session.TryChoose(DecisionCatalog.FarmStationId, DecisionCatalog.Store, out _));
            Check(ReferenceEquals(original, session.State) && session.History.Count == 0);
            Check(session.CurrentPoint.Id == DecisionCatalog.EnergyStationId && session.Ending == null);
        });
        Run("Pump output and running cost depend on energy", () =>
        {
            var high = new GameSession();
            var low = new GameSession();
            Select(high, DecisionCatalog.Solar);
            Select(low, DecisionCatalog.Conservation);
            var a = high.GetPreview(DecisionCatalog.WaterStationId, DecisionCatalog.Pump);
            var b = low.GetPreview(DecisionCatalog.WaterStationId, DecisionCatalog.Pump);
            Check(a.Cost == 25 && b.Cost == 30);
            Check(a.After.Water == 74 && b.After.Water == 62);
            Check(a.After.Water - a.Before.Water > b.After.Water - b.Before.Water);
        });
        Run("Insufficient energy blocks pumping without consuming resources", () =>
        {
            var session = new GameSession(new ResourceState(100, 0, 40, 55, 0));
            Select(session, DecisionCatalog.Conservation);
            var original = session.State;
            Check(!session.TryChoose(DecisionCatalog.WaterStationId, DecisionCatalog.Pump, out var reason));
            Check(reason.Contains("Enerji") && ReferenceEquals(original, session.State));
        });
        Run("Water investment budget shortage and recovery", () =>
        {
            var session = new GameSession(new ResourceState(30, 20, 40, 55, 0));
            Select(session, DecisionCatalog.Diesel);
            var original = session.State;
            Check(!session.TryChoose(DecisionCatalog.WaterStationId, DecisionCatalog.Pump, out var reason));
            Check(reason.Contains("Bütçe") && ReferenceEquals(original, session.State));
            Select(session, DecisionCatalog.Continue);
            Select(session, DecisionCatalog.Store);
            Check(session.IsComplete);
        });
        Run("Irrigation requires available water", () =>
        {
            var session = new GameSession();
            Select(session, DecisionCatalog.Solar);
            Select(session, DecisionCatalog.Continue);
            var original = session.State;
            Check(!session.TryChoose(DecisionCatalog.FarmStationId, DecisionCatalog.Irrigate, out var reason));
            Check(reason.Contains("Su yetersiz") && ReferenceEquals(original, session.State));
            Select(session, DecisionCatalog.Store);
            Check(session.IsComplete);
        });
        Run("Water efficiency persists into the farming turn", () =>
        {
            var session = new GameSession();
            Select(session, DecisionCatalog.Solar);
            Select(session, DecisionCatalog.Efficiency);
            Check(session.State.Water == 46);
            Select(session, DecisionCatalog.Protect);
            CheckState(session.State, 20, 55, 42, 59, 0);
        });
        Run("Nature support scales with remaining water", () =>
        {
            var rules = new SimulationRules();
            var protect = DecisionCatalog.FarmStation.Options[0];
            var wet = rules.Preview(new ResourceState(100, 30, 80, 40, 0), protect);
            var dry = rules.Preview(new ResourceState(100, 30, 10, 40, 0), protect);
            Check(wet.After.PlantHealth == 60 && dry.After.PlantHealth == 31);
        });
        Run("Zero resources still allow a full playthrough", () =>
        {
            var session = new GameSession(new ResourceState(0, 0, 0, 0, 0));
            Select(session, DecisionCatalog.Conservation);
            Select(session, DecisionCatalog.Continue);
            Select(session, DecisionCatalog.Store);
            Check(session.IsComplete && session.Ending.Category == EndingCategory.LostFuture);
        });
        Run("Previews do not mutate or consume turns", () =>
        {
            var session = new GameSession();
            var original = session.State;
            for (int i = 0; i < 10; i++)
                foreach (var option in session.CurrentPoint.Options) session.GetPreview(session.CurrentPoint.Id, option.Id);
            Check(ReferenceEquals(original, session.State) && session.History.Count == 0 && session.CompletedCount == 0);
        });
        Run("Preview exactly matches each applied outcome", () =>
        {
            var session = new GameSession();
            foreach (string option in new[] { DecisionCatalog.Solar, DecisionCatalog.Efficiency, DecisionCatalog.Protect })
            {
                var preview = session.GetPreview(session.CurrentPoint.Id, option);
                Select(session, option);
                Check(ReferenceEquals(preview.Before, session.History[session.History.Count - 1].Result.Before));
                CheckState(session.State, preview.After.Budget, preview.After.Energy, preview.After.Water, preview.After.PlantHealth, preview.After.Emissions);
            }
        });
        Run("Rebirth ending", () => Check(Play(DecisionCatalog.Solar, DecisionCatalog.Efficiency, DecisionCatalog.Protect).Ending.Category == EndingCategory.Rebirth));
        Run("Partial transition ending", () => Check(Play(DecisionCatalog.Diesel, DecisionCatalog.Pump, DecisionCatalog.Irrigate).Ending.Category == EndingCategory.PartialTransition));
        Run("Lost future ending", () => Check(Play(DecisionCatalog.Solar, DecisionCatalog.Continue, DecisionCatalog.Store).Ending.Category == EndingCategory.LostFuture));
        Run("Ending considers energy emissions and resource buffer", () =>
        {
            Check(EndingResult.Evaluate(new ResourceState(10, 15, 35, 55, 12)).Category == EndingCategory.Rebirth);
            Check(EndingResult.Evaluate(new ResourceState(10, 14, 35, 55, 12)).Category == EndingCategory.PartialTransition);
            Check(EndingResult.Evaluate(new ResourceState(10, 15, 35, 55, 13)).Category == EndingCategory.PartialTransition);
            Check(EndingResult.Evaluate(new ResourceState(0, 15, 35, 55, 12)).Category == EndingCategory.PartialTransition);
            Check(EndingResult.Evaluate(new ResourceState(0, 15, 50, 55, 12)).Category == EndingCategory.Rebirth);
        });
        Run("Completion and history survive rejected fourth decisions", () =>
        {
            var session = Play(DecisionCatalog.Solar, DecisionCatalog.Efficiency, DecisionCatalog.Protect);
            var original = session.State;
            var ending = session.Ending;
            Check(session.IsComplete && session.CurrentPoint == null && session.History.Count == 3);
            Check(!session.TryChoose(DecisionCatalog.FarmStationId, DecisionCatalog.Store, out _));
            Check(ReferenceEquals(original, session.State) && ReferenceEquals(ending, session.Ending));
            Check(session.History[0].Option.Id == DecisionCatalog.Solar && session.History[2].Option.Id == DecisionCatalog.Protect);
        });
        Run("Restart after full ending has no leaked choices", () =>
        {
            var completed = Play(DecisionCatalog.Diesel, DecisionCatalog.Pump, DecisionCatalog.Store);
            var fresh = new GameSession();
            Check(completed.IsComplete && !fresh.IsComplete && fresh.Ending == null && fresh.History.Count == 0);
            CheckState(fresh.State, 100, 20, 40, 55, 0);
        });
        Run("Empty decision registry is rejected", () => Throws<ArgumentException>(() => new GameSession(definitions: Array.Empty<DecisionPointDefinition>())));
        Run("All 27 base sequences and no softlocks", () => TestMatrix(args));
        Console.WriteLine($"{passed} passed; {failed} failed.");
        return failed == 0 ? 0 : 1;
    }


    private static void Select(GameSession session, string option)
    {
        Check(session.CurrentPoint != null, "No current point");
        Check(session.TryChoose(session.CurrentPoint.Id, option, out var reason), reason);
    }

    private static GameSession Play(params string[] options)
    {
        var session = new GameSession();
        foreach (var option in options) Select(session, option);
        return session;
    }

    private static void CheckHasOption(GameSession session)
    {
        foreach (var option in session.CurrentPoint.Options)
            if (session.GetPreview(session.CurrentPoint.Id, option.Id).CanChoose) return;
        throw new Exception("Softlock at " + session.CurrentPoint.Id);
    }

    private static void TestMatrix(string[] args)
    {
        int completed = 0, blocked = 0;
        var endings = new HashSet<EndingCategory>();
        var report = new StringBuilder("# 27 karar dizisi — saf C# test raporu\n\nUnity oynanış testi değildir. Değer sırası: bütçe / enerji / su / bitki / emisyon.\n\n| Enerji | Su | Tarım | Sonuç | Son değerler |\n| --- | --- | --- | --- | --- |\n");
        foreach (var energy in DecisionCatalog.EnergyStation.Options)
        foreach (var water in DecisionCatalog.WaterStation.Options)
        foreach (var farm in DecisionCatalog.FarmStation.Options)
        {
            var session = new GameSession();
            CheckHasOption(session);
            Select(session, energy.Id);
            CheckHasOption(session);
            Select(session, water.Id);
            CheckHasOption(session);
            string point = session.CurrentPoint.Id;
            var preview = session.GetPreview(point, farm.Id);
            if (!preview.CanChoose)
            {
                Check(farm.Id == DecisionCatalog.Irrigate && water.Id == DecisionCatalog.Continue && energy.Id != DecisionCatalog.Conservation);
                var original = session.State;
                Check(!session.TryChoose(point, farm.Id, out _) && ReferenceEquals(original, session.State));
                blocked++;
                Select(session, DecisionCatalog.Store);
                Check(session.IsComplete);
                report.AppendLine($"| {energy.Id} | {water.Id} | {farm.Id} | Engelli: {preview.Reason} Depolama ile final mümkün. | — |");
                continue;
            }
            Select(session, farm.Id);
            Check(session.IsComplete && session.CompletedCount == 3 && session.Ending != null);
            var state = session.State;
            Check(state.Budget >= 0 && state.Energy >= 0 && state.Energy <= 100 && state.Water >= 0 && state.Water <= 100 && state.PlantHealth >= 0 && state.PlantHealth <= 100);
            Check(!session.TryChoose(point, farm.Id, out _));
            endings.Add(session.Ending.Category);
            completed++;
            report.AppendLine($"| {energy.Id} | {water.Id} | {farm.Id} | {session.Ending.Title} | {state.Budget} / {state.Energy} / {state.Water} / {state.PlantHealth} / {state.Emissions} |");
        }
        Check(completed == 25 && blocked == 2 && endings.Count == 3, $"Matrix: {completed} completed, {blocked} blocked, {endings.Count} endings");
        report.AppendLine("\n25 dizi doğrudan finale ulaştı. Su yetersizliği nedeniyle engellenen 2 sulama dizisinde ücretsiz depolama seçilerek ilerleme doğrulandı. Üç final kategorisine de ulaşıldı.");
        if (args.Length == 2 && args[0] == "--report") File.WriteAllText(args[1], report.ToString());
        Console.WriteLine("MATRIX 27 checked: 25 completed, 2 blocked with a valid fallback; all 3 endings reached.");
    }

    private static ResourceState Choose(string id)
    {
        var session = new GameSession();
        Check(session.TryChoose(DecisionCatalog.EnergyStationId, id, out _));
        return session.State;
    }

    private static DecisionOption Option(string id)
    {
        foreach (var option in DecisionCatalog.EnergyStation.Options) if (option.Id == id) return option;
        throw new Exception("Missing test option: " + id);
    }

    private static void CheckState(ResourceState state, int budget, int energy, int water, int plants, int emissions)
    {
        Check(state.Budget == budget && state.Energy == energy && state.Water == water &&
            state.PlantHealth == plants && state.Emissions == emissions,
            $"Unexpected state: {state.Budget}/{state.Energy}/{state.Water}/{state.PlantHealth}/{state.Emissions}");
    }

    private static void Check(bool condition, string message = "Assertion failed")
    {
        if (!condition) throw new Exception(message);
    }

    private static void Throws<T>(Action action) where T : Exception
    {
        try { action(); }
        catch (T) { return; }
        throw new Exception("Expected " + typeof(T).Name);
    }

    private static void Run(string name, Action test)
    {
        try { test(); passed++; Console.WriteLine("PASS " + name); }
        catch (Exception exception) { failed++; Console.Error.WriteLine("FAIL " + name + ": " + exception.Message); }
    }
}
