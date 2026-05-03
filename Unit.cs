using Raylib_cs;

namespace SomberInertia;

public enum MovementType
{
    Warrior,
    Flyer,
    Horse,
    Mage,
    Thief,
    Archer,
    Werewolf,
}

public class Unit
{
    public Texture2D Texture { get; private set; }

    public string Name { get; private set; }
    public MovementType MovementType { get; private set; }

    // Core reference - source of truth for position
    private Block? _block;
    public Block? Block
    {
        get => _block;
        set
        {
            if (_block == value) return; // avoid spam on same value

            Logger.Debug($"Unit '{Name}' moved from " +
                $"{(_block?.PrintCoordinates() ?? "null")} → " +
                $"{(value?.PrintCoordinates() ?? "null")}");

            _block = value;
        }
    }

    // Stats
    public byte HP { get; set; }
    public byte MP { get; set; }
    public byte Attack { get; set; }
    public byte Defense { get; set; }
    public byte Speed { get; set; }
    public byte Movement { get; private set; }   // movement range

    public bool Friendly { get; set; }

    public Unit(string texturePath, string name, MovementType movementType, byte movement = 8)
    {
        Texture = Raylib.LoadTexture(texturePath);
        Name = name;
        MovementType = movementType;
        Movement = movement;

        Logger.Info($"Unit created → {Name} ({movementType}), Movement: {movement}");
    }

    /// <summary>
    /// Convenience method to set position (triggers logging)
    /// </summary>
    public void SetPosition(Block block)
    {
        Block = block;
    }

    public override string ToString() => $"{Name} ({MovementType}) at {Block?.PrintCoordinates() ?? "null"}";
}