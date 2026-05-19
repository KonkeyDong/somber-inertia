using SomberInertia.Core;
using SomberInertia.Enums;
using System.Numerics;

using Raylib_cs;

namespace SomberInertia.State;

public class SelectEnemyForPhysicalAttack : IGameState
{
    private Game _game { get; set; }
    private Unit _currentUnit { get; set; }
    private int _currentIndex { get; set; }

    // Smooth highlight animation
    private Vector2 _currentHighlightPos;
    private Vector2 _targetHighlightPos;

    private const float _highlightSpeed = 1000f; // lower number represents slower speed

    public SelectEnemyForPhysicalAttack(Game game)
    {
        _game = game;
        _currentUnit = _game.GetCurrentUnit();
    }

    public void Enter()
    {
        if (_game.UnfriendlyUnitsInRange.Count > 0)
        {
            _currentIndex = 0;

            var block = _game.UnfriendlyUnitsInRange[_currentIndex].Block;
            if (block != null)
            {
                _currentHighlightPos = _game.GetScaledBlockVectorPosition(block);
                _targetHighlightPos = _currentHighlightPos;
            }
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
                
                _currentIndex  = (_currentIndex + 1) % _game.UnfriendlyUnitsInRange.Count();
                changed = true;
            }
            if (Raylib.IsKeyPressed(KeyboardKey.Right)) 
            { 
                _currentIndex = (_currentIndex - 1 + _game.UnfriendlyUnitsInRange.Count() ) % _game.UnfriendlyUnitsInRange.Count();
                changed = true;
            }

            if (changed)
            {
                var newTarget = _game.UnfriendlyUnitsInRange[_currentIndex];
                if (newTarget.Block != null)
                {
                    _targetHighlightPos = _game.GetScaledBlockVectorPosition(newTarget.Block);
                }
            }
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

        // Move towards target at constant speed
        var distance = Vector2.Distance(_currentHighlightPos, _targetHighlightPos);

        if (distance > 0.1f)
        {
            var direction = Vector2.Normalize(_targetHighlightPos - _currentHighlightPos);
            var moveDistance = _highlightSpeed * Raylib.GetFrameTime();

            // Don't overshoot
            if (moveDistance > distance)
            {
                moveDistance = distance;
            }

            _currentHighlightPos += direction * moveDistance;
        }
        else
        {
            _currentHighlightPos = _targetHighlightPos; // snap when very close
        }
    }

    public void Draw(float scale)
    {
        _game.Grid.DrawBackground(scale);
        _game.Grid.DrawWeaponAttackRange(scale);
        _game.Grid.DrawUnits(_game.Units, scale);

        _game.Grid.DrawHighlightRectangle(scale, _currentHighlightPos);
    }
}