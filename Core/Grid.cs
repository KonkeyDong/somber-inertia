using SomberInertia;
using SomberInertia.Enums;
using SomberInertia.Timers;
using SomberInertia.Core.Units;
using SomberInertia.Core.Combat;
using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace SomberInertia.Core;

public class Grid
{
    public int Width { get; private set; }
    public int Height { get; private set; }

    public readonly Block[,] Blocks;

    public int BlockSize { get; set; } = (int)(GameConstants.TILE_SIZE * GameConstants.BASE_WINDOW_SCALE);

    private static readonly Dictionary<MovementType, Dictionary<TerrainType, int>> _movementCostsMap;
    public HashSet<(int x, int y)> MovementRangeSet { get; private set; } = new HashSet<(int x, int y)>();
    public HashSet<(int x, int y)> WeaponAttackRangeSet { get; private set; } = new HashSet<(int x, int y)>();
    public HashSet<(int x, int y)> MagicAttackRangeSet { get; private set; } = new HashSet<(int x, int y)>();
    public HashSet<(int x, int y)> SpellEffectRangeSet { get; private set; } = new HashSet<(int x, int y)>();
    public readonly RangeTint RangeTint = new RangeTint(GameConfig.Animations.RangeTintFrameDelay);

    // Static constructor will create the movement cost dictionary only once when Grid is first accessed.
    static Grid()
    {
        _movementCostsMap = new Dictionary<MovementType, Dictionary<TerrainType, int>>
        {
            [MovementType.Warrior] = new Dictionary<TerrainType, int>
            {
                { TerrainType.Road, 1 },
                { TerrainType.Plains, 1 },
                { TerrainType.Overgrowth, 1 },
                { TerrainType.Forest, 2 },
                { TerrainType.Hill, 3 },
                { TerrainType.Sand, 2 }
            },
        };
    }

