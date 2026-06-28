using System.Numerics;
using Raylib_cs;
using SomberInertia;
using SomberInertia.Timers;
using SomberInertia.State;
using SomberInertia.Core.Units;
using SomberInertia.Core.Graphics;
using SomberInertia.Graphics.UI;

namespace SomberInertia.Core;

public class Game
{
    public Renderer Renderer = new();
    public Grid Grid { get; set; }
    public List<Unit> Units { get; set; } = new();
    public List<Unit> FriendlyUnitsInRange { get; set; } = new();
    public List<Unit> UnfriendlyUnitsInRange { get; set; } = new();
    public FrameFlipper FrameFlipper = new FrameFlipper(GameConfig.Animations.FrameFlipperDelay);
    public MagicUI MagicUI = new MagicUI();
    public bool FirstUnitDiedFromPoison { get; private set; }

    private Vector2 _highlightCurrentPosition;
    private Vector2 _highlightTargetPosition;
    private bool _animationComplete = false;

    public Game(Grid grid)
    {
        Grid = grid;
    }

    public Unit GetCurrentUnit()
    {
        if (Units.Count == 0)
        {
            Logger.Error("Game::GetCurrentUnit(): list of units is empty! Aborting...");
        }

        return Units[0];
    }

    public Unit GetNextUnit()
    {
        if (Units.Count < 2)
        {
            Logger.Warning("Game::GetNextUnit(): list of units is less than two.");
            return Units[0];
        }

        return Units[1];
    }

    public Unit GetLastUnit()
    {
        if (Units.Count == 0)
        {
            Logger.Error("Game::GetLastUnit() list Unit is empty.");
        }

        return Units[^1];
    }

    public void MoveFirstUnitToEndOfList()
    {
        if (Units.Count <= 1)
        {
            Logger.Warning("Game::MoveFirstUnitToEndOfList(): Units list is empty.");
            return;
        }

        var first = Units[0];
        Units.RemoveAt(0);
        Units.Add(first);
    }

    public void SetFirstUnitDiedFromPoison()
    {
        Logger.Info("About to set poison flag to true.");
        FirstUnitDiedFromPoison = true;
    }

    public void ResetFirstUnitDiedFromPoison()
    {
        Logger.Info("About to set poison flag to false.");
        FirstUnitDiedFromPoison = false;
    }

    public void AddUnit(Unit unit, int x, int y)
    {
        if (unit == null)
        {
            Logger.Error("Game::AddUnit(): unit parameter is null; aborting.");
        }

        if (x < 0 || x >= Grid.Width || y < 0 || y >= Grid.Height)
        {
            Logger.Error($"Target position ({x}, {y}) is outside grid bounds.");
        }

        Units.Add(unit);
        Grid.PlaceUnit(unit, x, y);
    }

    public void ResetListOfUnitsInRange()
    {
        FriendlyUnitsInRange.Clear();
        UnfriendlyUnitsInRange.Clear();
    }

    public void SeparateListOfUnitsInRange(Unit currentUnit, List<Unit> unitsInRange)
    {
        Logger.Debug("Game::SeparateListOfUnitsInRange(): resetting FriendlyUnitsInRange and UnfriendlyUnitsInRange lists.");
        ResetListOfUnitsInRange();

        foreach (var unitInRange in unitsInRange)
        {
            if (currentUnit.Friendly == unitInRange.Friendly)
            {
                FriendlyUnitsInRange.Add(unitInRange);
            }
            else
            {
                UnfriendlyUnitsInRange.Add(unitInRange);
            }
        }

        Logger.Info($"FriendlyUnitsInRange.Count = {FriendlyUnitsInRange.Count}; UnfriendlyUnitsInRange.Count = {UnfriendlyUnitsInRange.Count}.");
    }

    public List<Unit> FindAllDeadUnits()
    {
        Logger.Debug("Game::FindAllDeadUnits(): removing all units that have 0 (current) HP.");
        var deadUnits = Units.FindAll(unit => unit.HP.Current == 0);

        Logger.Info($"Number of dead units FOUND: [{deadUnits.Count}].");

        return deadUnits;
    }

    public List<Unit> RemoveAllDeadUnits()
    {
        Logger.Debug("Game::RemoveAllDeadUnits(): removing all units that have 0 (current) HP.");
        var deadUnits = FindAllDeadUnits();
        var count = Units.RemoveAll(unit => unit.HP.Current == 0);

        Grid.RemoveDeadUnitsFromGrid(deadUnits);

        Logger.Info($"Number of dead units REMOVED: [{count}].");

        return deadUnits;
    }

    public void InitializeHighlight()
    {
        var currentUnit = GetCurrentUnit();
        if (currentUnit.Block == null)
        {
            Logger.Error("Game::InitializeHighlight() currentUnit.Block is null.");
        }

        _highlightCurrentPosition = currentUnit.Block.GetPixelCoordinates();
        _highlightTargetPosition = _highlightCurrentPosition;
        _animationComplete = false;
    }

    public Vector2 GetHighlightPosition() => _highlightCurrentPosition;
    public bool IsHighlightSettled() => _animationComplete;

    public void SetHighlightTarget(Unit targetUnit)
    {
        if (targetUnit.Block == null)
        {
            Logger.Error("Game::SetHighlightTarget() targetUnit.Block is null.");
        }

        _highlightTargetPosition = targetUnit.Block.GetPixelCoordinates();
        _animationComplete = false;
    }

    public void UpdateHighlightPosition()
    {
        var distance = Vector2.Distance(_highlightCurrentPosition, _highlightTargetPosition);

        if (distance > 0.1f)
        {
            var direction = Vector2.Normalize(_highlightTargetPosition - _highlightCurrentPosition);
            var moveDistance = GameConstants.HIGHLIGHT_TRANSITION_SPEED * Raylib.GetFrameTime();

            if (moveDistance > distance)
            {
                moveDistance = distance;
            }

            _highlightCurrentPosition += direction * moveDistance;
        }
        else
        {
            _highlightCurrentPosition = _highlightTargetPosition;
            _animationComplete = true;
        }
    }
}