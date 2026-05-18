using SomberInertia.Core;
using SomberInertia.Enums;

using Raylib_cs;

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

    }

    public void Exit()
    {

    }

    public void HandleInput()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Left))  { _currentIndex = (_currentIndex + 1) % _game.UnfriendlyUnitsInRange.Count(); }
        if (Raylib.IsKeyPressed(KeyboardKey.Right)) 
        { 
            _currentIndex = (_currentIndex - 1 + _game.UnfriendlyUnitsInRange.Count() ) % _game.UnfriendlyUnitsInRange.Count(); 
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Z) || Raylib.IsKeyPressed(KeyboardKey.C))
        {
            CombatSystem.Attack(_currentUnit, _game.UnfriendlyUnitsInRange[_currentIndex]);
            GameStateManager.ChangeStateType(GameStateType.EndTurn);
        }

        if (Raylib.IsKeyPressed(KeyboardKey.X)) { GameStateManager.ChangeStateType(GameStateType.BattleActionMenu); }
    }

    public void Update()
    {
        _game.Grid.RangeTint.Tick();
    }

    public void Draw(float scale)
    {
        _game.Grid.DrawBackground(scale);
        _game.Grid.DrawWeaponAttackRange(scale);
        _game.Grid.DrawUnits(_game.Units, scale);

        _game.Grid.DrawRectangleAroundCurrentUnit(scale, _game.UnfriendlyUnitsInRange[_currentIndex]);
    }
}