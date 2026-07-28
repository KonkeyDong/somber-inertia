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

        var unitsInRange = _game.Grid.BuildListOfUnitsInGiveRange(_currentUnit);
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

        if (_friendliesInRange.Count > 1)
        {
            var changed = false;

            if (Raylib.IsKeyPressed(KeyboardKey.Left))
            {
                _currentIndex = (_currentIndex + 1) % _friendliesInRange.Count;
                changed = true;
            }

            if (Raylib.IsKeyPressed(KeyboardKey.Right))
            {
                _currentIndex = (_currentIndex - 1 + _friendliesInRange.Count) % _friendliesInRange.Count;
                changed = true;
            }

            if (changed)
            {
                var newTarget = _friendliesInRange[_currentIndex];
                if (newTarget.Block != null)
                {
                    _game.SetHighlightTarget(newTarget);
                }
            }
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Z) || Raylib.IsKeyPressed(KeyboardKey.C))
        {
            ConfirmRecipient();
        }

        if (Raylib.IsKeyPressed(KeyboardKey.X))
        {
            GameStateManager.ChangeStateType(GameStateType.GiveWhichItem);
        }
    }

    private void ConfirmRecipient()
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
