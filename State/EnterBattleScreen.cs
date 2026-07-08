using SomberInertia.Core;
using SomberInertia.Core.Combat;
using SomberInertia.Graphics;
using SomberInertia.Timers;
using SomberInertia.Enums;
using System.Numerics;
using Raylib_cs;

namespace SomberInertia.State;

public class EnterBattleScreen : IGameState
{
    private readonly Game _game;
    private readonly Sprite _foregroundSprite;

    private DelayIterator _delayIterator;

    // Animation progress
    private float _progress = 0f;
    private const float _duration = 60;

    // Animation start positions
    private Vector2 _startUnfriendlyPosition;
    private Vector2 _startFriendlyPosition;
    private Vector2 _startForegroundPosition;

    public EnterBattleScreen(Game game)
    {
        _game = game;
        _delayIterator = new DelayIterator(GameConfig.Animations.IdleDelay);

        _foregroundSprite = new Sprite("Assets/Foregrounds/Rock.png", new FrameRect
        {
            X = 0, Y = 0, W = 96, H = 32
        });
    }

    public void Enter()
    {
        var scale = GameStateManager.CurrentScale;

        // Target (final) positions
        var targetUnfriendly = GameConstants.BASE_UNFRIENDLY_POSITION * scale;
        var targetFriendly   = GameConstants.BASE_FRIENDLY_POSITION * scale;
        var targetForeground = GameConstants.BASE_FOREGROUND_POSITION * scale;

        // Start positions (off-screen)
        _startUnfriendlyPosition = new Vector2(targetUnfriendly.X - 140, targetUnfriendly.Y);
        _startFriendlyPosition   = new Vector2(targetFriendly.X + 140, targetFriendly.Y);
        _startForegroundPosition = new Vector2(targetForeground.X + 100, targetForeground.Y);

        _progress = 0f;
    }

    public void Exit()
    {

    }

    public void HandleInput()
    {

    }

    public void Update()
    {
        _delayIterator.Tick();

        if (_progress < 1f)
        {
            _progress += 1f / _duration;
            _progress = Math.Min(1f, _progress);
        }
        else
        {
            GameStateManager.ChangeStateType(GameStateType.BattleResolution);
        }
    }

    public void Draw(float scale)
    {
        Raylib.ClearBackground(Color.Black);

        var eased = _game.Renderer.EaseInOut(_progress);

        var backgroundPosition = GameConstants.BASE_BACKGROUND_POSITION * scale;
        var foregroundPosition = Vector2.Lerp(_startForegroundPosition, GameConstants.BASE_FOREGROUND_POSITION * scale, eased);
        var unfriendlyPosition = Vector2.Lerp(_startUnfriendlyPosition, GameConstants.BASE_UNFRIENDLY_POSITION * scale, eased);
        var friendlyPosition   = Vector2.Lerp(_startFriendlyPosition,   GameConstants.BASE_FRIENDLY_POSITION * scale, eased);

        var alpha = (byte)(255 * _progress);
        var frameIndex = _delayIterator.CurrentIndex;

        var background = BattleBackgrounds.Frames[0];
        _game.Renderer.Draw(scale, background, backgroundPosition, alpha);

        _game.Renderer.Draw(scale, _game.AttackContext.MonsterSpriteSet.GetIdleFrame(frameIndex), unfriendlyPosition, alpha);
        _game.Renderer.Draw(scale, _foregroundSprite, foregroundPosition, alpha);
        _game.Renderer.Draw(scale, _game.AttackContext.ForceMemberSpriteSet.GetIdleFrame(frameIndex), friendlyPosition, alpha);
    }
}