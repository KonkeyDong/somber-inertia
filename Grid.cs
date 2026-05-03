using System;
using System.Collections.Generic;
using Raylib_cs;

namespace SomberInertia;

public enum Direction
{
    Up,
    Right,
    Down,
    Left
}

public class Grid
{
    public byte Width { get; private set; }
    public byte Height { get; private set; }

    public readonly Block[,] Blocks;

    private readonly List<Unit> _units = new List<Unit>();
    public IReadOnlyList<Unit> Units => _units; // public read-only

    private const int TileSize = 24;
    private const int BlockSize = TileSize * 8;   // 192 pixels per block

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
                Blocks[x, y] = new Block("assets/grass_tile.png", TerrainType.Grass, x, y);
            }
        }

        Logger.Info("Grid initialization complete.");
    }

    public void DrawBackground()
    {
        for (byte x = 0; x < Width; x++)
        {
            for (byte y = 0; y < Height; y++)
            {
                int screenX = x * BlockSize;
                int screenY = y * BlockSize;
                Raylib.DrawTexture(Blocks[x, y].Texture, screenX, screenY, Color.White);
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

        Logger.Info($"Unit '{unit.Name}' placed at [{x}, {y}]");
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

        PlaceUnit(unit, newX, newY);
    }
}