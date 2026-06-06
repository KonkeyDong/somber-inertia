using SomberInertia.Enums;
using SomberInertia.Core;
using SomberInertia.Core.Units;

namespace SomberInertia.State;

public class SelectMagic : IGameState
{
    private readonly Game _game;
    private readonly Unit _currentUnit;

    private static readonly Dictionary<Direction, int> _spellIndexByDirection = new()
    {
        { Direction.Up,    0 },   // First spell in the list
        { Direction.Left,  1 },
        { Direction.Right, 2 },
        { Direction.Down,  3 }
    };

    public SelectMagic(Game game)
    {
        _game = game;
        _currentUnit = _game.GetCurrentUnit();
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