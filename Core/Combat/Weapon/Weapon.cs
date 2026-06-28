using SomberInertia.Enums;

namespace SomberInertia.Core.Combat.Weapon;

public class Weapon
{
    public string Name { get; set; }
    public int Attack { get; set; }
    public WeaponType WeaponType { get; set; }
    public Range DistanceRange { get; set; }
    public string Spell { get; set; } // not used... yet!
    public Job AllowedJobs { get; set; }
    public bool Cursed { get; set; }

    public Weapon(string name, int attack, WeaponType weaponType, Range distanceRange, string spell, Job allowedJobs, bool cursed = false)
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