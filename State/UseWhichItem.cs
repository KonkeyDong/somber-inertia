using Raylib_cs;
using SomberInertia.Core;
using SomberInertia.Core.Units;
using SomberInertia.Enums;
using SomberInertia.Graphics;
using SomberInertia.Graphics.UI;

namespace SomberInertia.State;

/// <summary>
/// Item menu → Use: pick an inventory item this unit's job can use.
/// Shows radial icons (unusable slots blanked), info box, and item use range.
/// </summary>
public class UseWhichItem : IGameState
{
    private readonly Game _game;
    private Unit _currentUnit;

    public UseWhichItem(Game game)
    {
        _game = game;
        _currentUnit = _game.GetCurrentUnit();
    }

    public void Enter()
    {
        _currentUnit = _game.GetCurrentUnit();
        _game.ItemUI.Reset();
        _game.ItemUI.ResetLayoutCenter();
        _game.ItemUI.SelectFirstItem(_currentUnit, ItemUI.UsableFilter);

        if (!_game.ItemUI.HasValidSelection(_currentUnit, ItemUI.UsableFilter))
        {
            GameStateManager.ShowMessageNotice(
                GameConstants.MessageNotice.NoItem,
                GameStateType.BattleItemMenu);
            return;
        }

        UpdateRangeForSelection();
    }

    public void Exit()
    {
    }

    private void SetSelectedItem(Direction direction)
    {
        _game.ItemUI.SetSelected(direction, _currentUnit, ItemUI.UsableFilter);
        if (_game.ItemUI.HasValidSelection(_currentUnit, ItemUI.UsableFilter))
        {
            UpdateRangeForSelection();
        }
    }

    private void UpdateRangeForSelection()
    {
        var data = _game.ItemUI.GetSelectedItemData();
        _game.Grid.CalculateItemUseRange(_currentUnit, data);
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
        if (!_game.ItemUI.HasValidSelection(_currentUnit, ItemUI.UsableFilter))
        {
            Logger.Warning("UseWhichItem: no valid usable item selected.");
            return;
        }

        // Next: UseItemOnWhom (target select + apply). Keep selection for that hop.
        _game.Prompt.ItemSlotIndex = _game.ItemUI.GetSelectedIndex();
        Logger.Info(
            $"UseWhichItem: selected slot [{_game.Prompt.ItemSlotIndex}] " +
            $"{_game.ItemUI.GetSelectedItemName()} (target select not implemented yet)."
        );
    }

    private void CancelSelection()
    {
        _game.ItemUI.Reset();
        _game.ItemUI.ResetLayoutCenter();
        _game.Grid.ItemUseRangeSet.Clear();
        GameStateManager.ChangeStateType(GameStateType.BattleItemMenu);
    }

    public void Update()
    {
        _game.Grid.RangeTint.Tick();
        _game.FlipFlop.Tick();
        ItemIcons.Tick();
    }

    public void Draw(float scale)
    {
        _game.Renderer.DrawBackground(scale, _game.Grid);
        _game.Renderer.DrawItemUseRange(scale, _game.Grid);
        _game.Renderer.DrawUnits(scale, _game.Grid, _game.Units, _game.FlipFlop.IsOn);

        foreach (var iconData in _game.ItemUI.GetItemIconsToDraw(
            _currentUnit,
            ItemUI.UsableFilter,
            blankDisallowed: true))
        {
            _game.Renderer.DrawItemIcon(scale, iconData.ItemName, iconData.Position, iconData.IsSelected);
        }

        if (_game.ItemUI.HasValidSelection(_currentUnit, ItemUI.UsableFilter))
        {
            _game.Renderer.DrawItemInfoBox(
                scale,
                _game.ItemUI.GetSelectedItemData(),
                _game.ItemUI.IsSelectedItemEquipped(_currentUnit),
                _game.ItemUI.GetInformationBoxCoordinates()
            );
        }
    }
}
