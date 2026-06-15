using SomberInertia.Enums;
using SomberInertia.Core;
using SomberInertia.Core.Units;
using SomberInertia.Graphics;
using System.Numerics;

using Raylib_cs;

namespace SomberInertia.State;

public class SelectMagic : IGameState
{
    private readonly Game _game;
    private readonly Unit _currentUnit;

    public SelectMagic(Game game)
    {
        _game = game;
        _currentUnit = _game.GetCurrentUnit();
    }

    public void Enter()
    {
        SetSelectedMagic(Direction.Up);
    }

    public void Exit()
    {

    }

    public void HandleInput()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Up))
        {
            SetSelectedMagic(Direction.Up);
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Left))
        {
            SetSelectedMagic(Direction.Left);
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Right))
        {
            SetSelectedMagic(Direction.Right);
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Down))
        {
            SetSelectedMagic(Direction.Down);
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Z) || Raylib.IsKeyPressed(KeyboardKey.C))
        {
            var family = _game.MagicUI.GetSelectedFamily();
            var spell = _currentUnit.GetHighestMagicLevelInBucket(family);
            Logger.Info(spell.ToString());
        }

        if (Raylib.IsKeyPressed(KeyboardKey.X))
        {
            CancelMenu();
        }
    }

    private void SetSelectedMagic(Direction direction)
    {
        _game.MagicUI.SetSelected(direction, _currentUnit);
    }

    private void CancelMenu()
    {
        Logger.Debug("SelectMagic(): Cancelled - returning to BattleActionMenu.");
        GameStateManager.ChangeStateType(GameStateType.BattleActionMenu);
    }

    public void Update()
    {
        _game.FrameFlipper.Tick();
        MagicIcons.Update();
    }

    public void Draw(float scale)
    {
        _game.Renderer.DrawBackground(scale, _game.Grid);
        _game.Renderer.DrawUnits(scale, _game.Grid, _game.Units, _game.FrameFlipper.IsOn);

        foreach (var iconData in _game.MagicUI.GetMagicIconsToDraw(scale, _currentUnit))
        {
            _game.Renderer.DrawMagicIcon(scale, iconData.Family, iconData.Position);
        }
    }
}