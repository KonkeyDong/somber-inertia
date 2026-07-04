using SomberInertia.Enums;
using SomberInertia.Core.Combat.Spells;

namespace SomberInertia.Core.Combat.Weapon;

public class Weapon
{
    public WeaponName Name { get; set; }
    public int Attack { get; set; }
    public WeaponType WeaponType { get; set; }
    public Range DistanceRange { get; set; }
    public Magic? Spell { get; set; }
    public Job AllowedJobs { get; set; }
    public bool Cursed { get; set; }

    public Weapon(WeaponName name, int attack, WeaponType weaponType, Range distanceRange, Magic? spell, Job allowedJobs, bool cursed = false)
    {
        if (attack < 0)
        {
            Logger.Warning("Weapon(): attack cannot be less than 0; defaulting to 0.");
            attack = 0;
        }

        Name = name;
        Attack = attack;
        WeaponType = weaponType;
        DistanceRange = distanceRange;
        Spell = spell;
        AllowedJobs = allowedJobs;
        Cursed = cursed;
    }
}