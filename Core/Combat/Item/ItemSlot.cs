using SomberInertia.Enums;

namespace SomberInertia.Core.Combat.Item;

/// <summary>Inventory cell: item id + condition. Replace as a whole; do not mutate in place.</summary>
public readonly struct ItemSlot
{
    public ItemName Name { get; init; } // basically an ID
    public ItemCondition Condition { get; init; }

    public static ItemSlot Empty => new ItemSlot
    {
        Name = ItemName.NoItem,
        Condition = ItemCondition.Normal
    };

    public bool IsEmpty => Name == ItemName.NoItem;
}
