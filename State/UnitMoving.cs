using SomberInertia.Core;
using SomberInertia.State;
using SomberInertia.Timers;
using SomberInertia.Enums;
using System.Numerics;

using Raylib_cs;

namespace SomberInertia.State;

public class UnitMoving : IGameState
{
    private Game _game { get; set; }
    private Unit _currentUnit { get; set; }
    private CountdownTimer _countdownTimer { get; set; }

    public UnitMoving(Game game)
    {
        _game = game;
        _currentUnit = _game.GetCurrentUnit();
        _countdownTimer = new CountdownTimer(60); // 60 frames / 1 second
    }

    public void Enter()
    {
        Logger.Debug("UnitMoving::Enter() called.");
        _game.Grid.RangeTint.Reset();
        _game.InitializeHighlight();
    }

    public void Exit() => Logger.Debug("UnitMoving::Exit() called.");

    public void HandleInput()
    {
        // Arrow keys
        if (Raylib.IsKeyPressed(KeyboardKey.Up)) { _game.Grid.MoveUnitInDirection(_currentUnit, Direction.Up); }
        if (Raylib.IsKeyPressed(KeyboardKey.Down)) { _game.Grid.MoveUnitInDirection(_currentUnit, Direction.Down); }
        if (Raylib.IsKeyPressed(KeyboardKey.Left)) { _game.Grid.MoveUnitInDirection(_currentUnit, Direction.Left); }
        if (Raylib.IsKeyPressed(KeyboardKey.Right)) { _game.Grid.MoveUnitInDirection(_currentUnit, Direction.Right); }

        if (Raylib.IsKeyPressed(KeyboardKey.Z) || Raylib.IsKeyPressed(KeyboardKey.C)) { GameStateManager.ChangeStateType(GameStateType.CalculateWeaponAttackRange); }
    }

    public void Update()
    {
        _game.Grid.RangeTint.Tick();
        _countdownTimer.Tick();
    }

    public void Draw(float scale)
    {
        _game.Grid.DrawBackground(scale);
        _game.Grid.DrawMovementRange(scale);

        if (_countdownTimer.GetIsActive())
        {
            _game.Grid.DrawHighlightRectangle(scale, _game.GetHighlightPosition());
        }

        _game.Grid.DrawUnits(_game.Units, scale);
    }
}