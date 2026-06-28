using SomberInertia.Core;
using SomberInertia.Enums;

namespace SomberInertia.State;

public class EndTurn : IGameState
{
    private readonly Game _game;

    public EndTurn(Game game)
    {
        _game = game;
    }

    public void Enter()
    {
        var current = _game.GetCurrentUnit();
        current.ResetFacingDirection();
        Logger.Info($"{current.Name}'s turn ends.");

        _game.Grid.ResetAllRangeSets();
        _game.FrameFlipper.Reset();
        _game.ResetListOfUnitsInRange();
        _game.MagicUI.Reset();

        if (!_game.FirstUnitDiedFromPoison)
        {
            _game.MoveFirstUnitToEndOfList();
        }

        _game.ResetFirstUnitDiedFromPoison();

        GameStateManager.ChangeStateType(GameStateType.CalculateUnitMovementRange);
    }

    public void Exit()
    {
        var current = _game.GetCurrentUnit();

        Logger.Info($"{current.Name}'s turn begins.");
    }

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