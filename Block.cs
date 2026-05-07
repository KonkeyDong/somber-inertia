using SomberInertia.Enums;
using SomberInertia.Managers;
using Raylib_cs;

namespace SomberInertia;

public class Block
{
    public Texture2D Texture { get; private set; }
    public TerrainType TerrainType { get; private set; }

    // Coordinates (immutable after creation)
    public int X { get; private set; }
    public int Y { get; private set; }

    // Occupant management
    private Unit? _occupant;
    public Unit? Occupant
    {
        get => _occupant;
        set
        {
            if (_occupant == value) return; // avoid spam

            Logger.Debug($"Block [{X}, {Y}] occupant changed: " +
                $"{(_occupant?.Name ?? "null")} → {(value?.Name ?? "null")}");

            _occupant = value;
        }
    }

    public Block(string texturePath, TerrainType terrainType, int x, int y)
    {
        Texture = TextureManager.Load(texturePath);
        TerrainType = terrainType;
        X = x;
        Y = y;

        Logger.Debug($"Block created at [{x}, {y}] - {terrainType}");
    }

    /// <summary>
    /// Set a unit as occupant on this block (keeps both sides in sync)
    /// </summary>
    public void SetOccupant(Unit? unit)
    {
        Occupant = unit;

        if (unit != null)
            unit.Block = this;   // important: keep bidirectional link
    }

    /// <summary>
    /// Clear any occupant from this block
    /// </summary>
    public void ClearOccupant()
    {
        if (Occupant != null)
        {
            Occupant.Block = null;
            Occupant = null;
        }
    }

    public string PrintCoordinates() => $"[{X}, {Y}]";

    public override string ToString() => $"Block [{X}, {Y}] ({TerrainType})";
}