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
    private int _battleSequenceFrameLimit;
    private readonly Delay _delay;

    private SequenceTimerSlot[] _delayEffects = Array.Empty<SequenceTimerSlot>();
    private List<Sprite> _artilleryExplosions = new();

    public BattleResolution(Game game)
    {
        _game = game;

        _battleSequenceFrame = 0;
        _delay = new Delay(GameConstants.Animations.IdleDelay);

        _foregroundSprite = BattleForegrounds.Get(ForegroundNames.RoughTerrain);
    }

    public void Enter()
    {
        _delayEffects = Array.Empty<SequenceTimerSlot>();
        _artilleryExplosions = new List<Sprite>();

        // Use the longer side so dissolve frames on either sprite set are not truncated.
        _battleSequenceFrameLimit = Math.Max(
            _game.AttackContext.ForceMemberSpriteSet.BattleSequence.Count,
            _game.AttackContext.MonsterSpriteSet.BattleSequence.Count);

        Logger.Info("Attack Context Effect: " + _game.AttackContext.Effect.ToString());
        Logger.Debug($"DamageApplyFrame={_game.AttackContext.DamageApplyFrame}, sequenceLimit={_battleSequenceFrameLimit}");

        if (_game.AttackContext.Effect == Effects.ArtilleryExplosion)
        {
            _artilleryExplosions = ArtilleryExplosion.Frames;
            _delayEffects = ArtilleryBattleEffects.CreateSlots(
                _game.AttackContext.ForceMemberSpriteSet);

            var effectsDuration = ArtilleryBattleEffects.MaxDuration(_delayEffects);
            _battleSequenceFrameLimit = Math.Max(_battleSequenceFrameLimit, effectsDuration);
        }
    }

    public void Exit()
    {
        _delayEffects = Array.Empty<SequenceTimerSlot>();
    }

    public void HandleInput()
    {
    }

    public void Update()
    {
        _delay.Tick();

        if (_delayEffects.Length > 0)
        {
            for (var i = 0; i < _delayEffects.Length; i++)
            {
                _delayEffects[i].SequenceTimer.Tick();
            }
        }

        _battleSequenceFrame++;

        // Apply damage on the attacker-paced frame stored when the sequence was built.
        if (_game.AttackContext.Hit
            && _battleSequenceFrame == _game.AttackContext.DamageApplyFrame)
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
        var background = BattleBackgrounds.Get(BackgroundNames.GatesOfGuardiana);
        _game.Renderer.Draw(scale, background, GameConstants.Battle.Positions.Background);
        _game.Renderer.Draw(scale, _foregroundSprite, GameConstants.Battle.Positions.Foreground);
        _game.Renderer.DrawUnitInfoBox(scale, _game.AttackContext.GetMonster(), GameConstants.Battle.Positions.UnfriendlyStats);
        _game.Renderer.DrawUnitInfoBox(scale, _game.AttackContext.GetForceMember(), GameConstants.Battle.Positions.FriendlyStats);

        if (_battleSequenceFrame >= _battleSequenceFrameLimit)
        {
            var frameIndex = _delay.CurrentIndex;

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

            ArtilleryBattleEffects.DrawRange(scale, _game.Renderer, _delayEffects, _artilleryExplosions, 0, 3);
            _game.Renderer.Draw(scale, _game.AttackContext.ForceMemberSpriteSet.GetBattleSequenceFrame(_battleSequenceFrame), _game.AttackContext.ForceMemberSpriteSet.BasePosition);
            ArtilleryBattleEffects.DrawRange(scale, _game.Renderer, _delayEffects, _artilleryExplosions, 3, 7);
        }
    }
}
