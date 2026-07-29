using Raylib_cs;
using SomberInertia.Core;
using SomberInertia.Core.Units;
using SomberInertia.Enums;
using SomberInertia.Graphics;

namespace SomberInertia.State;

public class TradeWhichItemFromAdjacentNeighbor : IGameState
{
    private readonly Game _game;
    private Unit _currentUnit;
    private Unit _recipient = null!;

    public TradeWhichItemFromAdjacentNeighbor(Game game)
    {
        _game = game;
        _currentUnit = _game.GetCurrentUnit();
    }

    public void Enter()
    {
        _currentUnit = _game.GetCurrentUnit();
        _recipient = _game.Give.Recipient!;

        if (_recipient == null)
        {
            Logger.Error("TradeWhichItemFromAdjacentNeighbor: Give.Recipient is null. Returning to GiveItemToWhom.");
            GameStateManager.ChangeStateType(GameStateType.GiveItemToWhom);
            return;
        }

        _game.Grid.CalculateGiveRange(_currentUnit);

        _game.ItemUI.Reset();
        _game.ItemUI.SetLayoutCenter(GameConstants.Give.Positions.TradeInventoryCenter);
        _game.ItemUI.SelectFirstGiveableItem(_recipient);

        if (_recipient.Block != null)
        {
            _game.InitializeHighlight();
            _game.SetHighlightTarget(_recipient);
        }
    }

    public void Exit()
    {
    }

    private void SetSelectedItem(Direction direction)
    {
        _game.ItemUI.SetSelected(direction, _recipient);
    }

    public void HandleInput()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Up))
        {
            SetSelectedItem(Direction.Up);
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Left))
        {
            SetSelectedItem(Direction.Left);
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Right))
        {
            SetSelectedItem(Direction.Right);
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Down))
        {
            SetSelectedItem(Direction.Down);
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
        if (!_game.ItemUI.HasValidSelection())
        {
            Logger.Warning("TradeWhichItemFromAdjacentNeighbor: no valid item selected.");
            return;
        }

        _game.Give.RecipientSlotIndex = _game.ItemUI.GetSelectedIndex();

        _game.Prompt.Action = PromptAction.TradeItem;
        _game.Prompt.ReturnStateOnYes = GameStateType.EndTurn;
        _game.Prompt.ReturnStateOnNo = GameStateType.TradeWhichItemFromAdjacentNeighbor;
        GameStateManager.ChangeStateType(GameStateType.PromptYesNo);
    }

    private void CancelSelection()
    {
        _game.ItemUI.Reset();
        _game.ItemUI.ResetLayoutCenter();
        GameStateManager.ChangeStateType(GameStateType.GiveItemToWhom);
    }

    public void Update()
    {
        _game.Grid.RangeTint.Tick();
        _game.FrameFlipper.Tick();
        ItemIcons.Tick();
        _game.UpdateHighlightPosition();
    }

    public void Draw(float scale)
    {
        _game.Renderer.DrawBackground(scale, _game.Grid);
        _game.Renderer.DrawGiveRange(scale, _game.Grid);
        _game.Renderer.DrawUnits(scale, _game.Grid, _game.Units, _game.FrameFlipper.IsOn);
        _game.Renderer.DrawHighlightRectangle(scale, _game.GetHighlightPosition());

        foreach (var iconData in _game.ItemUI.GetItemIconsToDraw(scale, _recipient))
        {
            _game.Renderer.DrawItemIcon(scale, iconData.ItemName, iconData.Position, iconData.IsSelected);
        }

        if (_game.ItemUI.HasValidSelection())
        {
            _game.Renderer.DrawItemInfoBox(
                scale,
                _game.ItemUI.GetSelectedItemData(),
                _game.ItemUI.IsSelectedItemEquipped(_recipient),
                _game.ItemUI.GetInformationBoxCoordinates()
            );
        }
    }
}
