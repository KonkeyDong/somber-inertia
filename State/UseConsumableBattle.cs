using System.Numerics;
using Raylib_cs;
using SomberInertia.Core;
using SomberInertia.Core.Combat.Item;
using SomberInertia.Core.Units;
using SomberInertia.Enums;
using SomberInertia.Graphics;
using SomberInertia.Timers;

namespace SomberInertia.State;

/// <summary>
/// Item consumable battle beat (force side only).
/// Apply effect, hold 60 frames (stats update visible), then:
/// self → fade to map; ally → slide target out and restore caster.
/// </summary>
public class UseConsumableBattle : IGameState
{
    private enum Phase
    {
        // Self: apply + hold, then exit
        SelfPostEffectHold,
        // Ally
        ShowCaster,
        SlideCasterOut,
        SlideTargetIn,
        PostEffectHold,
        SlideTargetOut,
        SlideCasterBack,
        Done
    }

    private readonly Game _game;
    private readonly Delay _delay;
    private readonly Sprite _foregroundSprite;

    private ItemContext _context = null!;
    private Phase _phase;
    private float _progress;
    private bool _effectApplied;

    private Vector2 _casterBase;
    private Vector2 _targetBase;
    private Vector2 _displayPosition;
    private Unit _displayUnit = null!;
    private BattleUnitSpriteSet _displaySprites = null!;

    private const float PhaseDuration = 40f; // frames per slide segment
    private const float PostEffectHoldFrames = 60f;
    private const float OffScreenOffsetX = 160f;

    public UseConsumableBattle(Game game)
    {
        _game = game;
        _delay = new Delay(GameConstants.Animations.IdleDelay);
        _foregroundSprite = BattleForegrounds.Get(ForegroundNames.RoughTerrain);
    }

    public void Enter()
    {
        if (_game.ItemContext == null)
        {
            Logger.Error("UseConsumableBattle: ItemContext is null. Exiting to EndTurn.");
            GameStateManager.ChangeStateType(GameStateType.EndTurn);
            return;
        }

        _context = _game.ItemContext;
        _effectApplied = false;
        _progress = 0f;

        // Per-unit friendly slot from GameConstants.Battle (name + equipped weapon).
        _casterBase = GameConstants.Battle.GetSpritePosition(_context.Caster);
        _targetBase = GameConstants.Battle.GetSpritePosition(_context.Target);
        _context.CasterSprites.BasePosition = _casterBase;
        _context.TargetSprites.BasePosition = _targetBase;

        if (_context.IsSelfTarget)
        {
            _phase = Phase.SelfPostEffectHold;
            _displayUnit = _context.Caster;
            _displaySprites = _context.CasterSprites;
            _displayPosition = _casterBase;
            ApplyEffect();
        }
        else
        {
            _phase = Phase.ShowCaster;
            _displayUnit = _context.Caster;
            _displaySprites = _context.CasterSprites;
            _displayPosition = _casterBase;
        }
    }

    private static Vector2 OffRightOf(Vector2 basePosition) =>
        new Vector2(basePosition.X + OffScreenOffsetX, basePosition.Y);

    public void Exit()
    {
    }

    public void HandleInput()
    {
    }

    public void Update()
    {
        _delay.Tick();

        switch (_phase)
        {
            case Phase.SelfPostEffectHold:
                // Effect already applied on Enter; hold so heal shows, then fade to map.
                _progress += 1f / PostEffectHoldFrames;
                if (_progress >= 1f)
                {
                    Finish();
                }
                break;

            case Phase.ShowCaster:
                _progress += 1f / (PhaseDuration * 0.5f);
                if (_progress >= 1f)
                {
                    Advance(Phase.SlideCasterOut);
                }
                break;

            case Phase.SlideCasterOut:
                _progress += 1f / PhaseDuration;
                _displayPosition = Vector2.Lerp(_casterBase, OffRightOf(_casterBase), Ease(_progress));
                if (_progress >= 1f)
                {
                    _displayUnit = _context.Target;
                    _displaySprites = _context.TargetSprites;
                    _displayPosition = OffRightOf(_targetBase);
                    Advance(Phase.SlideTargetIn);
                }
                break;

            case Phase.SlideTargetIn:
                _progress += 1f / PhaseDuration;
                _displayPosition = Vector2.Lerp(OffRightOf(_targetBase), _targetBase, Ease(_progress));
                if (_progress >= 1f)
                {
                    _displayPosition = _targetBase;
                    ApplyEffect();
                    Advance(Phase.PostEffectHold);
                }
                break;

            case Phase.PostEffectHold:
                _progress += 1f / PostEffectHoldFrames;
                if (_progress >= 1f)
                {
                    Advance(Phase.SlideTargetOut);
                }
                break;

            case Phase.SlideTargetOut:
                _progress += 1f / PhaseDuration;
                _displayPosition = Vector2.Lerp(_targetBase, OffRightOf(_targetBase), Ease(_progress));
                if (_progress >= 1f)
                {
                    _displayUnit = _context.Caster;
                    _displaySprites = _context.CasterSprites;
                    _displayPosition = OffRightOf(_casterBase);
                    Advance(Phase.SlideCasterBack);
                }
                break;

            case Phase.SlideCasterBack:
                _progress += 1f / PhaseDuration;
                _displayPosition = Vector2.Lerp(OffRightOf(_casterBase), _casterBase, Ease(_progress));
                if (_progress >= 1f)
                {
                    _displayPosition = _casterBase;
                    Finish();
                }
                break;

            case Phase.Done:
                break;
        }
    }

    private void Advance(Phase next)
    {
        _phase = next;
        _progress = 0f;
    }

    private void ApplyEffect()
    {
        if (_effectApplied)
        {
            return;
        }

        _effectApplied = true;
        var slot = _context.Caster.GetItemAtIndex(_context.ItemSlotIndex);
        ItemDatabase.UseItem(slot.Name, _context);
    }

    private void Finish()
    {
        _phase = Phase.Done;
        GameStateManager.ChangeStateType(GameStateType.ExitBattleScreen);
    }

    private static float Ease(float t) =>
        // Match battle enter-style smoothing without depending on Renderer instance early.
        t * t * (3f - 2f * t);

    public void Draw(float scale)
    {
        Raylib.ClearBackground(Color.Black);

        var background = BattleBackgrounds.Get(BackgroundNames.GatesOfGuardiana);
        _game.Renderer.Draw(scale, background, GameConstants.Battle.Positions.Background);

        _game.Renderer.DrawUnitInfoBox(
            scale,
            _displayUnit,
            GameConstants.Battle.Positions.FriendlyStats);

        var frameIndex = _delay.CurrentIndex;
        _game.Renderer.Draw(scale, _foregroundSprite, GameConstants.Battle.Positions.Foreground);
        _game.Renderer.Draw(scale, _displaySprites.GetIdleFrame(frameIndex), _displayPosition);
    }
}
