using SomberInertia.Enums;
using SomberInertia.Core.Combat.Spells;

namespace SomberInertia.Core.Combat.Item.Weapon;

public class Weapon : Item
{
    public ItemName Name { get; set; }
    public int Attack { get; set; }
    public ItemType ItemType { get; set; }
    public Range DistanceRange { get; set; }
    public Magic? Spell { get; set; }
    public Job AllowedJobs { get; set; }
    public bool Cursed { get; set; }

    public Weapon(ItemName name, int attack, ItemType itemType, Range distanceRange, Magic? spell, Job allowedJobs, bool cursed = false)
    {
        if (attack < 0)
        {
            Logger.Warning("Weapon(): attack cannot be less than 0; defaulting to 0.");
            attack = 0;
        }

        Name = name;
        Attack = attack;
        ItemType = itemType;
        DistanceRange = distanceRange;
        Spell = spell;
        AllowedJobs = allowedJobs;
        Cursed = cursed;
    }
}