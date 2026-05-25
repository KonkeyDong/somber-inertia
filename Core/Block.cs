using System.Numerics;
using Raylib_cs;
using SomberInertia.Enums;
using SomberInertia.Graphics;
using SomberInertia.State;
using SomberInertia.Core.Units;

namespace SomberInertia.Core;

public class Block
{
    public Texture2D Texture { get; private set; }
    public TerrainType TerrainType { get; private set; }

    public int X { get; private set; }
    public int Y { get; private set; }

    private readonly Stack<Unit> _occupant = new();

    public Block(string texturePath, TerrainType terrainType, int x, int y)
    {
        Texture = SpriteManager.Load(texturePath);
        TerrainType = terrainType;
        X = x;
        Y = y;

        Logger.Debug($"Block created at [{x}, {y}] - {terrainType}");
    }

    public bool IsFullyOccupied() => _occupant.Count == 2;

    public void PushOccupant(Unit unit)
    {
        if (_occupant.Count == 2)
        {
            Logger.Error($"PushOccupant(): Block {PrintGridCoordinates()} already has two occupants.");
        }

        Logger.Debug($"PushOccupant(): pushing {unit.Name} into {PrintGridCoordinates()}.");
        _occupant.Push(unit);
        unit.Block = this;
    }

    public Unit? PeekOccupant() => _occupant.Count > 0 ? _occupant.Peek() : null;

    public Unit PopOccupant()
    {
        if (_occupant.Count == 0)
        {
            Logger.Error($"PopOccupant(): Block {PrintGridCoordinates()} has no occupants.");
        }

        var unit = _occupant.Pop();
        unit.Block = null;

        return unit;
    }

    public Vector2 GetPixelCoordinates()
    {
        var blockSize = GetScaledSize();

        return new Vector2(X * blockSize, Y * blockSize);
    }

    public int GetScaledSize() => GameConstants.TILE_SIZE * (int)GameStateManager.CurrentScale;

    public string PrintGridCoordinates() => $"[{X}, {Y}]";
}