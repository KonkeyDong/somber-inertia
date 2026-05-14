namespace SomberInertia.Core;

public class Game
{
    public Grid Grid { get; set; }
    public List<Unit> Units { get; set; } = new List<Unit>();

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
}