    public Grid(int width, int height)
    {
        Width = width;
        Height = height;

        Logger.Info($"Creating {Width}x{Height} grid...");

        Blocks = new Block[Width, Height];

        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Height; y++)
            {
                Blocks[x, y] = new Block("Assets/grass_tile.png", TerrainType.Plains, x, y);
            }
        }

        var tempCoords = new (int x, int y)[2]
        {
            (0, 2),
            (0, 1)
        };
        foreach (var point in tempCoords)
        {
            Blocks[point.x, point.y] = new Block("Assets/forest_tile.png", TerrainType.Forest, point.x, point.y);
        }


        Logger.Info("Grid initialization complete.");
    }

    public void ResetAllRangeSets()
    {
        MovementRangeSet.Clear();
        WeaponAttackRangeSet.Clear();
        MagicAttackRangeSet.Clear();
        SpellEffectRangeSet.Clear();
    }

    public void CalculateUnitMovementRange(Unit unit)
    {
        if (unit == null)
        {
            Logger.Error("CalculateUnitMovementRange() unit is null.");
        }

        if (unit.Block == null)
        {
            Logger.Error($"Unit {unit.Name} does not contain a block.");
        }

        MovementRangeSet.Clear();
        var costToReach = new Dictionary<(int x, int y), int>();

        var queue = new Queue<Block>();
        var start = unit.Block;

        MovementRangeSet.Add((start.X, start.Y));
        costToReach[(start.X, start.Y)] = 0;
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            var currentCost = costToReach[(current.X, current.Y)];

            foreach (var neighbor in GetAdjacentBlocks(current))
            {
                if (neighbor == null)
                {
                    continue;
                }

                var coord = (neighbor.X, neighbor.Y);
                if (MovementRangeSet.Contains(coord))
                {
                    continue;
                }

                var occupant = neighbor.PeekOccupant();
                var enterCost = occupant != null && unit.Friendly != occupant.Friendly
                    ? GameConstants.MAX_MOVEMENT_COST
                    : CalculateTerrainTypeCost(unit.MovementType, neighbor.TerrainType);

                var totalCost = currentCost + enterCost;

                if (totalCost <= unit.Movement)
                {
                    MovementRangeSet.Add(coord);
                    costToReach[coord] = totalCost;
                    queue.Enqueue(neighbor);
                }
            }
        }
    }

    public void CalculateWeaponAttackRange(Unit unit)
    {
        if (unit?.Block == null || unit.Weapon == null)
        {
            return;
        }

        WeaponAttackRangeSet = CalculateEffectDistanceRange(unit, unit.Weapon.DistanceRange);
    }

    public void CalculateMagicAttackRange(Unit unit, Magic magic)
    {
        if (unit?.Block == null)
        {
            return;
        }

        MagicAttackRangeSet = CalculateEffectDistanceRange(unit, magic.DistanceRange);
    }

    // Similar to CalculateMagicAttackRange(), but sets the SpellEffectRangeSet.
    // Used for when you cast an AOE spell on a unit and you need to get all units
    // at that unit's location, not the caster's location.
    public void CalculateSpellEffectRange(Unit unit, Magic magic)
    {
        if (unit?.Block == null)
        {
            return;
        }

        SpellEffectRangeSet = CalculateEffectDistanceRange(unit, magic.TargetRange);
    }

    // Effect meaning magical or weapon attack.
    private HashSet<(int, int)> CalculateEffectDistanceRange(Unit unit, SomberInertia.Core.Combat.Range range)
    {
        if (unit?.Block == null)
        {
            Logger.Error("Unit is not on a block.");
            return new HashSet<(int x, int y)>();
        }

        var queue = new Queue<Block>();
        var visited = new HashSet<(int x, int y)>();
        var effectRangeSet = new HashSet<(int x, int y)>();
        var start = unit.Block;

        var minRange = range.Min;
        var maxRange = range.Max;

        queue.Enqueue(start);
        visited.Add((start.X, start.Y));

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            var distance = Math.Abs(current.X - start.X) + Math.Abs(current.Y - start.Y);

            // Add tile if it's within min and max range (inclusive)
            if (distance >= minRange && distance <= maxRange)
            {
                effectRangeSet.Add((current.X, current.Y));
            }

            if (distance >= maxRange)
            {
                continue;
            }

            foreach (var neighbor in GetAdjacentBlocks(current))
            {
                var coord = (neighbor.X, neighbor.Y);

                if (!visited.Contains(coord))
                {
                    visited.Add(coord);
                    queue.Enqueue(neighbor);
                }
            }
        }

        return effectRangeSet;
    }

    // Note: attack range could be spell range, or item range.
    public List<Unit> BuildListOfUnitsInAttackRange(Unit unit)
    {
        Logger.Debug("Grid::BuildListOfUnitsInAttackRange() building list of units in attack.");

        return BuildListOfUnitsInRange(unit, WeaponAttackRangeSet);
    }

    public List<Unit> BuildListOfUnitsInMagicRange(Unit unit)
    {
        Logger.Debug("Grid::BuildListOfUnitsInMagicRange() building list of units in magic range.");

        return BuildListOfUnitsInRange(unit, MagicAttackRangeSet);
    }

    public List<Unit> BuildListOfUnitsInSpellEffectRange(Unit unit)
    {
        Logger.Debug("Grid::BuildListOfUnitsInSpellEffectRange() building list of units in spell effect range.");

        return BuildListOfUnitsInRange(unit, SpellEffectRangeSet);
    }

    private List<Unit> BuildListOfUnitsInRange(Unit unit, HashSet<(int x, int y)> rangeSet)
    {
        var unitsInRange = new List<Unit>();

        foreach (var (x, y) in rangeSet)
        {
            var occupant = Blocks[x, y].PeekOccupant();
            if (occupant == null)
            {
                continue;
            }

            Logger.Debug($"   occupant [{occupant.Name}] found at {Blocks[x, y].ToString()}");
            unitsInRange.Add(occupant);
        }

        Logger.Debug($"   unitsInRange = [{unitsInRange.Count}]. ");

        return unitsInRange;
    }

    private IEnumerable<Block> GetAdjacentBlocks(Block block)
    {
        var directions = new (int dx, int dy)[]
        {
            (-1, 0), (1, 0), (0, -1), (0, 1)   // Up, Down, Left, Right
        };

        foreach (var (dx, dy) in directions)
        {
            var newX = block.X + dx;
            var newY = block.Y + dy;

            if (newX >= 0 && newX < Width && newY >= 0 && newY < Height)
            {
                yield return Blocks[newX, newY];
            }
        }
    }

    private int CalculateTerrainTypeCost(MovementType movementType, TerrainType terrainType)
    {
        if (_movementCostsMap.TryGetValue(movementType, out var terrainDict))
        {
            if (terrainDict.TryGetValue(terrainType, out var cost))
            {
                return cost;
            }
        }

        throw new ArgumentNullException($"No movement type [{movementType}] cost or terrain type [{terrainType}] found in _movementCosts dictionary.");
    }

    public void PlaceUnit(Unit unit, int x, int y)
    {
        // Clear old position if any
        if (unit.Block != null)
        {
            unit.Block.PopOccupant();
        }

        // Place on new block
        Blocks[x, y].PushOccupant(unit);

        // Only snap to grid if the unit is NOT currently sliding
        if (!unit.IsAnimating)
        {
            unit.SnapToCurrentBlock();
        }

        Logger.Debug($"Unit '{unit.Name}' placed at {Blocks[x, y].PrintGridCoordinates()}.");
    }

    public void MoveUnitInDirection(Unit unit, Direction direction)
    {
        if (unit?.Block == null)
        {
            Logger.Warning("MoveUnitInDirection called with null unit or block.");
            return;
        }

        unit.FacingDirection = direction;

        var newX = unit.Block.X;
        var newY = unit.Block.Y;

        switch (direction)
        {
            case Direction.Up:    newY--; break;
            case Direction.Down:  newY++; break;
            case Direction.Left:  newX--; break;
            case Direction.Right: newX++; break;
        }

        if (newX < 0 || newX >= Width || newY < 0 || newY >= Height)
        {
            Logger.Debug($"Movement blocked: out of bounds ({newX}, {newY})");
            return;
        }

        if (!MovementRangeSet.Contains((newX, newY)))
        {
            Logger.Debug($"Movement blocked: block coordinate [{newX}, {newY}] not in movement range.");
            return;
        }

        // === IMPORTANT: Start visual movement FIRST ===
        unit.StartMovingTo(Blocks[newX, newY]);

        // === Then update logical grid position ===
        PlaceUnit(unit, newX, newY);
    }

    public void RemoveDeadUnitsFromGrid(List<Unit> deadUnits)
    {
        if (deadUnits == null || deadUnits.Count == 0)
        {
            return;
        }

        Logger.Debug($"Grid::RemoveDeadUnitsFromGrid(): removing {deadUnits.Count} dead unit(s).");

        foreach (var deadUnit in deadUnits)
        {
            if (deadUnit.Block != null)
            {
                var block = deadUnit.Block;

                var countBefore = block.OccupantCount();
                Logger.Debug($"  → Removing {deadUnit.Name} from block {block.PrintGridCoordinates()} (stack size before = {countBefore})");

                block.PopOccupant(); // ← only call once
                deadUnit.Block = null; // clear reference

                Logger.Debug($"  → Stack size after pop = {block.OccupantCount()}");
            }
            else
            {
                Logger.Warning($"Dead unit '{deadUnit.Name}' had no Block reference.");
            }
        }
    }
}