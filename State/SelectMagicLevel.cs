using SomberInertia.Enums;
using SomberInertia.Core;
using SomberInertia.Core.Units;
using SomberInertia.Timers;
using SomberInertia.Graphics;
using System.Numerics;

using Raylib_cs;

namespace SomberInertia.State;

public class SelectMagicLevel : IGameState
{
    private readonly Game _game;
    private readonly Unit _currentUnit;
    private readonly FrameFlipper _blinker;

    public SelectMagicLevel(Game game)
    {
        _game = game;
        _currentUnit = _game.GetCurrentUnit();
        _blinker = new FrameFlipper(GameConfig.Animations.BlinkDelay);
    }

    public void Enter()
    {

    }

    public void Exit()
    {

    }

    public void HandleInput()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Left))
        {
            _game.MagicUI.PreviousSpellLevel();
            _game.Grid.CalculateMagicAttackRange(_currentUnit, _game.MagicUI.GetSelectedMagic());
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Right))
        {
            _game.MagicUI.NextSpellLevel();
            _game.Grid.CalculateMagicAttackRange(_currentUnit, _game.MagicUI.GetSelectedMagic());
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Z) || Raylib.IsKeyPressed(KeyboardKey.C))
        {
            var spell = _game.MagicUI.GetSelectedMagic();
            Logger.Info(spell.ToString());
            GameStateManager.ChangeStateType(GameStateType.PrepareMagicTargets);
        }

        if (Raylib.IsKeyPressed(KeyboardKey.X))
        {
            GameStateManager.ChangeStateType(GameStateType.SelectMagic);
        }
    }

    private void CancelMenu()
    {
        Logger.Debug("SelectMagic(): Cancelled - returning to BattleActionMenu.");
        GameStateManager.ChangeStateType(GameStateType.BattleActionMenu);
    }

    public void Update()
    {
        _game.Grid.RangeTint.Tick();
        _game.FrameFlipper.Tick();
        _blinker.Tick();
        MagicIcons.Tick();
    }

    public void Draw(float scale)
    {
        _game.Renderer.DrawBackground(scale, _game.Grid);
        _game.Renderer.DrawMagicAttackRange(scale, _game.Grid);
        _game.Renderer.DrawUnits(scale, _game.Grid, _game.Units, _game.FrameFlipper.IsOn);

        foreach (var iconData in _game.MagicUI.GetMagicIconsToDraw(scale, _currentUnit))
        {
            _game.Renderer.DrawMagicIcon(scale, iconData.Family, iconData.Position);
        }

        _game.Renderer.DrawSpellInfoBox(scale, _game.MagicUI.GetSelectedMagic(), _game.MagicUI.GetMagicInformationBoxCoordinates(), _blinker.IsOn);
    }
}