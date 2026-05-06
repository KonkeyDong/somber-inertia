using System;
using System.Collections.Generic;
using SomberInertia.Timers;
using SomberInertia.Enums;
using Raylib_cs;

namespace SomberInertia;

public class Grid
{
    public byte Width { get; private set; }
    public byte Height { get; private set; }

    public readonly Block[,] Blocks;

    private readonly List<Unit> _units = new List<Unit>();
    public IReadOnlyList<Unit> Units => _units; // public read-only

    private const int TileSize = 24;
    private const int BlockSize = TileSize * 8;   // 192 pixels per block

    private static readonly Dictionary<MovementType, Dictionary<TerrainType, float>> _movementCostsMap;
    private HashSet<(byte x, byte y)> MovementRangeSet = new HashSet<(byte x, byte y)>();

    // Static constructor will create the movement cost dictionary only once when Grid is first accessed.
    static Grid()
    {
        _movementCostsMap = new Dictionary<MovementType, Dictionary<TerrainType, float>>
        {
            [MovementType.Warrior] = new Dictionary<TerrainType, float>
            {
                { TerrainType.Road, 10 },
                { TerrainType.Plains, 10 },
                { TerrainType.Overgrowth, 15 },
                { TerrainType.Forest, 20 },
                { TerrainType.Hill, 15 },
                { TerrainType.Sand, 15 }
            },
        };
    }

    public Grid(byte width, byte height)
    {
        Width = width;
        Height = height;

        Logger.Info($"Creating {Width}x{Height} grid...");

        Blocks = new Block[Width, Height];

        for (byte x = 0; x < Width; x++)
        {
            for (byte y = 0; y < Height; y++)
            {
                Blocks[x, y] = new Block("assets/grass_tile.png", TerrainType.Plains, x, y);
            }
        }

        var tempCoords = new (byte x, byte y)[2]
        {
            (0, 2),
            (0, 1)
        };
        foreach (var point in tempCoords)
        {
            Blocks[point.x, point.y] = new Block("assets/forest_tile.png", TerrainType.Forest, point.x, point.y);
        }


        Logger.Info("Grid initialization complete.");
    }

    public void ResetGridMovementCosts()
    {
        Logger.Info("ResetGridMovementCosts(): resetting Visited HashSet and Blocks[,].MovementCost values.");
        MovementRangeSet.Clear();

        for (byte x = 0; x < Width; x++)
        {
            for (byte y = 0; y < Height; y++)
            {
                Blocks[x, y].MovementCost = 0;
            }
        }
    }

    public void CalculateUnitMovementRange(Unit unit)
    {
        if (unit?.Block == null)
        {
            throw new NullReferenceException($"Unit {unit?.Name ?? "null"} is not on a block.");
        }

        Logger.Debug($"CalculateUnitMovementRange() called with unit: {unit.ToString()}");

        ResetGridMovementCosts();
        var queue = new Queue<Block>();
        var start = unit.Block;

        queue.Enqueue(start);
        MovementRangeSet.Add((start.X, start.Y));

        while (queue.Count > 0)
        {
            Block current = queue.Dequeue();

            // Check all 4 directions
            foreach (Block neighbor in GetAdjacentBlocks(current))
            {
                var coord = (neighbor.X, neighbor.Y);

                if (MovementRangeSet.Contains(coord))
                    continue;

                short enterCost = neighbor.Occupant != null && unit.Friendly != neighbor?.Occupant?.Friendly
                    ? (short)255
                    : CalculateTerrainTypeCost(unit.MovementType, neighbor.TerrainType);

                short totalCost = (short)(current.MovementCost + enterCost);
                neighbor.MovementCost = totalCost;

                if (totalCost <= unit.Movement)
                {                  
                    queue.Enqueue(neighbor);
                    MovementRangeSet.Add(coord);
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

    private short CalculateTerrainTypeCost(MovementType movementType, TerrainType terrainType)
    {
        if (_movementCostsMap.TryGetValue(movementType, out var terrainDict))
        {
            if (terrainDict.TryGetValue(terrainType, out float cost))
            {
                return (short)cost;
            }
        }

        throw new ArgumentNullException($"No movement type [{movementType}] cost or terrain type [{terrainType}] found in _movementCosts dictionary.");
    }

    public void DrawBackground(MovementRangeTint rangeTint)
    {
        for (byte x = 0; x < Width; x++)
        {
            for (byte y = 0; y < Height; y++)
            {
                int screenX = x * BlockSize;
                int screenY = y * BlockSize;

                var color = MovementRangeSet.Contains((x, y)) 
                    ? rangeTint.GetCurrentColor()
                    : Color.White;

                Raylib.DrawTexture(Blocks[x, y].Texture, screenX, screenY, color);

                if (Logger.MinimumLevel == LogLevel.Debug)
                {
                    Raylib.DrawText($"Movement Cost: {Blocks[x, y].MovementCost}", screenX, screenY, 16, Color.White);
                    Raylib.DrawText(Blocks[x, y].PrintCoordinates(), screenX, screenY + 20, 16, Color.White);
                }
            }
        }
    }

    public void DrawUnits()
    {
        foreach (Unit unit in _units)
        {
            if (unit.Block == null)
            {
                Logger.Error($"Unit {unit.Name} has no Block reference!");
                continue;                    // Don't crash the whole draw
            }

            int screenX = unit.Block.X * BlockSize;
            int screenY = unit.Block.Y * BlockSize;

            Raylib.DrawTexture(unit.Texture, screenX, screenY, Color.White);
        }
    }

    public void AddUnit(Unit unit, byte x, byte y)
    {
        if (unit == null) 
            throw new ArgumentNullException(nameof(unit));

        _units.Add(unit);
        PlaceUnit(unit, x, y);
    }

    private void PlaceUnit(Unit unit, byte x, byte y)
    {
        if (x >= Width || y >= Height)
            throw new ArgumentOutOfRangeException($"Target position ({x}, {y}) is outside grid bounds.");

        // Clear old position if any
        if (unit.Block != null)
            unit.Block.ClearOccupant();

        // Place on new block
        Blocks[x, y].SetOccupant(unit);

        Logger.Info($"Unit '{unit.Name}' placed at {Blocks[x, y].PrintCoordinates()}.");
    }

    public void MoveUnitInDirection(Unit unit, Direction direction)
    {
        if (unit?.Block == null) 
        {
            Logger.Warning("MoveUnitInDirection called with null unit or block.");
            return;
        }

        byte newX = unit.Block.X;
        byte newY = unit.Block.Y;

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

        PlaceUnit(unit, newX, newY);
    }
}