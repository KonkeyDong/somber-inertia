using SomberInertia.Core;
using SomberInertia.Core.Combat.Item;
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
    private readonly Delay _delay;

    private float _progress = 0f;
    private const float Duration = 60; // total frames for the transition

    private Vector2 _startUnfriendlyPosition;
    private Vector2 _startFriendlyPosition;
    private Vector2 _startForegroundPosition;
    private Vector2 _friendlyBase;
    private bool _itemMode;

    public EnterBattleScreen(Game game)
    {
        _game = game;
        _delay = new Delay(GameConstants.Animations.IdleDelay);

        _foregroundSprite = BattleForegrounds.Get(ForegroundNames.RoughTerrain);
    }

    public void Enter()
    {
        _itemMode = _game.BattleScreenMode == BattleScreenMode.ItemConsumable;
        _progress = 0f;

        var targetForeground = GameConstants.Battle.Positions.Foreground;

        if (_itemMode)
        {
            if (_game.ItemContext == null)
            {
                Logger.Error("EnterBattleScreen (item): ItemContext null.");
                GameStateManager.ChangeStateType(GameStateType.EndTurn);
                return;
            }

            _friendlyBase = GameConstants.Battle.GetSpritePosition(_game.ItemContext.Caster);
            _game.ItemContext.CasterSprites.BasePosition = _friendlyBase;
            _startFriendlyPosition = new Vector2(_friendlyBase.X + 140, _friendlyBase.Y);
            _startForegroundPosition = new Vector2(targetForeground.X + 100, targetForeground.Y);
            _startUnfriendlyPosition = Vector2.Zero;
        }
        else
        {
            var targetUnfriendly = _game.AttackContext.MonsterSpriteSet.BasePosition;
            var targetFriendly = _game.AttackContext.ForceMemberSpriteSet.BasePosition;
            _friendlyBase = targetFriendly;

            _startUnfriendlyPosition = new Vector2(targetUnfriendly.X - 140, targetUnfriendly.Y);
            _startFriendlyPosition = new Vector2(targetFriendly.X + 140, targetFriendly.Y);
            _startForegroundPosition = new Vector2(targetForeground.X + 100, targetForeground.Y);
        }
    }

    public void Exit()
    {
    }

    public void HandleInput()
    {
    }

    public void Update()
    {
        _delay.Tick();

        if (_progress < 1f)
        {
            _progress += 1f / Duration;
            _progress = Math.Min(1f, _progress);
        }
        else
        {
            if (_itemMode)
            {
                GameStateManager.ChangeStateType(GameStateType.UseConsumableBattle);
                return;
            }

            if (Logger.InDebugMode())
            {
                GameStateManager.ChangeStateType(GameStateType.BattleResolutionDebug);
            }
            else
            {
                GameStateManager.ChangeStateType(GameStateType.BattleResolution);
            }
        }
    }

    public void Draw(float scale)
    {
        Raylib.ClearBackground(Color.Black);

        var eased = _game.Renderer.EaseInOut(_progress);
        var frameIndex = _delay.CurrentIndex;

        // Phase 1: Fade out world map (0.0 -> 0.5)
        if (_progress < 0.5f)
        {
            var mapAlpha = (byte)(255 * (1f - eased * 2));

            _game.Renderer.DrawBackground(scale, _game.Grid, mapAlpha);
            _game.Renderer.DrawUnits(scale, _game.Grid, _game.Units, _game.FlipFlop.IsOn, mapAlpha);
        }
        // Phase 2: Fade in battle screen + slide sprites (0.5 -> 1.0)
        else
        {
            var battleAlpha = (byte)(255 * ((eased - 0.5f) * 2));

            var backgroundPosition = GameConstants.Battle.Positions.Background;
            var friendlyStatsPosition = GameConstants.Battle.Positions.FriendlyStats;
            var foregroundPosition = Vector2.Lerp(_startForegroundPosition, GameConstants.Battle.Positions.Foreground, eased);
            var friendlyPosition = Vector2.Lerp(_startFriendlyPosition, _friendlyBase, eased);

            var background = BattleBackgrounds.Get(BackgroundNames.GatesOfGuardiana);
            _game.Renderer.Draw(scale, background, backgroundPosition, battleAlpha);

            if (_itemMode)
            {
                var ctx = _game.ItemContext!;
                _game.Renderer.DrawUnitInfoBox(scale, ctx.Caster, friendlyStatsPosition, battleAlpha);
                _game.Renderer.Draw(scale, _foregroundSprite, foregroundPosition, battleAlpha);
                _game.Renderer.Draw(scale, ctx.CasterSprites.GetIdleFrame(frameIndex), friendlyPosition, battleAlpha);
            }
            else
            {
                var unfriendlyStatsPosition = GameConstants.Battle.Positions.UnfriendlyStats;
                var unfriendlyPosition = Vector2.Lerp(
                    _startUnfriendlyPosition,
                    _game.AttackContext.MonsterSpriteSet.BasePosition,
                    eased);

                _game.Renderer.DrawUnitInfoBox(scale, _game.AttackContext.GetMonster(), unfriendlyStatsPosition, battleAlpha);
                _game.Renderer.DrawUnitInfoBox(scale, _game.AttackContext.GetForceMember(), friendlyStatsPosition, battleAlpha);

                _game.Renderer.Draw(scale, _game.AttackContext.MonsterSpriteSet.GetIdleFrame(frameIndex), unfriendlyPosition, battleAlpha);
                _game.Renderer.Draw(scale, _foregroundSprite, foregroundPosition, battleAlpha);
                _game.Renderer.Draw(scale, _game.AttackContext.ForceMemberSpriteSet.GetIdleFrame(frameIndex), friendlyPosition, battleAlpha);
            }
        }
    }
}
