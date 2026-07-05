using SomberInertia.Core;
using SomberInertia.Core.Combat;
using SomberInertia.Graphics;
using SomberInertia.Timers;
using System.Numerics;
using Raylib_cs;

namespace SomberInertia.State;

public class EnterBattleScreen : IGameState
{
    private readonly Game _game;
    private BattleSpriteSet _forceMemberSpriteSet = new();
    private BattleSpriteSet _monsterSpriteSet = new();

    private readonly Sprite _foregroundSprite;

    private DelayIterator _delayIterator;

    // Animation progress
    private float _progress = 0f;

    private const float _duration = 60;

    // Base positions (in 256x224 resolution)
    private readonly Vector2 _baseBackgroundPosition = GameConstants.BASE_BACKGROUND_POSITION;
    private readonly Vector2 _baseUnfriendlyPosition = GameConstants.BASE_UNFRIENDLY_POSITION;
    private readonly Vector2 _baseFriendlyPosition   = GameConstants.BASE_FRIENDLY_POSITION;
    private readonly Vector2 _baseForegroundPosition = GameConstants.BASE_FOREGROUND_POSITION;

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
        // Assign correct sprite sets
        var defenderSprites = BattleSpriteManager.Get(_game.AttackContext.Defender);
        var attackerSprites = BattleSpriteManager.Get(_game.AttackContext.Attacker);

        if (_game.AttackContext.Defender.Friendly)
        {
            _forceMemberSpriteSet = defenderSprites;
            _monsterSpriteSet = attackerSprites;
        }
        else
        {
            _monsterSpriteSet = defenderSprites;
            _forceMemberSpriteSet = attackerSprites;
        }

        var scale = GameStateManager.CurrentScale;

        // Target (final) positions
        var targetUnfriendly = _baseUnfriendlyPosition * scale;
        var targetFriendly   = _baseFriendlyPosition * scale;
        var targetForeground = _baseForegroundPosition * scale;

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
    }

    public void Draw(float scale)
    {
        Raylib.ClearBackground(Color.Black);

        var eased = _game.Renderer.EaseInOut(_progress);

        var backgroundPosition = _baseBackgroundPosition * scale;
        var unfriendlyPosition = Vector2.Lerp(_startUnfriendlyPosition, _baseUnfriendlyPosition * scale, eased);
        var friendlyPosition   = Vector2.Lerp(_startFriendlyPosition,   _baseFriendlyPosition * scale, eased);
        var foregroundPosition = Vector2.Lerp(_startForegroundPosition, _baseForegroundPosition * scale, eased);

        // Draw sprites with fade
        var alpha = (byte)(255 * _progress);
        var frameIndex = _delayIterator.CurrentIndex;

        // Draw background
        var background = BattleBackgrounds.Frames[0];
        _game.Renderer.Draw(scale, background, backgroundPosition, alpha);

        _game.Renderer.Draw(scale, _monsterSpriteSet.GetIdleFrame(frameIndex), unfriendlyPosition, alpha);
        _game.Renderer.Draw(scale, _foregroundSprite, foregroundPosition, alpha);
        _game.Renderer.Draw(scale, _forceMemberSpriteSet.GetIdleFrame(frameIndex), friendlyPosition, alpha);
    }
}