using SomberInertia.Core;
using SomberInertia.Enums;
using SomberInertia.State;
using SomberInertia.Core.Units;

namespace SomberInertia.State;

public class CalculateWeaponAttackRange : IGameState
{
    private readonly Game _game;
    private Unit _currentUnit;

    public CalculateWeaponAttackRange(Game game)
    {
        _game = game;
        _currentUnit = _game.GetCurrentUnit();
    }

    public void Enter()
    {
        Logger.Debug("CalculateWeaponAttackRange::Enter(): called.");

        _game.Grid.CalculateWeaponAttackRange(_currentUnit);
        var unitsInRange = _game.Grid.BuildListOfUnitsInAttackRange(_currentUnit);
        _game.SeparateListOfUnitsInRange(_currentUnit, unitsInRange);

        if (_game.UnfriendlyUnitsInRange.Count > 0)
        {
            GameStateManager.ChangeStateType(GameStateType.SelectEnemyForPhysicalAttack);
        }
        else 
        {
            GameStateManager.ChangeStateType(GameStateType.NoAttackTargetAvailable);
        }
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