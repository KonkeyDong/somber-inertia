using SomberInertia.State;
using SomberInertia.Enums;
using SomberInertia.Core;

namespace SomberInertia.State;

public class CalculateWeaponAttackRange : IGameState
{
    private Grid _grid { get; set; }
    private Unit _currentUnit { get; set; }

    public CalculateWeaponAttackRange(Grid grid)
    {
        _grid = grid;

        if (_grid.Units.Count == 0)
        {
            Logger.Error("BattleActionMenu(): grid has no units! Aborting...");
            throw new IndexOutOfRangeException("BattleActionMenu(): trying to index empty list at _grid.Units.");
        }

        _currentUnit = _grid.Units[0];
    }

    public void Enter()
    {
        Logger.Debug("CalculateWeaponAttackRange::Enter(): called.");
        _grid.ResetAttackRangeCosts();
        _grid.CalculateWeaponAttackRange(_currentUnit);
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