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
/// Apply effect once on first hold, hold, then:
/// self → fade to map; multi/ally → slide units out/in, restore caster, exit.
/// </summary>
public class UseConsumableBattle : IGameState
{
    private enum Phase
    {
        FirstHold,
        SlideOut,
        SlideIn,
        Hold,
        SlideCasterBack,
        Done
    }

    private readonly Game _game;
    private readonly Delay _delay;
    private readonly Sprite _foregroundSprite;

    private ItemContext _context = null!;
    private ItemData _itemData;
    private Phase _phase;
    private int _phaseFrame;
    private bool _itemConsumed;
    private bool _returnToCasterAfterSlideOut;

    /// <summary>Units already given the consumable effect this battle (on-screen apply).</summary>
    private readonly HashSet<Unit> _unitsAlreadyAffected = new();

    /// <summary>Caster first, then other targets (no duplicates).</summary>
    private readonly List<(Unit Unit, BattleUnitSpriteSet Sprites, Vector2 Base)> _queue = new();

    private int _queueIndex;
    private int _incomingIndex;

    private Vector2 _slideFrom;
    private Vector2 _slideTo;
    private Vector2 _displayPosition;
    private Unit _displayUnit = null!;
    private BattleUnitSpriteSet _displaySprites = null!;

    private const int SlideFrames = 40;
    private const int HoldFrames = 60;

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
        _itemData = ItemDatabase.Get(_context.Caster.GetItemAtIndex(_context.ItemSlotIndex).Name);
        _itemConsumed = false;
        _returnToCasterAfterSlideOut = false;
        _phaseFrame = 0;
        _queue.Clear();
        _unitsAlreadyAffected.Clear();

        BuildPresentationQueue();

        if (_queue.Count == 0)
        {
            Logger.Error("UseConsumableBattle: empty presentation queue.");
            GameStateManager.ChangeStateType(GameStateType.EndTurn);
            return;
        }

        _queueIndex = 0;
        SetDisplayed(_queue[0]);
        // Heal/cure this unit only if they are a real target and currently on screen.
        TryApplyEffectToDisplayedUnit();
        BeginPhase(Phase.FirstHold);

