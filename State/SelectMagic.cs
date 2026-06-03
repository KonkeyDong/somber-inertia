using SomberInertia.Enums;
using SomberInertia.Core;

namespace SomberInertia.State;

public class SelectMagic : IGameState
{
    private readonly Game _game;

    public SelectMagic(Game game)
    {
        _game = game;
    }

    public void Enter()
    {
        Logger.Warning("SelectMagic::Enter(): state hasn't been fully implemented; changing state to EndTurn.");
        GameStateManager.ChangeStateType(GameStateType.EndTurn);
    }

    public void Exit()
    {

    }

    public void HandleInput()
    {

    }

    public void Update()
    {

    }

    public void Draw(float scale)
    {

    }
}