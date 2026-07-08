using SomberInertia.Core;
using SomberInertia.Timers;
using SomberInertia.Enums;
using SomberInertia.Graphics;

using System.Numerics;
using Raylib_cs;

namespace SomberInertia.State;

public class BattleResolution : IGameState
{
    private Game _game;
    private readonly Sprite _foregroundSprite;
    private DelayIterator _delayIterator;
    private int _finalAttackFrameAttackDelay = GameConfig.Animations.IdleDelay;

    private bool _isAttackAnimationComplete = false;

    public BattleResolution(Game game)
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

        if (_isAttackAnimationComplete)
        {
            _finalAttackFrameAttackDelay--;
        }

        if (_finalAttackFrameAttackDelay < -60)
        {
            GameStateManager.ChangeStateType(GameStateType.AnimateUnitDeaths);
        }
    }

    public void Draw(float scale)
    {
        var backgroundPosition = GameConstants.BASE_BACKGROUND_POSITION * scale;
        var foregroundPosition = GameConstants.BASE_FOREGROUND_POSITION * scale;
        var unfriendlyPosition = GameConstants.BASE_UNFRIENDLY_POSITION * scale;
        var friendlyPosition   = GameConstants.BASE_FRIENDLY_POSITION * scale;

        var frameIndex = _delayIterator.CurrentIndex;

        Raylib.ClearBackground(Color.Black);
        var background = BattleBackgrounds.Frames[0];
        _game.Renderer.Draw(scale, background, backgroundPosition);
        _game.Renderer.Draw(scale, _foregroundSprite, foregroundPosition);

        if (_isAttackAnimationComplete && _finalAttackFrameAttackDelay < 0)
        {
            _game.Renderer.Draw(scale, _game.AttackContext.MonsterSpriteSet.GetIdleFrame(frameIndex), unfriendlyPosition);
            _game.Renderer.Draw(scale, _game.AttackContext.ForceMemberSpriteSet.GetIdleFrame(frameIndex), friendlyPosition);
        }
        else
        {
            if (_game.AttackContext.Attacker.Friendly)
            {
                _game.Renderer.Draw(scale, _game.AttackContext.MonsterSpriteSet.GetIdleFrame(frameIndex), unfriendlyPosition);
                _game.Renderer.Draw(scale, _game.AttackContext.ForceMemberSpriteSet.GetAttackFrame(frameIndex, out _isAttackAnimationComplete), friendlyPosition);
            }
            else
            {
                _game.Renderer.Draw(scale, _game.AttackContext.MonsterSpriteSet.GetAttackFrame(frameIndex, out _isAttackAnimationComplete), unfriendlyPosition);
                _game.Renderer.Draw(scale, _game.AttackContext.ForceMemberSpriteSet.GetIdleFrame(frameIndex), friendlyPosition);
            }
        }
    }
}