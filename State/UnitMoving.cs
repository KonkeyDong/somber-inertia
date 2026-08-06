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
    private readonly Game _game;
    private Unit _currentUnit;
    private readonly CountdownTimer _countdownTimer;

    public UnitMoving(Game game)
    {
        _game = game;
        _currentUnit = _game.GetCurrentUnit();
        _countdownTimer = new CountdownTimer(GameConstants.Animations.CountdownTimerDelay);
    }

    public void Enter()
    {
        Logger.Debug("UnitMoving::Enter() called.");
        _currentUnit = _game.GetCurrentUnit();
        _game.Grid.RangeTint.Reset();
        _game.Grid.CalculateUnitMovementRange(_currentUnit);
        _game.InitializeHighlight();
        _currentUnit.ResetStartingWorldPosition();
    }

    public void Exit() => Logger.Debug("UnitMoving::Exit() called.");

    public void HandleInput()
    {
        if (_currentUnit.IsAnimating)
        {
            return;
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Up)) { _game.Grid.MoveUnitInDirection(_currentUnit, Direction.Up); }
        if (Raylib.IsKeyPressed(KeyboardKey.Down)) { _game.Grid.MoveUnitInDirection(_currentUnit, Direction.Down); }
        if (Raylib.IsKeyPressed(KeyboardKey.Left)) { _game.Grid.MoveUnitInDirection(_currentUnit, Direction.Left); }
        if (Raylib.IsKeyPressed(KeyboardKey.Right)) { _game.Grid.MoveUnitInDirection(_currentUnit, Direction.Right); }

        if (Input.IsConfirmPressed())
        {
            ConfirmSelection();
        }
    }

    private void ConfirmSelection()
    {
        if (_currentUnit.Block != null && !_currentUnit.Block.IsFullyOccupied())
        {
            GameStateManager.ChangeStateType(GameStateType.BattleActionMenu);
        }
    }

    public void Update()
    {
        _game.Grid.RangeTint.Tick();
        _countdownTimer.Tick();
        _game.FlipFlop.Tick();

        if (_currentUnit.IsAnimating)
        {
            _currentUnit.UpdateMovement(Raylib.GetFrameTime());
        }
    }

    public void Draw(float scale)
    {
        _game.Renderer.DrawBackground(scale, _game.Grid);
        _game.Renderer.DrawRange(scale, _game.Grid);

        if (_countdownTimer.IsActive)
        {
            _game.Renderer.DrawHighlightRectangle(scale, _game.GetHighlightPosition());
        }

        _game.Renderer.DrawUnits(scale, _game.Grid, _game.Units, _game.FlipFlop.IsOn);
    }
}