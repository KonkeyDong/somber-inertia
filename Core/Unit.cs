using SomberInertia.Enums;
using SomberInertia.Structs;
using Raylib_cs;

namespace SomberInertia.Core;

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

            _block = value;
        }
    }

    // Stats
    public Stat HP { get; set; }
    public Stat MP { get; set; }
    public Job Job { get; set; }
    public byte Exp { get; set; } // experience
    public byte Attack { get; set; }
    public Weapon Weapon { get; set; }
    public byte Defense { get; set; }
    public byte Speed { get; set; }
    public int Movement { get; private set; }

    public bool Friendly { get; set; }

    public Unit(string texturePath, string name, MovementType movementType, int movement)
    {
        Texture = Raylib.LoadTexture(texturePath);
        Name = name;
        MovementType = movementType;

        // Original terrain movement costs could use values like 1.5.
        // Multiply by two to get rid of the decimal.
        Movement = (movement * 2);

        Logger.Info($"Unit created → {Name} ({movementType}), Movement: {movement}");
    }

    public void SetPosition(Block block)
    {
        Block = block;
    }

    public void EquipWeapon(Weapon weapon)
    {
        Logger.Warning("Unit::EquipWeapon(): will need to redesign when items are more incorporated.");

        Weapon = weapon;
    }

    public override string ToString() => $"{Name} ({MovementType}) at {Block?.PrintCoordinates() ?? "[null]"}";
}