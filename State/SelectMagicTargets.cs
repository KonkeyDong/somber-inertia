using SomberInertia.Enums;
using SomberInertia.Core;
using SomberInertia.Core.Combat;
using SomberInertia.Core.Combat.Spells;
using SomberInertia.Core.Units;
using SomberInertia.Graphics;
using System.Numerics;

namespace SomberInertia.State;

public class SelectMagicTargets : IGameState
{
    private readonly Game _game;
    private readonly Unit _currentUnit;
    private readonly List<Unit> _listOfUnits;
    private int _currentIndex;
    private MagicContext _magicContext = null!;
    private List<Block> _areaOfEffect = null!;

    public SelectMagicTargets(Game game)
    {
        _game = game;
        _currentUnit = _game.GetCurrentUnit();

        var offensive = _game.MagicUI.GetSelectedMagicData().Offensive;
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
        if (Input.TryCycleIndex(ref _currentIndex, _listOfUnits.Count))
        {
            var newTarget = _listOfUnits[_currentIndex];
            if (newTarget.Block != null)
            {
                _game.SetHighlightTarget(newTarget);
                SetMagicContext();
            }
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
        Logger.Info(_magicContext.ToString());

        var spellName = _game.MagicUI.GetSelectedMagicName();
        MagicDatabase.Cast(spellName, _magicContext);

        GameStateManager.ChangeStateType(GameStateType.AnimateUnitDeaths);
    }

    private void CancelSelection()
    {
        GameStateManager.ChangeStateType(GameStateType.SelectMagicLevel);
    }

    private void SetMagicContext()
    {
        var selectedUnit = _listOfUnits[_currentIndex];
        var spellData = _game.MagicUI.GetSelectedMagicData();

        // Fills RangeSet with AoE around the selected target (cast range refilled on magic select).
        _game.Grid.CalculateSpellEffectRange(selectedUnit, spellData);

        var unitsInRange = _game.Grid.BuildListOfUnitsInRange(selectedUnit);
        _magicContext = new MagicContext(_currentUnit, unitsInRange, _game.Grid);

        _areaOfEffect = _game.Grid.GetBlocksFromRangeSet();
    }

    public void Update()
    {
        _game.FlipFlop.Tick();
        _game.Grid.RangeTint.Tick();
        _game.UpdateHighlightPosition();
    }

    public void Draw(float scale)
    {
        _game.Renderer.DrawBackground(scale, _game.Grid);
        _game.Renderer.DrawRange(scale, _game.Grid);
        _game.Renderer.DrawUnits(scale, _game.Grid, _game.Units, _game.FlipFlop.IsOn);

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