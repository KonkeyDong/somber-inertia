using Raylib_cs;

using SomberInertia.Core;

namespace SomberInertia.State;

public class BattleActionMenu : IGameState
{
    private Game _game { get; set; }
    private Unit _currentUnit { get; set; }

    public BattleActionMenu(Game game)
    {
        _game = game;
        _currentUnit = _game.GetCurrentUnit();
    }

    public void Enter()
    {
        _game.Grid.RangeTint.Reset();
    }
    
    public void Exit()
    {
        
    }

    public void HandleInput()
    {
        // Arrow keys
        if (Raylib.IsKeyPressed(KeyboardKey.Up))
            _game.Grid.MoveUnitInDirection(_currentUnit, Direction.Up);

        if (Raylib.IsKeyPressed(KeyboardKey.Down))
            _game.Grid.MoveUnitInDirection(_currentUnit, Direction.Down);

        if (Raylib.IsKeyPressed(KeyboardKey.Left))
            _game.Grid.MoveUnitInDirection(_currentUnit, Direction.Left);

        if (Raylib.IsKeyPressed(KeyboardKey.Right))
            _game.Grid.MoveUnitInDirection(_currentUnit, Direction.Right);
    }

    public void Update()
    {
        _game.Grid.RangeTint.Tick();
    }

    public void Draw(float scale)
    {
        _game.Grid.DrawBackground(scale);
        _game.Grid.DrawWeaponAttackRange(scale);
        _game.Grid.DrawUnits(_game.Units, scale);
    }
}