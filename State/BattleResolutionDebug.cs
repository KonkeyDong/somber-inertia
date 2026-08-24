using System.Numerics;
using Raylib_cs;
using SomberInertia.Core;
using SomberInertia.Enums;
using SomberInertia.Graphics;

namespace SomberInertia.State;

/// <summary>
/// Manual scrubber for battle poses when <see cref="Logger.InDebugMode"/> is on.
/// Left/Right step frames; no auto-play and no damage until confirm exit.
/// Supports Artillery explosion FX scrubbed to the current frame.
/// </summary>
public class BattleResolutionDebug : IGameState
{
    private readonly Game _game;
    private readonly Sprite _foregroundSprite;
    private int _frameIndex;
    private int _frameLimit;

    private SequenceTimerSlot[] _delayEffects = Array.Empty<SequenceTimerSlot>();
    private List<Sprite> _artilleryExplosions = new();

    public BattleResolutionDebug(Game game)
    {
        _game = game;
        _foregroundSprite = BattleForegrounds.Get(ForegroundNames.RoughTerrain);
    }

    public void Enter()
    {
        _frameIndex = 0;
        _delayEffects = Array.Empty<SequenceTimerSlot>();
        _artilleryExplosions = new List<Sprite>();

        _frameLimit = Math.Max(
            _game.AttackContext.ForceMemberSpriteSet.BattleSequence.Count,
            _game.AttackContext.MonsterSpriteSet.BattleSequence.Count
        );

        if (_game.AttackContext.Effect == Effects.ArtilleryExplosion)
        {
            _artilleryExplosions = ArtilleryExplosion.Frames;
            _delayEffects = ArtilleryBattleEffects.CreateSlots(
                _game.AttackContext.ForceMemberSpriteSet);
            _frameLimit = Math.Max(_frameLimit, ArtilleryBattleEffects.MaxDuration(_delayEffects));
            ArtilleryBattleEffects.SeekAll(_delayEffects, _frameIndex);
        }

        if (_frameLimit <= 0)
        {
            Logger.Error("BattleResolutionDebug: BattleSequence is empty.");
            _frameLimit = 1;
        }

        Logger.Info(
            $"BattleResolutionDebug: {_frameLimit} scrub frame(s). " +
            $"Effect={_game.AttackContext.Effect}, DamageApplyFrame={_game.AttackContext.DamageApplyFrame}. " +
            $"Force base={_game.AttackContext.ForceMemberSpriteSet.BasePosition}, " +
            $"Monster base={_game.AttackContext.MonsterSpriteSet.BasePosition}. " +
            "←/→ scrub, click logs coords, Z/C apply damage+exit, X exit without damage."
        );
    }

    public void Exit()
    {
        _delayEffects = Array.Empty<SequenceTimerSlot>();
    }

    public void HandleInput()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Right))
        {
            _frameIndex = (_frameIndex + 1) % _frameLimit;
            OnScrubbed();
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Left))
        {
            _frameIndex = (_frameIndex - 1 + _frameLimit) % _frameLimit;
            OnScrubbed();
        }

        if (Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            var screen = Raylib.GetMousePosition();
            var logical = screen / GameStateManager.CurrentScale;
            Logger.Info(
                $"Mouse click screen=({screen.X:F0},{screen.Y:F0}) " +
                $"logical=({logical.X:F1},{logical.Y:F1})"
            );
        }

        if (Input.IsConfirmPressed())
        {
            ConfirmSelection();
        }

        if (Input.IsCancelPressed())
        {
            CancelSelection();
        }
    }

    private void OnScrubbed()
    {
        ArtilleryBattleEffects.SeekAll(_delayEffects, _frameIndex);
        LogFrame();
    }

    private void ConfirmSelection()
    {
        // Apply pending combat outcome once when leaving scrub intentionally.
        if (_game.AttackContext.Hit)
        {
            _game.AttackContext.Defender.TakeDamage(_game.AttackContext.Damage);
        }

        GameStateManager.ChangeStateType(GameStateType.ExitBattleScreen);
    }

    private void CancelSelection()
    {
        // Leave without applying damage so positioning tests are non-destructive.
        GameStateManager.ChangeStateType(GameStateType.ExitBattleScreen);
    }

    private void LogFrame()
    {
        var damageMark = _frameIndex == _game.AttackContext.DamageApplyFrame ? " [DAMAGE]" : "";
        Logger.Debug($"BattleResolutionDebug frame {_frameIndex + 1}/{_frameLimit}{damageMark}");
    }

    public void Update()
    {
        // Manual scrub only — no automatic frame advance / effect Tick.
        // Effects are Seek()'d to _frameIndex on scrub.
    }

    public void Draw(float scale)
    {
        Raylib.ClearBackground(Color.Black);

        // Layer: background → grid → foreground → monster → FX under → force → FX over → HUD
        var background = BattleBackgrounds.Get(BackgroundNames.GatesOfGuardiana);
        _game.Renderer.Draw(scale, background, GameConstants.Battle.Positions.Background);

        _game.Renderer.DrawDebugLogicalGrid(
            scale,
            GameConstants.Debug.BattleGridLogicalSpacing,
            GameConstants.Debug.BattleGridColor
        );

        _game.Renderer.Draw(scale, _foregroundSprite, GameConstants.Battle.Positions.Foreground);

        var monsterSet = _game.AttackContext.MonsterSpriteSet;
        var forceSet = _game.AttackContext.ForceMemberSpriteSet;

        if (monsterSet.BattleSequence.Count > 0)
        {
            var monsterFrame = monsterSet.GetBattleSequenceFrame(_frameIndex);
            _game.Renderer.Draw(scale, monsterFrame, monsterSet.BasePosition);
        }

        ArtilleryBattleEffects.DrawRange(scale, _game.Renderer, _delayEffects, _artilleryExplosions, 0, 3);

        if (forceSet.BattleSequence.Count > 0)
        {
            var forceFrame = forceSet.GetBattleSequenceFrame(_frameIndex);
            _game.Renderer.Draw(scale, forceFrame, forceSet.BasePosition);
        }

        ArtilleryBattleEffects.DrawRange(scale, _game.Renderer, _delayEffects, _artilleryExplosions, 3, 7);

        _game.Renderer.DrawUnitInfoBox(
            scale,
            _game.AttackContext.GetMonster(),
            GameConstants.Battle.Positions.UnfriendlyStats
        );
        _game.Renderer.DrawUnitInfoBox(
            scale,
            _game.AttackContext.GetForceMember(),
            GameConstants.Battle.Positions.FriendlyStats
        );

        var damageMark = _frameIndex == _game.AttackContext.DamageApplyFrame ? " DMG" : "";
        var label = $"frame {_frameIndex + 1}/{_frameLimit}{damageMark}  {_game.AttackContext.Effect}";
        Raylib.DrawText(label, 8, 8, GameConstants.Debug.FontSize, GameConstants.Debug.Color);
    }
}
