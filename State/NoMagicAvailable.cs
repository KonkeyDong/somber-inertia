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

        // draw no magic message
        var text = "No magic";
        var textPosition = new Vector2(100, 100);
        var fontSize = 20;
        var textSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), text, fontSize, 1);
        var padding = 10;

        var background = new Rectangle(
            textPosition.X - padding,
            textPosition.Y - padding,
            textSize.X + padding * 2,
            textSize.Y + padding * 2
        );

        // Draw background first
        Raylib.DrawRectangleRounded(background, 0.2f, 6, GameConfig.Textures.BlueBackground);

        // Draw text on top
        Raylib.DrawTextEx(
            Raylib.GetFontDefault(), 
            text, 
            textPosition, 
            fontSize, 
            1, 
            Color.White
        );
    }
}