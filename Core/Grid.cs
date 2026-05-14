using System;
using System.Collections.Generic;
using System.Numerics;
using SomberInertia.Timers;
using SomberInertia.Enums;
using Raylib_cs;

namespace SomberInertia.Core;

public class Grid
{
    public int Width { get; private set; }
    public int Height { get; private set; }

    public readonly Block[,] Blocks;

    public int BlockSize { get; set; } = (int)(GameConstants.TILE_SIZE * GameConstants.BASE_WINDOW_SCALE);

    private static readonly Dictionary<MovementType, Dictionary<TerrainType, int>> _movementCostsMap;
    private HashSet<(int x, int y)> _movementRangeSet = new HashSet<(int x, int y)>();
    private HashSet<(int x, int y)> _weaponAttackRangeSet = new HashSet<(int x, int y)>();
    public readonly MovementRangeTint RangeTint = new MovementRangeTint(6);

    // Static constructor will create the movement cost dictionary only once when Grid is first accessed.
    static Grid()
    {
        _movementCostsMap = new Dictionary<MovementType, Dictionary<TerrainType, int>>
        {
            [MovementType.Warrior] = new Dictionary<TerrainType, int>
            {
                { TerrainType.Road, 2 },
                { TerrainType.Plains, 2 },
                { TerrainType.Overgrowth, 3 },
                { TerrainType.Forest, 4 },
                { TerrainType.Hill, 3 },
                { TerrainType.Sand, 3 }
            },
        };
    }

    public Grid(int width, int height)
    {
        Width = width;
        Height = height;

        Logger.Info($"Creating {Width}x{Height} grid...");

        Blocks = new Block[Width, Height];

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
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

    public void ResetGridMovementCosts()
    {
        Logger.Info("ResetGridMovementCosts(): resetting visited HashSet and Blocks[,].MovementCost values.");
        _movementRangeSet.Clear();
    }

    public void ResetAttackRangeCosts()
    {
        Logger.Info("ResetAttackRangeCosts(): resetting attack range HashSet.");
        _weaponAttackRangeSet.Clear();
    }

    public void CalculateUnitMovementRange(Unit unit)
    {
        if (unit == null)
        {
            Logger.Error("CalculateUnitMovementRange() unit is null.");
            return;
        }

        if (unit?.Block == null) 
        {
            Logger.Error($"Unit {unit.Name} does not contain a block.");
            return;
        }

        _movementRangeSet.Clear();
        var costToReach = new Dictionary<(int x, int y), int>();

        var queue = new Queue<Block>();
        var start = unit.Block;

        _movementRangeSet.Add((start.X, start.Y));
        costToReach[(start.X, start.Y)] = 0;
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            Block current = queue.Dequeue();
            int currentCost = costToReach[(current.X, current.Y)];

            foreach (Block neighbor in GetAdjacentBlocks(current))
            {
                if (neighbor is null)
                {
                    Logger.Error("CalculateUnitMovementRange() neighbor is null; ignoring.");
                    continue;
                }

                var coord = (neighbor.X, neighbor.Y);
                if (_movementRangeSet.Contains(coord)) 
                    continue;

                int enterCost = neighbor.PeekOccupant() != null && unit.Friendly != neighbor?.PeekOccupant()?.Friendly
                    ? GameConstants.MAX_MOVEMENT_COST
                    : CalculateTerrainTypeCost(unit.MovementType, neighbor.TerrainType);

                int totalCost = currentCost + enterCost;

                if (totalCost <= unit.Movement)
                {
                    _movementRangeSet.Add(coord);
                    costToReach[coord] = totalCost;
                    queue.Enqueue(neighbor);
                }
            }
        }
    }

