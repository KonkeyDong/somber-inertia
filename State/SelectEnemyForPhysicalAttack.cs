using System.Numerics;
using Raylib_cs;
using SomberInertia.Core;
using SomberInertia.Enums;
using SomberInertia.Core.Units;

namespace SomberInertia.State;

public class SelectEnemyForPhysicalAttack : IGameState
{
    private Game _game { get; set; }
    private Unit _currentUnit { get; set; }
    private int _currentIndex { get; set; }

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
        if (_game.UnfriendlyUnitsInRange.Count() > 1)
        {
            var changed = false;
            if (Raylib.IsKeyPressed(KeyboardKey.Left))
            {

                _currentIndex = (_currentIndex + 1) % _game.UnfriendlyUnitsInRange.Count();
                changed = true;
            }
            if (Raylib.IsKeyPressed(KeyboardKey.Right))
            {
                _currentIndex = (_currentIndex - 1 + _game.UnfriendlyUnitsInRange.Count()) % _game.UnfriendlyUnitsInRange.Count();
                changed = true;
            }

            if (changed)
            {
                var newTarget = _game.UnfriendlyUnitsInRange[_currentIndex];
                if (newTarget.Block != null)
                {
                    _game.SetHighlightTarget(newTarget);
                }
            }
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Z) || Raylib.IsKeyPressed(KeyboardKey.C))
        {
            CombatSystem.Attack(_currentUnit, _game.UnfriendlyUnitsInRange[_currentIndex]);
            GameStateManager.ChangeStateType(GameStateType.TransitionSelectorToNextUnit);
        }

        if (Raylib.IsKeyPressed(KeyboardKey.X)) { GameStateManager.ChangeStateType(GameStateType.BattleActionMenu); }
    }

    public void Update()
    {
        _game.Grid.RangeTint.Tick();

        _game.UpdateHighlightPosition();
    }

    public void Draw(float scale)
    {
        _game.Renderer.DrawBackground(scale, _game.Grid);
        _game.Renderer.DrawWeaponAttackRange(scale, _game.Grid);
        _game.Renderer.DrawUnits(scale, _game.Grid, _game.Units);

        _game.Renderer.DrawHighlightRectangle(scale, _game.GetHighlightPosition());
    }
}