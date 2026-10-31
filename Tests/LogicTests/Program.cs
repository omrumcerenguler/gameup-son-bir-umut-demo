using System;
using SonBirUmutDemo.Decisions;
using SonBirUmutDemo.Simulation;

internal static class Program
{
    private static int passed;
    private static int failed;

    private static int Main()
    {
        Run("Initial state", () => CheckState(ResourceState.Initial(), 100, 20, 40, 55, 0));
        Run("Solar outcome", () => CheckState(Choose(DecisionCatalog.Solar), 40, 55, 32, 41, 1));
        Run("Diesel outcome", () => CheckState(Choose(DecisionCatalog.Diesel), 75, 70, 29, 41, 18));
        Run("Conservation outcome", () => CheckState(Choose(DecisionCatalog.Conservation), 90, 15, 40, 53, 0));
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
            Check(session.CompletedCount == 2 && session.State.Budget == 85 && session.State.Water == 52);
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
        Console.WriteLine($"{passed} passed; {failed} failed.");
        return failed == 0 ? 0 : 1;
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
