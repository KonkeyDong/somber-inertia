using System.Numerics;
using Raylib_cs;
using SomberInertia.Core;
using SomberInertia.Enums;
using SomberInertia.Core.Units;
using SomberInertia.Core.Combat;

namespace SomberInertia.State;

public class NoMagicTargetAvailable : IGameState
{
    private Game _game { get; set; }
    private Unit _currentUnit { get; set; }
    private int _countdownTimer = GameConstants.Animations.SwitchStateCountdownTimer;

    public NoMagicTargetAvailable(Game game)
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
        if (Raylib.IsKeyPressed(KeyboardKey.Z) || Raylib.IsKeyPressed(KeyboardKey.X) || Raylib.IsKeyPressed(KeyboardKey.C))
        {
            GameStateManager.ChangeStateType(GameStateType.SelectMagicLevel);
        }
    }

    public void Update()
    {
        _game.Grid.RangeTint.Tick();
        _game.FrameFlipper.Tick();

        _countdownTimer--;

        if (_countdownTimer <= 0)
        {
            Logger.Info("Countdown timer has exhausted. Changing state.");
            GameStateManager.ChangeStateType(GameStateType.SelectMagicLevel);
        }
    }

    public void Draw(float scale)
    {
        _game.Renderer.DrawBackground(scale, _game.Grid);
        _game.Renderer.DrawMagicAttackRange(scale, _game.Grid);
        _game.Renderer.DrawUnits(scale, _game.Grid, _game.Units, _game.FrameFlipper.IsOn);

        var position = GameConstants.WorldMap.Positions.BASE_NO_TARGET_MESSAGE_BOX_POSITION * (int)scale;

        _game.Renderer.DrawBattleMenuMessage(scale, "No target", position);
    }
}