        Logger.Debug(
            $"UseConsumableBattle: queue={_queue.Count} " +
            $"[{string.Join(", ", _queue.Select(e => e.Unit.GetDisplayName()))}]");
    }

    /// <summary>
    /// Always: caster first, then every other target once.
    /// Single-ally Targets=[ally] still shows caster then ally (with slides).
    /// </summary>
    private void BuildPresentationQueue()
    {
        void Add(Unit unit)
        {
            if (_queue.Any(e => ReferenceEquals(e.Unit, unit)))
            {
                return;
            }

            BattleUnitSpriteSet sprites;
            var fromPresentation = _context.PresentationUnits
                .FirstOrDefault(p => ReferenceEquals(p.Unit, unit));

            if (fromPresentation.Unit != null)
            {
                sprites = fromPresentation.Sprites;
            }
            else if (ReferenceEquals(unit, _context.Caster) && _context.CasterSprites.Idle.Count > 0)
            {
                sprites = _context.CasterSprites;
            }
            else
            {
                sprites = BattleUnitSpriteManager.Get(unit);
            }

            var basePos = GameConstants.Battle.GetSpritePosition(unit);
            sprites.BasePosition = basePos;
            _queue.Add((unit, sprites, basePos));
        }

        Add(_context.Caster);
        foreach (var target in _context.Targets)
        {
            Add(target);
        }
    }

    private void SetDisplayed((Unit Unit, BattleUnitSpriteSet Sprites, Vector2 Base) entry)
    {
        _displayUnit = entry.Unit;
        _displaySprites = entry.Sprites;
        _displayPosition = entry.Base;
    }

    /// <summary>Past the right edge of the logical viewport (256px wide).</summary>
    private static Vector2 OffScreenRight(Vector2 basePos) =>
        new Vector2(GameConstants.Window.Width + 48f, basePos.Y);

    public void Exit()
    {
    }

    public void HandleInput()
    {
    }

    public void Update()
    {
        _delay.Tick();
        _phaseFrame++;

        switch (_phase)
        {
            case Phase.FirstHold:
                if (_phaseFrame >= HoldFrames)
                {
                    if (_queue.Count == 1)
                    {
                        Finish();
                    }
                    else
                    {
                        StartSlideOutToward(_queueIndex + 1, returnToCaster: false);
                    }
                }
                break;

            case Phase.SlideOut:
                UpdateSlide();
                if (_phaseFrame >= SlideFrames)
                {
                    if (_returnToCasterAfterSlideOut)
                    {
                        // Bring caster back from off-screen right.
                        var caster = _queue[0];
                        SetDisplayed(caster);
                        _slideFrom = OffScreenRight(caster.Base);
                        _slideTo = caster.Base;
                        _displayPosition = _slideFrom;
                        BeginPhase(Phase.SlideCasterBack);
                    }
                    else
                    {
                        // Next unit enters from off-screen right.
                        var incoming = _queue[_incomingIndex];
                        SetDisplayed(incoming);
                        _slideFrom = OffScreenRight(incoming.Base);
                        _slideTo = incoming.Base;
                        _displayPosition = _slideFrom;
                        BeginPhase(Phase.SlideIn);
                    }
                }
                break;

            case Phase.SlideIn:
                UpdateSlide();
                if (_phaseFrame >= SlideFrames)
                {
                    _displayPosition = _slideTo;
                    _queueIndex = _incomingIndex;
                    // Effect when this unit finishes sliding into place (visible on screen).
                    TryApplyEffectToDisplayedUnit();
                    BeginPhase(Phase.Hold);
                }
                break;

            case Phase.Hold:
                if (_phaseFrame >= HoldFrames)
                {
                    if (_queueIndex + 1 < _queue.Count)
                    {
                        StartSlideOutToward(_queueIndex + 1, returnToCaster: false);
                    }
                    else if (!ReferenceEquals(_displayUnit, _context.Caster))
                    {
                        StartSlideOutToward(0, returnToCaster: true);
                    }
                    else
                    {
                        Finish();
                    }
                }
                break;

            case Phase.SlideCasterBack:
                UpdateSlide();
                if (_phaseFrame >= SlideFrames)
                {
                    _displayPosition = _slideTo;
                    Finish();
                }
                break;

            case Phase.Done:
                break;
        }
    }

    private void StartSlideOutToward(int incomingIndex, bool returnToCaster)
    {
        _returnToCasterAfterSlideOut = returnToCaster;
        _incomingIndex = incomingIndex;

        var outgoing = _queue[_queueIndex];
        SetDisplayed(outgoing);
        _slideFrom = outgoing.Base;
        _slideTo = OffScreenRight(outgoing.Base);
        _displayPosition = _slideFrom;
        BeginPhase(Phase.SlideOut);

        Logger.Debug(
            $"UseConsumableBattle: slide out {outgoing.Unit.GetDisplayName()} " +
            $"{_slideFrom} → {_slideTo} (returnCaster={returnToCaster}, nextIdx={incomingIndex})");
    }

    private void UpdateSlide()
    {
        var t = Math.Clamp(_phaseFrame / (float)SlideFrames, 0f, 1f);
        _displayPosition = Vector2.Lerp(_slideFrom, _slideTo, Ease(t));
    }

    private void BeginPhase(Phase next)
    {
        _phase = next;
        _phaseFrame = 0;
    }

    /// <summary>
    /// Apply consumable to the on-screen unit if they are in ItemContext.Targets.
    /// (Caster may appear in the queue only for presentation and skip if not a target.)
    /// </summary>
    private void TryApplyEffectToDisplayedUnit()
    {
        if (!_context.Targets.Contains(_displayUnit))
        {
            return;
        }

        if (!_unitsAlreadyAffected.Add(_displayUnit))
        {
            return;
        }

        ItemDatabase.ApplyConsumableToTarget(_itemData, _context.Caster, _displayUnit);
    }

    private void Finish()
    {
        // Remove consumable once after all on-screen applies.
        if (!_itemConsumed)
        {
            _itemConsumed = true;
            ItemDatabase.ConsumeItem(_context.Caster, _context.ItemSlotIndex);
        }

        _phase = Phase.Done;
        GameStateManager.ChangeStateType(GameStateType.ExitBattleScreen);
    }

    private static float Ease(float t) =>
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
