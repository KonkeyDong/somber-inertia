using System.Numerics;
using Raylib_cs;
using SomberInertia.Core;
using SomberInertia.Core.Units;
using SomberInertia.Enums;
using SomberInertia.Graphics;

namespace SomberInertia.State;

public class PromptYesNo : IGameState
{
    private readonly Game _game;
    private readonly Unit _currentUnit;

    private Vector2 _centerPosition;
    private bool _yesSelected = true;

    private const float IconSpacingTiles = 0.75f;

    public PromptYesNo(Game game)
    {
        _game = game;
        _currentUnit = _game.GetCurrentUnit();
    }

    public void Enter()
    {
        _yesSelected = true;
        CommandIcons.SetSelectedIcon(CommandIconType.Yes);
        UpdateCenterPosition();
    }

    public void Exit()
    {
    }

    private bool IsGiveOrTradePrompt()
    {
        return _game.Prompt.Action is PromptAction.GiveItem or PromptAction.TradeItem;
    }

    private void UpdateCenterPosition()
    {
        var yFactor = IsGiveOrTradePrompt()
            ? GameConstants.Give.TradePromptYesNoYFactor
            : 0.75f;

        _centerPosition = new Vector2(
            GameStateManager.CurrentWidth / 2f,
            GameStateManager.CurrentHeight * yFactor
        ) / GameStateManager.CurrentScale;
    }

    public void HandleInput()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Left))
        {
            SetSelection(yes: true);
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Right))
        {
            SetSelection(yes: false);
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

    private void SetSelection(bool yes)
    {
        if (_yesSelected == yes)
        {
            return;
        }

        _yesSelected = yes;
        CommandIcons.SetSelectedIcon(yes ? CommandIconType.Yes : CommandIconType.No);
    }

    private void ConfirmSelection()
    {
        if (_yesSelected)
        {
            OnYes();
        }
        else
        {
            OnNo();
        }
    }

    private void CancelSelection()
    {
        OnNo();
    }

    private void OnYes()
    {
        Logger.Debug($"PromptYesNo: Yes — action [{_game.Prompt.Action}].");

        switch (_game.Prompt.Action)
        {
            case PromptAction.DropItem:
                _currentUnit.RemoveItemAtIndex(_game.Prompt.ItemSlotIndex);
                break;

            case PromptAction.GiveItem:
            {
                var recipient = _game.Give.Recipient;
                if (recipient == null)
                {
                    Logger.Error("PromptYesNo GiveItem: recipient is null.");
                    break;
                }

                _currentUnit.GiveItemTo(recipient, _game.Give.GiverSlotIndex);
                break;
            }

            case PromptAction.TradeItem:
            {
                var recipient = _game.Give.Recipient;
                if (recipient == null)
                {
                    Logger.Error("PromptYesNo TradeItem: recipient is null.");
                    break;
                }

                _currentUnit.SwapItemWith(recipient, _game.Give.GiverSlotIndex, _game.Give.RecipientSlotIndex);
                break;
            }

            case PromptAction.None:
            default:
                Logger.Warning("PromptYesNo: No prompt action set.");
                break;
        }

        var nextState = _game.Prompt.ReturnStateOnYes;
        _game.Prompt.Reset();
        GameStateManager.ChangeStateType(nextState);
    }

    private void OnNo()
    {
        Logger.Debug("PromptYesNo: No selected.");

        var nextState = _game.Prompt.ReturnStateOnNo;
        _game.Prompt.Reset();
        GameStateManager.ChangeStateType(nextState);
    }

    public void Update()
    {
        _game.FrameFlipper.Tick();
        CommandIcons.Tick();
    }

    private void DrawTradeSummary(float scale)
    {
        var recipient = _game.Give.Recipient;
        if (recipient == null)
        {
            Logger.Error("PromptYesNo: cannot draw trade summary; recipient is null.");
            return;
        }

        var giverIndex = _game.Give.GiverSlotIndex;
        if (giverIndex < 0 || giverIndex >= _currentUnit.Items.Length)
        {
            Logger.Error($"PromptYesNo: invalid giver slot [{giverIndex}].");
            return;
        }

        var giverItem = Unit.GetDisplayItemName(_currentUnit.Items[giverIndex].Name);
        ItemName? receiverItem = null;
        var actionLabel = "Give";

        if (_game.Prompt.Action == PromptAction.TradeItem)
        {
            actionLabel = "Swap";
            var receiverIndex = _game.Give.RecipientSlotIndex;
            if (receiverIndex < 0 || receiverIndex >= recipient.Items.Length)
            {
                Logger.Error($"PromptYesNo: invalid receiver slot [{receiverIndex}].");
                return;
            }

            receiverItem = Unit.GetDisplayItemName(recipient.Items[receiverIndex].Name);
        }

        _game.Renderer.DrawTradePromptBox(
            scale,
            actionLabel,
            _currentUnit.GetDisplayName(),
            giverItem,
            recipient.GetDisplayName(),
            receiverItem,
            GameConstants.Give.Positions.TradePromptBox
        );
    }

    public void Draw(float scale)
    {
        _game.Renderer.DrawBackground(scale, _game.Grid);
        _game.Renderer.DrawUnits(scale, _game.Grid, _game.Units, _game.FrameFlipper.IsOn);

        if (IsGiveOrTradePrompt())
        {
            DrawTradeSummary(scale);
        }

        var gap = GameConstants.TILE_SIZE * IconSpacingTiles;

        var yesPosition = new Vector2(_centerPosition.X - gap, _centerPosition.Y);
        var noPosition = new Vector2(_centerPosition.X + gap, _centerPosition.Y);

        _game.Renderer.Draw(scale, CommandIcons.GetSprite(CommandIconType.Yes), yesPosition);
        _game.Renderer.Draw(scale, CommandIcons.GetSprite(CommandIconType.No), noPosition);

        var messagePosition = _centerPosition;
        messagePosition.X += 65;
        messagePosition.Y += 10;

        var label = _yesSelected ? "Yes" : "No";
        _game.Renderer.DrawBattleMenuMessage(scale, label, messagePosition);
    }
}
