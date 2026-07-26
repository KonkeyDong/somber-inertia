using SomberInertia.Enums;
using SomberInertia.Core.Combat.Spells;

namespace SomberInertia.Core.Combat.Item;

public abstract class Item
{
    public ItemName Name { get; protected set; }
    public ItemType ItemType { get; protected set; }
    public Range DistanceRange { get; protected set; }
    public Job AllowedJobs { get; protected set; }
    public int Price { get; protected set; }
    public bool Cursed { get; protected set; }

    // Optional effect for consumables / special weapons
    public IItemEffect? Effect { get; protected set; }

    protected Item(
        ItemName name,
        ItemType itemType,
        Range distanceRange,
        Job allowedJobs,
        int price,
        bool cursed = false,
        IItemEffect? effect = null)
    {
        Name = name;
        ItemType = itemType;
        DistanceRange = distanceRange;
        AllowedJobs = allowedJobs;
        Price = price;
        Cursed = cursed;
        Effect = effect;
    }

    public abstract Item Clone();
    public int SellPrice => (int)(Price * 0.75f);
}