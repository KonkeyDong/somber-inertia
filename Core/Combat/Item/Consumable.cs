using SomberInertia.Enums;

namespace SomberInertia.Core.Combat.Item;

public class Consumable : Item
{
    public Consumable(ItemName name, Range distanceRange, int price, IItemEffect? effect)
        : base(name, ItemType.Consumable, distanceRange, Job.Any, price, false, effect)
    {

    }

    public override Item Clone()
    {
        return new Consumable(
            Name,
            DistanceRange,
            Price,
            Effect
        );
    }
}