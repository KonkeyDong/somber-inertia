using SomberInertia.Enums;
using Raylib_cs;

namespace SomberInertia;


public class Block
{
    public Texture2D Texture { get; private set; }
    public TerrainType TerrainType { get; private set; }

    // Coordinates (immutable after creation)
    public byte X { get; private set; }
    public byte Y { get; private set; }

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

    public short MovementCost { get; set; } = 0;

    public Block(string texturePath, TerrainType terrainType, byte x, byte y)
    {
        Texture = Raylib.LoadTexture(texturePath);
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