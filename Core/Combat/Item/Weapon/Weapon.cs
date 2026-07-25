using SomberInertia.Core.Combat.Item;
using SomberInertia.Core.Combat.Spells;
using SomberInertia.Enums;

namespace SomberInertia.Core.Combat.Item.Weapon;

public class Weapon : Item
{
    public int Attack { get; private set; }
    public Magic? Spell { get; private set; }

    public Weapon(
        ItemName name,
        int attack,
        ItemType itemType,
        Range distanceRange,
        Magic? spell,
        Job allowedJobs,
        int price,
        bool cursed = false)
        : base(name, itemType, distanceRange, allowedJobs, price, cursed)
    {
        if (attack < 0)
        {
            Logger.Error("Weapon(): attack cannot be less than 0; Aborting.");
        }

        Attack = attack;
        Spell = spell;
    }

    public bool CanBeUsedAsItem(Job job)
    {
        if (Spell == null)
        {
            return false;
        }

        if (AllowedJobs == Job.Any)
        {
            return true;
        }

        return (AllowedJobs & job) != 0;
    }

    public override Item Clone()
    {
        return new Weapon(
            Name,
            Attack,
            ItemType,
            DistanceRange,
            Spell?.Clone(),   // if Magic has Clone()
            AllowedJobs,
            Price,
            Cursed
        );
    }
}