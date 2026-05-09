using SomberInertia.Enums;
using SomberInertia.Graphics;
using Raylib_cs;

namespace SomberInertia.Core;

public class Block
{
    public Texture2D Texture { get; private set; }
    public TerrainType TerrainType { get; private set; }

    // Coordinates (immutable after creation)
    public int X { get; private set; }
    public int Y { get; private set; }

    // Occupant management
    private Stack<Unit> _occupant = new Stack<Unit>();

    public Block(string texturePath, TerrainType terrainType, int x, int y)
    {
        Texture = SpriteManager.Load(texturePath);
        TerrainType = terrainType;
        X = x;
        Y = y;

        Logger.Debug($"Block created at [{x}, {y}] - {terrainType}");
    }

    public void PushOccupant(Unit unit)
    {
        if (_occupant.Count == 2)
        {
            Logger.Error($"PushOccupant(): Block {PrintCoordinates()} contains two occupants; no more can be set at block.");
            throw new IndexOutOfRangeException("Stack<Unit> in Block class can at most contain two occupants.");
        }

        Logger.Debug($"PushOccupant(): pushing unit {unit.Name} into block {PrintCoordinates()}.");
        _occupant.Push(unit);

        if (unit != null)
        {
            unit.Block = this;   // important: keep bidirectional link
        }
    }

    public Unit PeekOccupant()
    {
        if (_occupant.Count == 0)
        {
            return null;
        }

        var unit = _occupant.Peek();
        Logger.Debug($"PeekOccupant(): block {PrintCoordinates()} currently contains {unit?.Name ?? null}.");

        return unit;
    }

    public Unit PopOccupant()
    {
        if (_occupant.Count == 0)
        {
            Logger.Error($"PopOccupant(): Block {PrintCoordinates()} attempting to pop occupant stack with size zero.");
            throw new IndexOutOfRangeException("Stack<Unit> in Block class has zero units and cannot be popped any further.");
        }

        var unit  = _occupant.Pop();
        unit.Block = null; // clear block data on unit

        return unit;
    }

    public string PrintCoordinates() => $"[{X}, {Y}]";

    public override string ToString() => $"Block [{X}, {Y}] ({TerrainType})";
}