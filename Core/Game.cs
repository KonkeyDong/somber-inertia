namespace SomberInertia.Core;

public class Game
{
    public Grid Grid { get; set; }
    public List<Unit> Units { get; set; } = new List<Unit>();
    public List<Unit> FriendlyUnitsInRange { get; set; } = new List<Unit>();
    public List<Unit> UnfriendlyUnitsInRange { get; set; } = new List<Unit>();

    public Game(Grid grid)
    {
        Grid = grid;
    }

    public Unit GetCurrentUnit()
    {
        if (Units.Count == 0)
        {
            Logger.Error("Game::GetCurrentUnit(): list of units is empty! Aborting...");
            throw new IndexOutOfRangeException("Game::GetCurrentUnit(): trying to index empty list at Units.");
        }

        return Units[0];
    }

    public void MoveFirstUnitToEndOfList()
    {
        if (Units.Count <= 1)
        {
            Logger.Error("Game::MoveFirstUnitToEndOfList(): Units list is empty.");
            throw new InvalidOperationException("units list is empty.");
        }
        
        var first = Units[0];
        Units.RemoveAt(0);
        Units.Add(first);
    }

    public void AddUnit(Unit unit, int x, int y)
    {
        if (unit == null)
        {
            Logger.Error("Game::Unit(): unit parameter is null; aborting.");
            throw new ArgumentNullException("unit cannot be null when adding");
        }

        if (x < 0 || x >= Grid.Width || y < 0 || y >= Grid.Height)
        {
            throw new ArgumentOutOfRangeException($"Target position ({x}, {y}) is outside grid bounds.");
        }

        Units.Add(unit);
        Grid.PlaceUnit(unit, x, y);
    }

    public void ResetListOfUnitsInRange()
    {
        FriendlyUnitsInRange.Clear();
        UnfriendlyUnitsInRange.Clear();
    }

    public void SeparateListOfUnitsInRange(Unit currentUnit, List<Unit> unitsInRange)
    {
        Logger.Debug("Game::SeparateListOfUnitsInRange(): resetting FriendlyUnitsInRange and UnfriendlyUnitsInRange lists.");
        ResetListOfUnitsInRange();

        foreach (var unitInRange in unitsInRange)
        {
            if (currentUnit.Friendly == unitInRange.Friendly)
            {
                FriendlyUnitsInRange.Add(unitInRange);
            }
            else
            {
                UnfriendlyUnitsInRange.Add(unitInRange);
            }
        }

        Logger.Info($"FriendlyUnitsInRange.Count = {FriendlyUnitsInRange.Count()}; UnfriendlyUnitsInRange.Count = {UnfriendlyUnitsInRange.Count()}.");
    }

    public List<Unit> RemoveDeadUnits()
    {
        Logger.Debug("Game::RemoveDeadUnits(): removing all units that have 0 (current) HP.");
        var deadUnits = Units.FindAll(unit => unit.HP.Current == 0);
        var count = Units.RemoveAll(unit => unit.HP.Current == 0);

        Logger.Info($"Number of dead units removed: [{count}].");

        return deadUnits;
    }
}