    public void CalculateWeaponAttackRange(Unit unit)
    {
        if (unit?.Block == null || unit.Weapon == null)
            return;

        _weaponAttackRangeSet.Clear();

        var queue = new Queue<Block>();
        var visited = new HashSet<(int x, int y)>();
        var start = unit.Block;

        int minRange = unit.Weapon.Range.Min;
        int maxRange = unit.Weapon.Range.Max;

        queue.Enqueue(start);
        visited.Add((start.X, start.Y));

        while (queue.Count > 0)
        {
            Block current = queue.Dequeue();
            int distance = Math.Abs(current.X - start.X) + Math.Abs(current.Y - start.Y);

            // Add tile if it's within min and max range (inclusive)
            if (distance >= minRange && distance <= maxRange)
            {
                _weaponAttackRangeSet.Add((current.X, current.Y));
            }

            if (distance >= maxRange)
            {
                continue;
            }

            foreach (Block neighbor in GetAdjacentBlocks(current))
            {
                var coord = (neighbor.X, neighbor.Y);

                if (!visited.Contains(coord))
                {
                    visited.Add(coord);
                    queue.Enqueue(neighbor);
                }
            }
        }
    }

    private IEnumerable<Block> GetAdjacentBlocks(Block block)
    {
        var directions = new (int dx, int dy)[]
        {
            (-1, 0), (1, 0), (0, -1), (0, 1)   // Up, Down, Left, Right
        };

        foreach (var (dx, dy) in directions)
        {
            int newX = block.X + dx;
            int newY = block.Y + dy;

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
            if (terrainDict.TryGetValue(terrainType, out int cost))
            {
                return cost;
            }
        }

        throw new ArgumentNullException($"No movement type [{movementType}] cost or terrain type [{terrainType}] found in _movementCosts dictionary.");
    }

    public void DrawBackground(float scale)
    {
        var debugFlag = Logger.MinimumLevel == LogLevel.Debug;

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                int screenX = x * BlockSize;
                int screenY = y * BlockSize;

                Raylib.DrawTextureEx(
                    Blocks[x, y].Texture, 
                    new Vector2(screenX, screenY), 
                    0.0f, // rotation
                    scale,
                    Color.White
                );

                if (debugFlag)
                {
                    // Raylib.DrawText($"MC: {Blocks[x, y].MovementCost}", screenX, screenY, 16, Color.White);
                    Raylib.DrawText(Blocks[x, y].PrintCoordinates(), screenX, screenY + 20, 16, Color.White);
                }
            }
        }
    }

    public void DrawMovementRange(float scale)
    {
        DrawRangeBlockColor(scale, _movementRangeSet);
    }

    public void DrawWeaponAttackRange(float scale)
    {
        DrawRangeBlockColor(scale, _weaponAttackRangeSet);
    }

    private void DrawRangeBlockColor(float scale, HashSet<(int x, int y)> hashSet)
    {
        foreach((int x, int y) in hashSet)
        {
            int screenX = x * BlockSize;
            int screenY = y * BlockSize;

            Raylib.DrawTextureEx(
                Blocks[x, y].Texture,
                new Vector2(screenX, screenY), 
                0.0f,
                scale,
                RangeTint.GetCurrentColor()
            );
        }
    }

    public void DrawUnits(List<Unit> units, float scale)
    {
        // We loop in reverse to get the drawing order correct.
        // This allows current controlled unit to always be on top
        // of a block containing an occupant.
        for (int i = units.Count - 1; i >= 0; i--)
        {
            var unit = units[i];
            if (unit.Block == null)
            {
                Logger.Error($"Unit {unit.Name} has no Block reference!");
                continue;
            }

            int screenX = unit.Block.X * BlockSize;
            int screenY = unit.Block.Y * BlockSize;

            Raylib.DrawTextureEx(
                unit.Texture,
                new Vector2(screenX, screenY), 
                0.0f,
                scale,
                Color.White
            );
        }
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

        Logger.Debug($"Unit '{unit.Name}' placed at {Blocks[x, y].PrintCoordinates()}.");
    }

    public void MoveUnitInDirection(Unit unit, Direction direction)
    {
        if (unit?.Block == null) 
        {
            Logger.Warning("MoveUnitInDirection called with null unit or block.");
            return;
        }

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

        if (!_movementRangeSet.Contains((newX, newY)))
        {
            Logger.Debug($"Movement blocked: block coordinate [{newX}, {newY}] not in movement range.");
            return;
        }

        PlaceUnit(unit, newX, newY);
    }
}