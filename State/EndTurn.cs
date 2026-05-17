using SomberInertia.Enums;
using SomberInertia.Core;

namespace SomberInertia.State;

public class EndTurn : IGameState
{
    private readonly Game _game;

    public EndTurn(Game game)
    {
        _game = game;

        var current = _game.GetCurrentUnit();
        Logger.Info($"{current.Name}'s turn ends.");
    }
    
    public void Enter()
    {
        Logger.Debug("EndTurn::Enter()");

        _game.MoveFirstUnitToEndOfList();
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