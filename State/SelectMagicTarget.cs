using SomberInertia.Enums;
using SomberInertia.Core;
using SomberInertia.Core.Units;
using SomberInertia.Graphics;
using System.Numerics;

using Raylib_cs;

namespace SomberInertia.State;

public class SelectMagicTargets : IGameState
{
    private readonly Game _game;
    private readonly Unit _currentUnit;
    private readonly List<Unit> _listOfUnits;
    private int _currentIndex;
    private readonly FrameFlipper _blinker;

    public SelectMagicTargets(Game game)
    {
        _game = game;
        _currentUnit = _game.GetCurrentUnit();
        _blinker = new FrameFlipper(GameConfig.Animations.BlinkDelay);

        var offensive = _game.MagicUI.GetSelectedMagic().Offensive;
        _listOfUnits = offensive ? _game.UnfriendlyUnitsInRange : _game.FriendlyUnitsInRange;
    }

    public void Enter()
    {
        if (_listOfUnits.Count > 0)
        {
            _game.InitializeHighlight();
            _currentIndex = 0;

            _game.SetHighlightTarget(_listOfUnits[_currentIndex]);
        }
    }

    public void Exit()
    {

    }

    public void HandleInput()
    {
        if (_listOfUnits.Count() > 1)
        {
            var changed = false;
            if (Raylib.IsKeyPressed(KeyboardKey.Left))
            {

                _currentIndex = (_currentIndex + 1) % _listOfUnits.Count();
                changed = true;
            }
            if (Raylib.IsKeyPressed(KeyboardKey.Right))
            {
                _currentIndex = (_currentIndex - 1 + _listOfUnits.Count()) % _listOfUnits.Count();
                changed = true;
            }

            if (changed)
            {
                var newTarget = _listOfUnits[_currentIndex];
                if (newTarget.Block != null)
                {
                    _game.SetHighlightTarget(newTarget);
                }
            }
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Z) || Raylib.IsKeyPressed(KeyboardKey.C))
        {
            var spell = _game.MagicUI.GetSelectedMagic();
            Logger.Info(spell.ToString());
            // GameStateManager.ChangeStateType(GameStateType.PrepareMagicTargets);
        }

        if (Raylib.IsKeyPressed(KeyboardKey.X))
        {
            GameStateManager.ChangeStateType(GameStateType.SelectMagicLevel);
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
        _game.UpdateHighlightPosition();
    }

    public void Draw(float scale)
    {
        _game.Renderer.DrawBackground(scale, _game.Grid);
        _game.Renderer.DrawMagicAttackRange(scale, _game.Grid);
        _game.Renderer.DrawUnits(scale, _game.Grid, _game.Units, _game.FrameFlipper.IsOn);

        _game.Renderer.DrawHighlightRectangle(scale, _game.GetHighlightPosition());
    }
}