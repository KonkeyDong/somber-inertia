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

    private readonly List<Unit> _units = new List<Unit>();
    public IReadOnlyList<Unit> Units => _units; // public read-only

    public int BlockSize { get; set; } = (int)(GameConstants.TILE_SIZE * GameConstants.BASE_WINDOW_SCALE);

    private static readonly Dictionary<MovementType, Dictionary<TerrainType, int>> _movementCostsMap;
    private HashSet<(int x, int y)> _movementRangeSet = new HashSet<(int x, int y)>();
    public readonly MovementRangeTint MovementRangeTint = new MovementRangeTint(6);

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
        Logger.Info("ResetGridMovementCosts(): resetting Visited HashSet and Blocks[,].MovementCost values.");
        _movementRangeSet.Clear();
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
        foreach ((int x, int y) in _movementRangeSet)
        {
            int screenX = x * BlockSize;
            int screenY = y * BlockSize;

            Raylib.DrawTextureEx(
                Blocks[x, y].Texture,
                new Vector2(screenX, screenY), 
                0.0f,
                scale,
                MovementRangeTint.GetCurrentColor()
            );
        }
    }

    public void DrawUnits(float scale)
    {
        // We loop in reverse to get the drawing order correct.
        // This allows current controlled unit to always be on top
        // of a block containing an occupant.
        for (int i = _units.Count - 1; i >= 0; i--)
        {
            var unit = _units[i];
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

    public void AddUnit(Unit unit, int x, int y)
    {
        if (unit == null) 
            throw new ArgumentNullException(nameof(unit));

        _units.Add(unit);
        PlaceUnit(unit, x, y);
    }

    private void PlaceUnit(Unit unit, int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            throw new ArgumentOutOfRangeException($"Target position ({x}, {y}) is outside grid bounds.");

        // Clear old position if any
        if (unit.Block != null)
        {
            unit.Block.PopOccupant();
        }

        // Place on new block
        Blocks[x, y].PushOccupant(unit);

        Logger.Info($"Unit '{unit.Name}' placed at {Blocks[x, y].PrintCoordinates()}.");
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