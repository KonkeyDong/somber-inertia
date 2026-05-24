using System.Numerics;
using Raylib_cs;
using SomberInertia.Core;
using SomberInertia.Enums;
using SomberInertia.State;

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

        _game.FrameFlipper.Tick();

        _game.UpdateHighlightPosition();
    }

    public void Draw(float scale)
    {
        _game.Renderer.DrawBackground(scale, _game.Grid);
        _game.Renderer.DrawWeaponAttackRange(scale, _game.Grid);
        _game.Renderer.DrawUnits(scale, _game.Grid, _game.Units, _game.FrameFlipper.IsOn);

        _game.Renderer.DrawHighlightRectangle(scale, _game.GetHighlightPosition());
    }
}