using SomberInertia.Core;
using SomberInertia.Graphics;
using SomberInertia.Enums;
using SomberInertia.Timers;
using System.Numerics;
using Raylib_cs;

namespace SomberInertia.State;

public class ExitBattleScreen : IGameState
{
    private readonly Game _game;
    private readonly DelayIterator _delayIterator;
    private readonly Sprite _foregroundSprite;

    private float _progress = 0f;
    private const float _duration = 60; // frames for the full transition

    public ExitBattleScreen(Game game)
    {
        _game = game;
        _delayIterator = new DelayIterator(GameConstants.Animations.IdleDelay);

        _foregroundSprite = new Sprite("Assets/Foregrounds/Rock.png", new FrameRect
        {
            X = 0, Y = 0, W = 96, H = 32
        });
    }

    public void Enter()
    {
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
        _game.FrameFlipper.Tick();

        if (_progress < 1f)
        {
            _progress += 1f / _duration;
            _progress = Math.Min(1f, _progress);
        }
        else
        {
            GameStateManager.ChangeStateType(GameStateType.AnimateUnitDeaths);
        }
    }

    public void Draw(float scale)
    {
        Raylib.ClearBackground(Color.Black);

        var eased = _game.Renderer.EaseInOut(_progress);

        var backgroundPosition = GameConstants.Battle.Positions.Background;
        var unfriendlyStatsPosition = GameConstants.Battle.Positions.UnfriendlyStats;
        var friendlyStatsPosition = GameConstants.Battle.Positions.FriendlyStats;

        // Phase 1: Fade battle screen to black (0.0 -> 0.5)
        if (_progress < 0.5f)
        {
            var battleAlpha = (byte)(255 * (1f - eased * 2));

            // Draw current battle screen with fading alpha
            var background = BattleBackgrounds.Frames[0];
            _game.Renderer.Draw(scale, background, backgroundPosition, battleAlpha);

            _game.Renderer.DrawUnitInfoBox(scale, _game.AttackContext.GetMonster(), unfriendlyStatsPosition, battleAlpha);
            _game.Renderer.DrawUnitInfoBox(scale, _game.AttackContext.GetForceMember(), friendlyStatsPosition, battleAlpha);

            var frameIndex = _delayIterator.CurrentIndex;

            if (!_game.AttackContext.GetMonster().IsDead())
            {
                _game.Renderer.Draw(scale, _game.AttackContext.MonsterSpriteSet.GetIdleFrame(frameIndex), 
                    _game.AttackContext.MonsterSpriteSet.BasePosition, battleAlpha);
            }
            
            _game.Renderer.Draw(scale, _foregroundSprite, GameConstants.Battle.Positions.Foreground, battleAlpha);

            if (!_game.AttackContext.GetForceMember().IsDead())
            {
                _game.Renderer.Draw(scale, _game.AttackContext.ForceMemberSpriteSet.GetIdleFrame(frameIndex), 
                    _game.AttackContext.ForceMemberSpriteSet.BasePosition, battleAlpha);
            }
        }
        // Phase 2: Fade in world map (0.5 -> 1.0)
        else
        {
            var mapAlpha = (byte)(255 * ((eased - 0.5f) * 2));

            _game.Renderer.DrawBackground(scale, _game.Grid, mapAlpha);
            _game.Renderer.DrawUnits(scale, _game.Grid, _game.Units, _game.FrameFlipper.IsOn, mapAlpha);
        }
    }
}