using Raylib_cs;
using SomberInertia.Core;
using SomberInertia.Enums;
using SomberInertia.Timers;

namespace SomberInertia.State;

/// <summary>
/// Temporary notice: show a primed message, then return to a primed state
/// on dismiss (Z/X/C) or countdown.
/// </summary>
public class MessageNotice : IGameState
{
    private readonly Game _game;
    private readonly CountdownTimer _countdownTimer = new(GameConstants.Animations.SwitchStateCountdownTimer);
    private GameStateType _returnState;
    private string _message = "";

    public MessageNotice(Game game)
    {
        _game = game;
    }

    public void Enter()
    {
        _countdownTimer.Reset();
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
        _game.FlipFlop.Tick();

        _countdownTimer.Tick();

        if (!_countdownTimer.IsActive)
        {
            Logger.Info("MessageNotice: countdown exhausted.");
            ConfirmSelection();
        }
    }

    public void Draw(float scale)
    {
        _game.Renderer.DrawBackground(scale, _game.Grid);

        // Draw range left by the previous state (empty = no-op).
        _game.Renderer.DrawRange(scale, _game.Grid);

        _game.Renderer.DrawUnits(scale, _game.Grid, _game.Units, _game.FlipFlop.IsOn);

        var position = GameConstants.WorldMap.Positions.NoTargetMessageBox;
        _game.Renderer.DrawBattleMenuMessage(scale, _message, position);
    }
}
