using SomberInertia.Core;
using SomberInertia.Enums;

namespace SomberInertia.State;

public class CalculateUnitMovementRange : IGameState
{
    private Game _game { get; set; }
    private Unit _currentUnit { get; set; }

    public CalculateUnitMovementRange(Game game)
    {
        _game = game;
        _currentUnit = _game.GetCurrentUnit();
    }

    public void Enter()
    {
        Logger.Debug("CalculateUnitMovementRange::Enter(): called.");
        _game.Grid.CalculateUnitMovementRange(_currentUnit);
        GameStateManager.ChangeStateType(GameStateType.UnitMoving);
    }

    public void Exit() => Logger.Debug("CalculateUnitMovementRange::Enter(): called.");

    // no op
    public void HandleInput()
    {

    }

    public void Update()
    {

    }

    public void Draw(float _)
    {

    }
}