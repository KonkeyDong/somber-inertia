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

        // since both the attacker and defender should have the same list of sprites, it shouldn't matter
        // which sprite set we look at to retrieve the last index of the set. YOLO!
        if (_battleSequenceFrame == (_game.AttackContext.MonsterSpriteSet.GetIndexOfLastAttackFrame() * 10) && _game.AttackContext.Hit)
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
        Raylib.ClearBackground(Color.Black);
        var background = BattleBackgrounds.Frames[0];
        _game.Renderer.Draw(scale, background, GameConstants.Battle.Positions.Background);
        _game.Renderer.Draw(scale, _foregroundSprite, GameConstants.Battle.Positions.Foreground);
        _game.Renderer.DrawUnitInfoBox(scale, _game.AttackContext.GetMonster(), GameConstants.Battle.Positions.UnfriendlyStats);
            _game.Renderer.DrawUnitInfoBox(scale, _game.AttackContext.GetForceMember(), GameConstants.Battle.Positions.FriendlyStats);

        if (_battleSequenceFrame >= _battleSequenceFrameLimit)
        {
            var frameIndex = _delayIterator.CurrentIndex;

            if (!_game.AttackContext.GetMonster().IsDead())
            {
                _game.Renderer.Draw(scale, _game.AttackContext.MonsterSpriteSet.GetIdleFrame(frameIndex), _game.AttackContext.MonsterSpriteSet.BasePosition);
            }

            if (!_game.AttackContext.GetForceMember().IsDead())
            {
                _game.Renderer.Draw(scale, _game.AttackContext.ForceMemberSpriteSet.GetIdleFrame(frameIndex), _game.AttackContext.ForceMemberSpriteSet.BasePosition);
            }
        }
        else
        {
            _game.Renderer.Draw(scale, _game.AttackContext.MonsterSpriteSet.GetBattleSequenceFrame(_battleSequenceFrame), _game.AttackContext.MonsterSpriteSet.BasePosition);
            _game.Renderer.Draw(scale, _game.AttackContext.ForceMemberSpriteSet.GetBattleSequenceFrame(_battleSequenceFrame), _game.AttackContext.ForceMemberSpriteSet.BasePosition);
        }
    }
}