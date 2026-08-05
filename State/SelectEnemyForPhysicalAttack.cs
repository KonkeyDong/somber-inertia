using SomberInertia.Core;
using SomberInertia.Enums;
using SomberInertia.Core.Units;
using SomberInertia.Core.Combat;

namespace SomberInertia.State;

public class SelectEnemyForPhysicalAttack : IGameState
{
    private readonly Game _game;
    private Unit _currentUnit;
    private int _currentIndex;

    public SelectEnemyForPhysicalAttack(Game game)
    {
        _game = game;
        _currentUnit = _game.GetCurrentUnit();
    }

    public void Enter()
    {
        if (_game.UnfriendlyUnitsInRange.Count > 0)
        {
            _game.InitializeHighlight();
            _currentIndex = 0;

            _game.SetHighlightTarget(_game.UnfriendlyUnitsInRange[_currentIndex]);
        }
    }

    public void Exit()
    {

    }

    public void HandleInput()
    {
        if (Input.TryCycleIndex(ref _currentIndex, _game.UnfriendlyUnitsInRange.Count))
        {
            var newTarget = _game.UnfriendlyUnitsInRange[_currentIndex];
            if (newTarget.Block != null)
            {
                _game.SetHighlightTarget(newTarget);
            }
        }

        if (Input.IsConfirmPressed())
        {
            ConfirmSelection();
        }

        if (Input.IsCancelPressed())
        {
            CancelSelection();
        }
    }

    private void ConfirmSelection()
    {
        _game.AttackContext = new AttackContext(_currentUnit, _game.UnfriendlyUnitsInRange[_currentIndex]);
        GameStateManager.ChangeStateType(GameStateType.EnterBattleScreen);
    }

    private void CancelSelection()
    {
        GameStateManager.ChangeStateType(GameStateType.BattleActionMenu);
    }

    public void Update()
    {
        _game.Grid.RangeTint.Tick();
        _game.FlipFlop.Tick();

        _game.UpdateHighlightPosition();
    }

    public void Draw(float scale)
    {
        _game.Renderer.DrawBackground(scale, _game.Grid);
        _game.Renderer.DrawWeaponAttackRange(scale, _game.Grid);
        _game.Renderer.DrawUnits(scale, _game.Grid, _game.Units, _game.FlipFlop.IsOn);

        _game.Renderer.DrawHighlightRectangle(scale, _game.GetHighlightPosition());
    }
}