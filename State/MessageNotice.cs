using Raylib_cs;
using SomberInertia.Core;
using SomberInertia.Enums;

namespace SomberInertia.State;

/// <summary>
/// Temporary notice: show a primed message, then return to a primed state
/// on dismiss (Z/X/C) or countdown.
/// </summary>
public class MessageNotice : IGameState
{
    private readonly Game _game;
    private int _countdownTimer = GameConstants.Animations.SwitchStateCountdownTimer;
    private GameStateType _returnState;
    private string _message = "";

    public MessageNotice(Game game)
    {
        _game = game;
    }

    public void Enter()
    {
        _countdownTimer = GameConstants.Animations.SwitchStateCountdownTimer;
        _message = _game.MessageNotice.Message;
        _returnState = _game.MessageNotice.ReturnState;

        if (string.IsNullOrEmpty(_message))
        {
            Logger.Warning("MessageNotice: Message was empty. Returning immediately.");
            GameStateManager.ChangeStateType(_returnState);
            return;
        }

        _game.Grid.RangeTint.Reset();

        Logger.Info($"MessageNotice: \"{_message}\" → return to [{_returnState}].");
    }

    public void Exit()
    {
        _game.MessageNotice.Reset();
    }

    public void HandleInput()
    {
        // Z, C, or X all dismiss the notice
        if (Input.IsDismissPressed())
        {
            ConfirmSelection();
        }
    }

    private void ConfirmSelection()
    {
        GameStateManager.ChangeStateType(_returnState);
    }

    public void Update()
    {
        _game.Grid.RangeTint.Tick();
        _game.FrameFlipper.Tick();

        _countdownTimer--;

        if (_countdownTimer <= 0)
        {
            Logger.Info("MessageNotice: countdown exhausted.");
            ConfirmSelection();
        }
    }

    public void Draw(float scale)
    {
        _game.Renderer.DrawBackground(scale, _game.Grid);

        // Draw any range sets already filled by the previous state (empty = no-op).
        _game.Renderer.DrawWeaponAttackRange(scale, _game.Grid);
        _game.Renderer.DrawMagicAttackRange(scale, _game.Grid);
        _game.Renderer.DrawGiveRange(scale, _game.Grid);
        _game.Renderer.DrawItemUseRange(scale, _game.Grid);

        _game.Renderer.DrawUnits(scale, _game.Grid, _game.Units, _game.FrameFlipper.IsOn);

        var position = GameConstants.WorldMap.Positions.NoTargetMessageBox;
        _game.Renderer.DrawBattleMenuMessage(scale, _message, position);
    }
}
