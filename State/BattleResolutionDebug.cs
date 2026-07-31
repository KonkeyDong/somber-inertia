using System.Numerics;
using Raylib_cs;
using SomberInertia.Core;
using SomberInertia.Enums;
using SomberInertia.Graphics;

namespace SomberInertia.State;

/// <summary>
/// Manual scrubber for battle poses when <see cref="Logger.InDebugMode"/> is on.
/// Left/Right step frames; no auto-play and no damage until confirm exit.
/// </summary>
public class BattleResolutionDebug : IGameState
{
    private readonly Game _game;
    private readonly Sprite _foregroundSprite;
    private int _frameIndex;
    private int _frameLimit;

    public BattleResolutionDebug(Game game)
    {
        _game = game;
        _foregroundSprite = BattleForegrounds.Get(ForegroundNames.RoughTerrain);
    }

    public void Enter()
    {
        _frameIndex = 0;
        _frameLimit = Math.Max(
            _game.AttackContext.ForceMemberSpriteSet.BattleSequence.Count,
            _game.AttackContext.MonsterSpriteSet.BattleSequence.Count
        );

        if (_frameLimit <= 0)
        {
            Logger.Error("BattleResolutionDebug: BattleSequence is empty.");
            _frameLimit = 1;
        }

        Logger.Info(
            $"BattleResolutionDebug: {_frameLimit} scrub frame(s). " +
            $"Force base={_game.AttackContext.ForceMemberSpriteSet.BasePosition}, " +
            $"Monster base={_game.AttackContext.MonsterSpriteSet.BasePosition}. " +
            "←/→ scrub, click logs coords, Z/C apply damage+exit, X exit without damage."
        );
    }

    public void Exit()
    {
    }

    public void HandleInput()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Right))
        {
            _frameIndex = (_frameIndex + 1) % _frameLimit;
            LogFrame();
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Left))
        {
            _frameIndex = (_frameIndex - 1 + _frameLimit) % _frameLimit;
            LogFrame();
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
        Logger.Debug($"BattleResolutionDebug frame {_frameIndex + 1}/{_frameLimit}");
    }

    public void Update()
    {
        // Manual scrub only — no automatic frame advance.
    }

    public void Draw(float scale)
    {
        Raylib.ClearBackground(Color.Black);

        // Layer: background → grid → foreground → character sprites → HUD
        var background = BattleBackgrounds.Get(BackgroundNames.Battle01);
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

        if (forceSet.BattleSequence.Count > 0)
        {
            var forceFrame = forceSet.GetBattleSequenceFrame(_frameIndex);
            _game.Renderer.Draw(scale, forceFrame, forceSet.BasePosition);
        }

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

        var label = $"frame {_frameIndex + 1}/{_frameLimit}";
        Raylib.DrawText(label, 8, 8, GameConstants.Debug.FontSize, GameConstants.Debug.Color);
    }
}
