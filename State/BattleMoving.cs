using SomberInertia.Core;
using Raylib_cs;

namespace SomberInertia.State;

public class BattleMoving : IGameState
{
    private Grid _grid { get; set; }
    private Unit _currentUnit { get; set; }

    public BattleMoving(Grid grid)
    {
        _grid = grid;

        if (_grid.Units.Count == 0)
        {
            Logger.Error("BattleMoving(): grid has no units! Aborting...");
            throw new IndexOutOfRangeException("BattleMoving(): trying to index empty list at _grid.Units.");
        }

        _currentUnit = _grid.Units[0];
    }

    public void Enter()
    {
        Logger.Debug("BattleMoving::Enter() called.");
        _grid.MovementRangeTint.Reset();
        _grid.CalculateUnitMovementRange(_currentUnit);
    }

    public void Exit()
    {
        Logger.Debug("BattleMoving::Exit() called.");
    }

    public void HandleInput()
    {
        // Arrow keys
        if (Raylib.IsKeyPressed(KeyboardKey.Up))
            _grid.MoveUnitInDirection(_currentUnit, Direction.Up);

        if (Raylib.IsKeyPressed(KeyboardKey.Down))
            _grid.MoveUnitInDirection(_currentUnit, Direction.Down);

        if (Raylib.IsKeyPressed(KeyboardKey.Left))
            _grid.MoveUnitInDirection(_currentUnit, Direction.Left);

        if (Raylib.IsKeyPressed(KeyboardKey.Right))
            _grid.MoveUnitInDirection(_currentUnit, Direction.Right);

        // if (Raylib.IsKeyPressed(KeyboardKey.Z))
        //     _grid.
    }

    public void Update()
    {
        _grid.MovementRangeTint.Tick();
    }

    public void Draw(float scale)
    {
        _grid.DrawBackground(scale);
        _grid.DrawMovementRange(scale);
        _grid.DrawUnits(scale);
    }
}