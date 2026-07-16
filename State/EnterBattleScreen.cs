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
    private readonly DelayIterator _delayIterator;

    private float _progress = 0f;
    private const float _duration = 60; // total frames for the transition

    // Animation start positions for sprites
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

        // Final (target) positions
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
        var frameIndex = _delayIterator.CurrentIndex;

        // Phase 1: Fade out world map (0.0 -> 0.5)
        if (_progress < 0.5f)
        {
            var mapAlpha = (byte)(255 * (1f - eased * 2));

            _game.Renderer.DrawBackground(scale, _game.Grid, mapAlpha);
            _game.Renderer.DrawUnits(scale, _game.Grid, _game.Units, _game.FrameFlipper.IsOn, mapAlpha);
        }
        // Phase 2: Fade in battle screen + slide sprites (0.5 -> 1.0)
        else
        {
            var battleAlpha = (byte)(255 * ((eased - 0.5f) * 2));

            var backgroundPosition = GameConstants.BASE_BACKGROUND_POSITION * scale;
            var unfriendlyStatsPosition = GameConstants.BASE_UNFRIENDLY_STATS_POSITION * scale;
            var friendlyStatsPosition = GameConstants.BASE_FRIENDLY_STATS_POSITION * scale;

            var foregroundPosition = Vector2.Lerp(_startForegroundPosition, GameConstants.BASE_FOREGROUND_POSITION * scale, eased);
            var unfriendlyPosition = Vector2.Lerp(_startUnfriendlyPosition, GameConstants.BASE_UNFRIENDLY_POSITION * scale, eased);
            var friendlyPosition   = Vector2.Lerp(_startFriendlyPosition,   GameConstants.BASE_FRIENDLY_POSITION * scale, eased);

            // Draw battle background
            var background = BattleBackgrounds.Frames[0];
            _game.Renderer.Draw(scale, background, backgroundPosition, battleAlpha);
            _game.Renderer.DrawUnitInfoBox(scale, _game.AttackContext.GetMonster(), unfriendlyStatsPosition, battleAlpha);
            _game.Renderer.DrawUnitInfoBox(scale, _game.AttackContext.GetForceMember(), friendlyStatsPosition, battleAlpha);

            // Draw sprites with slide + fade
            _game.Renderer.Draw(scale, _game.AttackContext.MonsterSpriteSet.GetIdleFrame(frameIndex), unfriendlyPosition, battleAlpha);
            _game.Renderer.Draw(scale, _foregroundSprite, foregroundPosition, battleAlpha);
            _game.Renderer.Draw(scale, _game.AttackContext.ForceMemberSpriteSet.GetIdleFrame(frameIndex), friendlyPosition, battleAlpha);
        }
    }
}