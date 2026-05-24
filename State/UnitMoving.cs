using System.Numerics;
using Raylib_cs;
using SomberInertia.Core;
using SomberInertia.Enums;
using SomberInertia.State;
using SomberInertia.Timers;
using SomberInertia.Core.Units;

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
        _game.FrameFlipper.Tick();
    }

    public void Draw(float scale)
    {
        _game.Renderer.DrawBackground(scale, _game.Grid);
        _game.Renderer.DrawMovementRange(scale, _game.Grid);

        if (_countdownTimer.GetIsActive())
        {
            _game.Renderer.DrawHighlightRectangle(scale, _game.GetHighlightPosition());
        }

        _game.Renderer.DrawUnits(scale, _game.Grid, _game.Units, _game.FrameFlipper.IsOn);
    }
}