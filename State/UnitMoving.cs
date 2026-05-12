using SomberInertia.Core;
using SomberInertia.State;
using SomberInertia.Enums;
using Raylib_cs;

namespace SomberInertia.State;

public class UnitMoving : IGameState
{
    private Grid _grid { get; set; }
    private Unit _currentUnit { get; set; }

    public UnitMoving(Grid grid)
    {
        _grid = grid;

        if (_grid.Units.Count == 0)
        {
            Logger.Error("UnitMoving(): grid has no units! Aborting...");
            throw new IndexOutOfRangeException("UnitMoving(): trying to index empty list at _grid.Units.");
        }

        _currentUnit = _grid.Units[0];
    }

    public void Enter()
    {
        Logger.Debug("UnitMoving::Enter() called.");
        _grid.RangeTint.Reset();
        _grid.CalculateUnitMovementRange(_currentUnit);
    }

    public void Exit()
    {
        Logger.Debug("UnitMoving::Exit() called.");
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

        if (Raylib.IsKeyPressed(KeyboardKey.Z))
            GameStateManager.ChangeStateType(GameStateType.CalculateWeaponAttackRange);
    }

    public void Update()
    {
        _grid.RangeTint.Tick();
    }

    public void Draw(float scale)
    {
        _grid.DrawBackground(scale);
        _grid.DrawMovementRange(scale);
        _grid.DrawUnits(scale);
    }
}