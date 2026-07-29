using Raylib_cs;
using SomberInertia.Core;
using SomberInertia.Core.Units;
using SomberInertia.Enums;
using SomberInertia.Graphics;

namespace SomberInertia.State;

public class GiveWhichItem : IGameState
{
    private readonly Game _game;
    private Unit _currentUnit;

    public GiveWhichItem(Game game)
    {
        _game = game;
        _currentUnit = _game.GetCurrentUnit();
    }

    public void Enter()
    {
        _currentUnit = _game.GetCurrentUnit();

        // Fill give range first — BuildListOfUnitsInGiveRange only reads GiveRangeSet.
        _game.Grid.CalculateGiveRange(_currentUnit);

        var units = _game.Grid.BuildListOfUnitsInGiveRange(_currentUnit);
        _game.SeparateListOfUnitsInRange(_currentUnit, units);

        if (_game.FriendlyUnitsInRange.Count == 0)
        {
            // GiveRangeSet is filled; MessageNotice draws + pulses it.
            GameStateManager.ShowMessageNotice(
                GameConstants.MessageNotice.NoTarget,
                GameStateType.BattleItemMenu);
            return;
        }

        _game.ItemUI.Reset();
        _game.ItemUI.ResetLayoutCenter();
        _game.ItemUI.SelectFirstGiveableItem(_currentUnit);
    }

    public void Exit()
    {
    }

    private void SetSelectedItem(Direction direction)
    {
        _game.ItemUI.SetSelected(direction, _currentUnit);
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
            Logger.Warning("GiveWhichItem: no valid item selected.");
            return;
        }

        _game.Give.GiverSlotIndex = _game.ItemUI.GetSelectedIndex();
        _game.Give.Recipient = null;
        _game.Give.RecipientSlotIndex = -1;

        GameStateManager.ChangeStateType(GameStateType.GiveItemToWhom);
    }

    private void CancelSelection()
    {
        _game.ItemUI.Reset();
        _game.ItemUI.ResetLayoutCenter();
        GameStateManager.ChangeStateType(GameStateType.BattleItemMenu);
    }

    public void Update()
    {
        _game.Grid.RangeTint.Tick();
        _game.FrameFlipper.Tick();
        ItemIcons.Tick();
    }

    public void Draw(float scale)
    {
        _game.Renderer.DrawBackground(scale, _game.Grid);
        _game.Renderer.DrawGiveRange(scale, _game.Grid);
        _game.Renderer.DrawUnits(scale, _game.Grid, _game.Units, _game.FrameFlipper.IsOn);

        foreach (var iconData in _game.ItemUI.GetItemIconsToDraw(scale, _currentUnit))
        {
            _game.Renderer.DrawItemIcon(scale, iconData.ItemName, iconData.Position, iconData.IsSelected);
        }

        if (_game.ItemUI.HasValidSelection())
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
