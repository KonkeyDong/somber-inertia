using SomberInertia.Core;
using SomberInertia.Enums;

namespace SomberInertia.State;

// This state is a way to reset the state of all of the game, grid, and unit objects.
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
        _game.AttackContext.Reset();

        // If a unit begins their turn dying from poison, the AnimateUnitDeath
        // state will remove that unit from the Game.Units list and then switch
        // the state to EndTurn. Since the unit was removed from the list, the
        // current unit selected is actually the unit after the unit that just
        // died from poison. Therefore, we skip moving that unit to the end of
        // the list to avoid skipping them. Their turn will begin after as
        // after this state exits.
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