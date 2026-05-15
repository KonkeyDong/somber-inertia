using SomberInertia.State;
using SomberInertia.Enums;
using SomberInertia.Core;

namespace SomberInertia.State;

public class CalculateWeaponAttackRange : IGameState
{
    private Game _game { get; set; }
    private Unit _currentUnit { get; set; }

    public CalculateWeaponAttackRange(Game game)
    {
        _game = game;
        _currentUnit = _game.GetCurrentUnit();
    }

    public void Enter()
    {
        Logger.Debug("CalculateWeaponAttackRange::Enter(): called.");
        // _game.Grid.ResetAttackRangeCosts();
        _game.Grid.CalculateWeaponAttackRange(_currentUnit);
        GameStateManager.ChangeStateType(GameStateType.BattleActionMenu);
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