using Raylib_cs;
using SomberInertia.Core;
using SomberInertia.Core.Units;
using SomberInertia.Enums;
using SomberInertia.Graphics;
using SomberInertia.Graphics.UI;

namespace SomberInertia.State;

public class GiveItemToWhom : IGameState
{
    private readonly Game _game;
    private Unit _currentUnit;
    private List<Unit> _friendliesInRange = new();
    private int _currentIndex;

    public GiveItemToWhom(Game game)
    {
        _game = game;
        _currentUnit = _game.GetCurrentUnit();
    }

    public void Enter()
    {
        _currentUnit = _game.GetCurrentUnit();
        _game.Grid.CalculateGiveRange(_currentUnit);

        // Display-only inventory: clear any name-based item blink from GiveWhichItem.
        ItemIcons.ClearSelection();
        ItemIcons.Reset();

        var unitsInRange = _game.Grid.BuildListOfUnitsInRange(_currentUnit);
        _friendliesInRange = unitsInRange
            .Where(u => u.Friendly == _currentUnit.Friendly && u != _currentUnit)
            .ToList();

        if (_friendliesInRange.Count == 0)
        {
            GameStateManager.ShowMessageNotice(
                GameConstants.MessageNotice.NoTarget,
                GameStateType.GiveWhichItem);
            return;
        }

        _currentIndex = 0;
        _game.InitializeHighlight();
        _game.SetHighlightTarget(_friendliesInRange[_currentIndex]);
    }

    public void Exit()
    {
    }

    public void HandleInput()
    {
        if (_friendliesInRange.Count == 0)
        {
            return;
        }

        if (Input.TryCycleIndex(ref _currentIndex, _friendliesInRange.Count))
        {
            var newTarget = _friendliesInRange[_currentIndex];
            if (newTarget.Block != null)
            {
                _game.SetHighlightTarget(newTarget);
            }
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
        var recipient = _friendliesInRange[_currentIndex];
        _game.Give.Recipient = recipient;
        _game.Give.RecipientSlotIndex = -1;

        if (recipient.HasEmptyItemSlot())
        {
            _game.Prompt.Action = PromptAction.GiveItem;
            _game.Prompt.ReturnStateOnYes = GameStateType.EndTurn;
            _game.Prompt.ReturnStateOnNo = GameStateType.GiveItemToWhom;
            GameStateManager.ChangeStateType(GameStateType.PromptYesNo);
        }
        else
        {
            // Full inventory (or only non-empty slots including Unarmed filling all 4): trade.
            // If full of Unarmed-only edge case with no giveable items, trade state will block selection.
            if (!recipient.HasGiveableItem())
            {
                Logger.Warning("GiveItemToWhom: recipient has no empty slots and no giveable items to trade.");
                return;
            }

            GameStateManager.ChangeStateType(GameStateType.TradeWhichItemFromAdjacentNeighbor);
        }
    }

    private void CancelSelection()
    {
        GameStateManager.ChangeStateType(GameStateType.GiveWhichItem);
    }

    public void Update()
    {
        _game.Grid.RangeTint.Tick();
        _game.FlipFlop.Tick();
        ItemIcons.Tick();
        _game.UpdateHighlightPosition();
    }

    public void Draw(float scale)
    {
        _game.Renderer.DrawBackground(scale, _game.Grid);
        _game.Renderer.DrawRange(scale, _game.Grid);
        _game.Renderer.DrawUnits(scale, _game.Grid, _game.Units, _game.FlipFlop.IsOn);
        _game.Renderer.DrawHighlightRectangle(scale, _game.GetHighlightPosition());

        if (_friendliesInRange.Count == 0)
        {
            return;
        }

        var recipient = _friendliesInRange[_currentIndex];
        var inventoryCenter = GameConstants.Give.Positions.RecipientInventoryCenter;
        var infoBoxPos = GameConstants.Give.Positions.RecipientInfoBox;

        foreach (var iconData in _game.ItemUI.GetItemIconsToDrawAt(inventoryCenter, recipient))
        {
            _game.Renderer.DrawItemIcon(scale, iconData.ItemName, iconData.Position, iconData.IsSelected);
        }

        _game.Renderer.DrawUnitInfoBox(scale, recipient, infoBoxPos);
    }
}
