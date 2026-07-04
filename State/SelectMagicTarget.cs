using SomberInertia.Enums;
using SomberInertia.Core;
using SomberInertia.Core.Combat;
using SomberInertia.Core.Combat.Spells;
using SomberInertia.Core.Units;
using SomberInertia.Timers;
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
    private MagicContext _magicContext = null!;
    private List<Block> _areaOfEffect = null!;

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
            SetMagicContext();
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
                    SetMagicContext();
                }
            }
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Z) || Raylib.IsKeyPressed(KeyboardKey.C))
        {
            Logger.Info(_magicContext.ToString());
            _game.MagicUI.GetSelectedMagic().Cast(_magicContext);
            GameStateManager.ChangeStateType(GameStateType.AnimateUnitDeaths);
        }

        if (Raylib.IsKeyPressed(KeyboardKey.X))
        {
            GameStateManager.ChangeStateType(GameStateType.SelectMagicLevel);
        }
    }

    private void SetMagicContext()
    {
        var selectedUnit = _listOfUnits[_currentIndex];
        _game.Grid.CalculateSpellEffectRange(selectedUnit, _game.MagicUI.GetSelectedMagic());

        var unitsInRange = _game.Grid.BuildListOfUnitsInSpellEffectRange(selectedUnit);
        _magicContext = new MagicContext(_currentUnit, unitsInRange, _game.Grid);

        _areaOfEffect = _game.Grid.GetBlocksFromRangeSet(_game.Grid.SpellEffectRangeSet);
    }

    private void CancelMenu()
    {
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

        if (_game.IsHighlightSettled())
        {
            foreach (var block in _areaOfEffect)
            {
                _game.Renderer.DrawHighlightRectangle(scale, block.GetPixelCoordinates());
            }
        }
        else
        {
            _game.Renderer.DrawHighlightRectangle(scale, _game.GetHighlightPosition());
        }
    }
}