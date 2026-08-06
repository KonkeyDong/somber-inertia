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
    private readonly FlipFlop _blinker;

    public SelectMagicLevel(Game game)
    {
        _game = game;
        _currentUnit = _game.GetCurrentUnit();
        _blinker = new FlipFlop(GameConstants.Animations.BlinkDelay);
    }

    public void Enter()
    {
        _game.Grid.CalculateMagicAttackRange(_currentUnit, _game.MagicUI.GetSelectedMagicData());
    }

    public void Exit()
    {

    }

    public void HandleInput()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Left))
        {
            _game.MagicUI.PreviousSpellLevel();
            _game.Grid.CalculateMagicAttackRange(_currentUnit, _game.MagicUI.GetSelectedMagicData());
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Right))
        {
            _game.MagicUI.NextSpellLevel();
            _game.Grid.CalculateMagicAttackRange(_currentUnit, _game.MagicUI.GetSelectedMagicData());
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
        var spellData = _game.MagicUI.GetSelectedMagicData();
        Logger.Info(spellData.ToString());
        GameStateManager.ChangeStateType(GameStateType.PrepareMagicTargets);
    }

    private void CancelSelection()
    {
        GameStateManager.ChangeStateType(GameStateType.SelectMagic);
    }

    public void Update()
    {
        _game.Grid.RangeTint.Tick();
        _game.FlipFlop.Tick();
        _blinker.Tick();
        MagicIcons.Tick();
    }

    public void Draw(float scale)
    {
        _game.Renderer.DrawBackground(scale, _game.Grid);
        _game.Renderer.DrawRange(scale, _game.Grid);
        _game.Renderer.DrawUnits(scale, _game.Grid, _game.Units, _game.FlipFlop.IsOn);

        foreach (var iconData in _game.MagicUI.GetMagicIconsToDraw(scale, _currentUnit))
        {
            _game.Renderer.DrawMagicIcon(scale, iconData.Family, iconData.Position);
        }

        _game.Renderer.DrawSpellInfoBox(
            scale,
            _game.MagicUI.GetSelectedMagicData(),
            _game.MagicUI.GetInformationBoxCoordinates(),
            _blinker.IsOn
        );
    }
}