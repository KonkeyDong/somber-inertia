using SomberInertia.Enums;
using SomberInertia.Core;
using SomberInertia.Core.Units;
using SomberInertia.Graphics;
using System.Numerics;

using Raylib_cs;

namespace SomberInertia.State;

public class NoMagicAvailable : IGameState
{
    private readonly Game _game;
    private Unit _currentUnit;
    private int _countdownTimer = GameConfig.Animations.SwitchStateCountdownTimer;

    public NoMagicAvailable(Game game)
    {
        _game = game;
        _currentUnit = _game.GetCurrentUnit();
    }

    public void Enter() 
    {
        Logger.Warning("Text rendering in this state is a placeholder. Remember need to make a menu text renderer later.");
    }

    public void Exit()
    {

    }

    public void HandleInput()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Z) || Raylib.IsKeyPressed(KeyboardKey.X) || Raylib.IsKeyPressed(KeyboardKey.C))
        {
            Logger.Info("Input has been handling. Changing state.");
            GameStateManager.ChangeStateType(GameStateType.BattleActionMenu);
        }
    }

    public void Update()
    {
        _game.FrameFlipper.Tick();

        _countdownTimer--;

        if (_countdownTimer <= 0)
        {
            Logger.Info("Countdown timer has exhausted. Changing state.");
            GameStateManager.ChangeStateType(GameStateType.BattleActionMenu);
        }
    }

    public void Draw(float scale)
    {
        _game.Renderer.DrawBackground(scale, _game.Grid);
        _game.Renderer.DrawUnits(scale, _game.Grid, _game.Units, _game.FrameFlipper.IsOn);

        _game.Renderer.DrawBattleMenuMessage(scale, "No magic", new Vector2(100, 100));
    }
}