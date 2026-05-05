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

// Class is a reserved word; job will have to do.
public enum Job
{
    Swordsman,
    Hero,
    Warrior,
    Gladiator,
    Archer,
    Sniper,
    Mage,
    Wizard,
    Knight,
    Paladin,
}

public struct Stat
{
    public byte Current { get; set; }
    public byte Max { get; set; }
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
    public Stat HP { get; set; }
    public Stat MP { get; set; }
    public Job Job { get; set; }
    public byte Exp { get; set; } // experience
    public byte Attack { get; set; }
    public byte Defense { get; set; }
    public byte Speed { get; set; }
    public float Movement { get; private set; }   // movement range

    public bool Friendly { get; set; }

    public Unit(string texturePath, string name, MovementType movementType, float movement)
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