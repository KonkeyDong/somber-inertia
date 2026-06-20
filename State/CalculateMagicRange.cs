using SomberInertia.Core;
using SomberInertia.Enums;
using SomberInertia.State;
using SomberInertia.Core.Units;

namespace SomberInertia.State;

public class CalculateMagicRange : IGameState
{
    private Game _game { get; set; }
    private Unit _currentUnit { get; set; }

    public CalculateMagicRange(Game game)
    {
        _game = game;
        _currentUnit = _game.GetCurrentUnit();
    }

    public void Enter()
    {
        // _game.Grid.CalculateMagicAttackRange(_currentUnit);
        var unitsInRange = _game.Grid.BuildListOfUnitsInMagicRange(_currentUnit);
        _game.SeparateListOfUnitsInRange(_currentUnit, unitsInRange);

        if (_game.MagicUI.IsSelectedMagicOffensive())
        {
            Logger.Info("Spell is offensive-type.");
            if (_game.UnfriendlyUnitsInRange.Count > 0)
            {
                // GameStateManager.ChangeStateType(GameStateType.SelectEnemyForPhysicalAttack);
                Logger.Warning("Select unit for magic attack not implemented.");
                GameStateManager.ChangeStateType(GameStateType.SelectMagicLevel);
            }
            else 
            {
                GameStateManager.ChangeStateType(GameStateType.NoMagicTargetAvailable);
            }
        }
        else 
        {
            Logger.Info("Spell is defensive-type.");
            if (_game.FriendlyUnitsInRange.Count > 0)
            {
                Logger.Warning("Select unit for magic attack not implemented.");
                GameStateManager.ChangeStateType(GameStateType.SelectMagicLevel);
            }
            else 
            {
                GameStateManager.ChangeStateType(GameStateType.NoMagicTargetAvailable);
            }
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