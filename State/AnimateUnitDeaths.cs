using SomberInertia.Core.Units;
using SomberInertia.Graphics;
using SomberInertia.Core;
using SomberInertia.State;
using SomberInertia.Enums;

namespace SomberInertia.State;

public class AnimateUnitDeaths : IGameState
{
    private readonly Game _game;
    private List<Unit> _deadUnits = new();

    private readonly List<Direction> _directionCycle = new List<Direction>
    {
        Direction.Up, Direction.Right, Direction.Down, Direction.Left,
        Direction.Up, Direction.Right, Direction.Down, Direction.Left,
        Direction.Up, Direction.Right, Direction.Down, Direction.Left
    };

    private int _directionIndex = 0;
    private int _deathFrameIndex = 0;
    private int _frameDelay = 2;
    private int _currentDelay = 0;

    private bool _directionPhaseComplete = false;

    public AnimateUnitDeaths(Game game)
    {
        _game = game;
    }

    // This state only plays the death animation. It does not remove units
    // from the game. Unit removal and cleanup is handled in the EndTurn state.
    public void Enter()
    {
        _deadUnits = _game.FindAllDeadUnits();
        DeathSprites.Load();

        if (_deadUnits.Count == 0)
        {
            Logger.Info("No dead units found; exiting state.");
            GameStateManager.ChangeStateType(GameStateType.EndTurn);
            return;
        }

        _directionIndex = 0;
        _deathFrameIndex = 0;
        _directionPhaseComplete = false;
        _currentDelay = 0;
    }

    public void Update()
    {
        if (_currentDelay > 0)
        {
            _currentDelay--;
            return;
        }

        _currentDelay = _frameDelay;

        if (!_directionPhaseComplete)
        {
            // Advance direction cycle
            _directionIndex++;
            if (_directionIndex >= _directionCycle.Count)
            {
                _directionPhaseComplete = true;
                _deathFrameIndex = 0;
            }
        }
        else
        {
            // Advance death frames
            _deathFrameIndex++;
            if (_deathFrameIndex >= DeathSprites.Frames.Count)
            {
                GameStateManager.ChangeStateType(GameStateType.EndTurn);
            }
        }
    }

    public void Draw(float scale)
    {
        _game.Renderer.DrawBackground(scale, _game.Grid);

        // Draw all living units normally
        _game.Renderer.DrawUnits(scale, _game.Grid, _game.Units, _game.FrameFlipper.IsOn);

        // Draw dying units
        foreach (var unit in _deadUnits)
        {
            if (!_directionPhaseComplete)
            {
                // Phase 1: Cycle through directions
                var dir = _directionCycle[_directionIndex];
                var sprite = unit.GetFacingDirectionTexture(dir);   // you may need to add this method
                _game.Renderer.Draw(scale, sprite, unit.WorldPosition);
            }
            else
            {
                // Phase 2: Death animation frames
                var deathSprite = DeathSprites.Frames[_deathFrameIndex];
                _game.Renderer.Draw(scale, deathSprite, unit.WorldPosition);
            }
        }
    }

    public void Exit() { }
    public void HandleInput() { }
}