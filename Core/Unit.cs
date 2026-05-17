using SomberInertia.Enums;
using Raylib_cs;

namespace SomberInertia.Core;

public class Unit
{
    public class Stat
    {
        public int Current { get; set; }
        public int Max { get; set; }

        public Stat(int max)
        {
            Current = max;
            Max = max;
        }
    }

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
            if (_block == value) 
            {
                return; // avoid spam on same value
            }

            _block = value;
        }
    }

    // Stats
    public Stat HP { get; set; }
    public Stat MP { get; set; }
    public Job Job { get; set; }
    public int Exp { get; set; } // experience
    public int Attack { get; set; }
    public Weapon Weapon { get; set; } = null!;
    public int Defense { get; set; }
    public int Speed { get; set; }
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
        HP = new Stat(10);
        MP = new Stat(10);

        // default for now
        EquipWeapon(new Weapon("Unarmed", 0, WeaponType.Unarmed, new WeaponRange(1, 1)));

        Logger.Info($"Unit created → {Name} ({movementType}), Movement: {movement}");
    }

    public void SetPosition(Block block) => Block = block;

    public void EquipWeapon(Weapon weapon)
    {
        Logger.Warning("Unit::EquipWeapon(): will need to redesign when items are more incorporated.");

        Weapon = weapon;
    }

    public void TakeDamage(int amount)
    {
        Logger.Debug($"Unit::TakeDamage({amount})");
        Logger.Info($"Unit [{Name}] has been damaged for {amount}.");
    
        HP.Current = HP.Current - amount;
        if (HP.Current < 0)
        {
            HP.Current = 0;
        }
        Logger.Info($"\tUnit's current health: {HP.Current} / {HP.Max}.");
    }

    public override string ToString() => $"{Name} ({MovementType}) at {Block?.PrintCoordinates() ?? "[null]"}";
}