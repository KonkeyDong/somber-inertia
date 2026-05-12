using Raylib_cs;

using SomberInertia.Core;

namespace SomberInertia.State;

public class BattleActionMenu : IGameState
{
    private Grid _grid { get; set; }
    private Unit _currentUnit { get; set; }

    public BattleActionMenu(Grid grid)
    {
        _grid = grid;

        if (_grid.Units.Count == 0)
        {
            Logger.Error("BattleActionMenu(): grid has no units! Aborting...");
            throw new IndexOutOfRangeException("BattleActionMenu(): trying to index empty list at _grid.Units.");
        }

        _currentUnit = _grid.Units[0];
    }

    public void Enter()
    {
        _grid.RangeTint.Reset();
    }
    
    public void Exit()
    {
        
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
    }

    public void Update()
    {
        _grid.RangeTint.Tick();
    }

    public void Draw(float scale)
    {
        _grid.DrawBackground(scale);
        _grid.DrawWeaponAttackRange(scale);
        _grid.DrawUnits(scale);
    }
}