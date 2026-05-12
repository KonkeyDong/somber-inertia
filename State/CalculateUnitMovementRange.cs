using SomberInertia.Core;
using SomberInertia.Enums;

namespace SomberInertia.State;

public class CalculateUnitMovementRange : IGameState
{
    private Grid _grid { get; set; }
    private Unit _currentUnit { get; set; }

    public CalculateUnitMovementRange(Grid grid)
    {
        _grid = grid;

        if (_grid.Units.Count == 0)
        {
            Logger.Error("CalculateUnitMovementRange(): grid has no units! Aborting...");
            throw new IndexOutOfRangeException("CalculateUnitMovementRange(): trying to index empty list at _grid.Units.");
        }

        _currentUnit = _grid.Units[0];
    }

    public void Enter()
    {
        Logger.Debug("CalculateUnitMovementRange::Enter(): called.");
        _grid.CalculateUnitMovementRange(_currentUnit);
        GameStateManager.ChangeStateType(GameStateType.UnitMoving);
    }

    public void Exit()
    {
        Logger.Debug("CalculateUnitMovementRange::Enter(): called.");
    }

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