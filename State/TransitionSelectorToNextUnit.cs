using SomberInertia.Core;
using SomberInertia.State;
using SomberInertia.Enums;
using System.Numerics;

using Raylib_cs;

namespace SomberInertia.State;

public class TransitionSelectorToNextUnit : IGameState
{
    private readonly Game _game;
    
    public TransitionSelectorToNextUnit(Game game)
    {
        _game = game;
    }

    public void Enter()
    {
        _game.InitializeHighlight();
        _game.SetHighlightTarget(_game.GetNextUnit());
    }

    public void Exit()
    {

    }

    public void HandleInput()
    {

    }

    public void Update()
    {
        if (_game.IsHighlightSettled())
        {
            GameStateManager.ChangeStateType(GameStateType.EndTurn);
        }
        
        _game.UpdateHighlightPosition();
    }

    public void Draw(float scale)
    {
        _game.Grid.DrawBackground(scale);
        _game.Grid.DrawWeaponAttackRange(scale);
        _game.Grid.DrawUnits(_game.Units, scale);

        _game.Grid.DrawHighlightRectangle(scale, _game.GetHighlightPosition());
    }
}