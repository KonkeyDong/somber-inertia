using SomberInertia.Core;
using SomberInertia.Enums;
using SomberInertia.State;
using SomberInertia.Core.Units;
using SomberInertia.Graphics;
using Raylib_cs;

namespace SomberInertia.State;

public class DropItem : IGameState
{
    private Game _game { get; set; }
    private Unit _currentUnit { get; set; }

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

        if (Raylib.IsKeyPressed(KeyboardKey.Z) || Raylib.IsKeyPressed(KeyboardKey.C))
        {
            // GameStateManager.ChangeStateType(GameStateType.BattleItemMenu);
            Logger.Warning("Drop mechanics still not implemented.");
        }

        if (Raylib.IsKeyPressed(KeyboardKey.X))
        {
            GameStateManager.ChangeStateType(GameStateType.BattleItemMenu);
        }
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
            _game.Renderer.DrawItemIcon(scale, iconData.Item.Name, iconData.Position);
        }

        // _game.Renderer.DrawSpellInfoBox(scale, _game.MagicUI.GetSelectedMagic(), _game.MagicUI.GetMagicInformationBoxCoordinates());
    }
}