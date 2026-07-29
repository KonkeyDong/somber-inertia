using SomberInertia.Core;
using SomberInertia.Enums;
using SomberInertia.State;
using SomberInertia.Core.Units;
using SomberInertia.Graphics;
using Raylib_cs;

namespace SomberInertia.State;

public class DropItem : IGameState
{
    private readonly Game _game;
    private Unit _currentUnit;

    public DropItem(Game game)
    {
        _game = game;
        _currentUnit = _game.GetCurrentUnit();
    }

    public void Enter()
    {
        SetSelectedItem(Direction.Up);
    }

    public void Exit()
    {

    }

    private void SetSelectedItem(Direction direction)
    {
        _game.ItemUI.SetSelected(direction, _currentUnit);
        // _game.Grid.CalculateMagicAttackRange(_currentUnit, _game.MagicUI.GetSelectedMagic());
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
        _game.Prompt.Action = PromptAction.DropItem;
        _game.Prompt.ItemSlotIndex = _game.ItemUI.GetSelectedIndex();
        _game.Prompt.ReturnStateOnNo = GameStateType.DropItem;
        _game.Prompt.ReturnStateOnYes = GameStateType.BattleItemMenu;

        GameStateManager.ChangeStateType(GameStateType.PromptYesNo);
    }

    private void CancelSelection()
    {
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
        _game.Renderer.DrawMagicAttackRange(scale, _game.Grid);
        _game.Renderer.DrawUnits(scale, _game.Grid, _game.Units, _game.FrameFlipper.IsOn);

        foreach (var iconData in _game.ItemUI.GetItemIconsToDraw(scale, _currentUnit))
        {
            _game.Renderer.DrawItemIcon(scale, iconData.ItemName, iconData.Position, iconData.IsSelected);
        }

        _game.Renderer.DrawItemInfoBox(scale, _game.ItemUI.GetSelectedItemData(), _game.ItemUI.IsSelectedItemEquipped(_currentUnit), _game.ItemUI.GetInformationBoxCoordinates());
    }
}