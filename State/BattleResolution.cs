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
    private int _battleSequenceFrame;
    private readonly int _battleSequenceFrameLimit;
    private readonly DelayIterator _delayIterator;

    public BattleResolution(Game game)
    {
        _game = game;

        _battleSequenceFrame = 0;
        _battleSequenceFrameLimit = _game.AttackContext.ForceMemberSpriteSet.BattleSequence.Count;
        _delayIterator = new DelayIterator(GameConstants.Animations.IdleDelay);

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
        _battleSequenceFrame++;

        if (_battleSequenceFrame == _battleSequenceFrameLimit && _game.AttackContext.Hit)
        {
            _game.AttackContext.Defender.TakeDamage(_game.AttackContext.Damage);
        }

        if (_battleSequenceFrame > _battleSequenceFrameLimit + 60)
        {
            GameStateManager.ChangeStateType(GameStateType.ExitBattleScreen);
        }
    }

    public void Draw(float scale)
    {
        var backgroundPosition = GameConstants.Battle.Positions.BASE_BACKGROUND_POSITION * scale;
        var foregroundPosition = GameConstants.Battle.Positions.BASE_FOREGROUND_POSITION * scale;
        var unfriendlyPosition = GameConstants.BASE_UNFRIENDLY_POSITION * scale;
        var friendlyPosition   = _game.AttackContext.ForceMemberSpriteSet.BasePosition * scale;
        var unfriendlyStatsPosition = GameConstants.BASE_UNFRIENDLY_STATS_POSITION * scale;
        var friendlyStatsPosition = GameConstants.BASE_FRIENDLY_STATS_POSITION * scale;

        Raylib.ClearBackground(Color.Black);
        var background = BattleBackgrounds.Frames[0];
        _game.Renderer.Draw(scale, background, backgroundPosition);
        _game.Renderer.Draw(scale, _foregroundSprite, foregroundPosition);
        _game.Renderer.DrawUnitInfoBox(scale, _game.AttackContext.GetMonster(), unfriendlyStatsPosition);
            _game.Renderer.DrawUnitInfoBox(scale, _game.AttackContext.GetForceMember(), friendlyStatsPosition);

        if (_battleSequenceFrame > _battleSequenceFrameLimit)
        {
            var frameIndex = _delayIterator.CurrentIndex;
            _game.Renderer.Draw(scale, _game.AttackContext.MonsterSpriteSet.GetIdleFrame(frameIndex), unfriendlyPosition);
            _game.Renderer.Draw(scale, _game.AttackContext.ForceMemberSpriteSet.GetIdleFrame(frameIndex), friendlyPosition);
        }
        else
        {
            _game.Renderer.Draw(scale, _game.AttackContext.MonsterSpriteSet.GetBattleSequenceFrame(_battleSequenceFrame), unfriendlyPosition);
            _game.Renderer.Draw(scale, _game.AttackContext.ForceMemberSpriteSet.GetBattleSequenceFrame(_battleSequenceFrame), friendlyPosition);
        }
    }
}