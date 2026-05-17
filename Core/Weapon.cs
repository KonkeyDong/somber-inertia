using SomberInertia.Enums;

namespace SomberInertia.Core;

public readonly struct WeaponRange
{
    public int Min { get; init; }
    public int Max { get; init; }

    public WeaponRange(int min, int max)
    {
        if (min < 0 || max < min)
        {
            Logger.Error("WeaponRange(): Min cannot be less than zero; Max cannot be less than Min.");
            throw new ArgumentOutOfRangeException("Min cannot be less than zero; Max cannot be less than Min.");
        }

        Min = min;
        Max = max;
    }

    public override string ToString() => $"[<Attack Range> Min: {Min}; Max: {Max}]]";
}

public class Weapon
{
    public string Name { get; set; }
    public int Attack { get; set; }
    public WeaponType WeaponType { get; set; }
    public WeaponRange Range { get; set; }
    public string Spell { get; set; } // not used... yet!
    public Job AllowedJobs { get; set; }
    public bool Cursed { get; set; }

    public Weapon(string name, int attack, WeaponType weaponType, WeaponRange range, string spell, Job allowedJobs, bool cursed = false)
    {
        if (attack < 0)
        {
            Logger.Warning("Weapon(): attack cannot be less than 0; defaulting to 0.");
            attack = 0;
        }

        Name = name;
        Attack = attack;
        WeaponType = weaponType;
        Range = range;
        Spell = spell;
        AllowedJobs = allowedJobs;
        Cursed = cursed;
    }
}