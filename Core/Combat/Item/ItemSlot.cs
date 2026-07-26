using SomberInertia.Enums;

namespace SomberInertia.Core.Combat.Item;

public struct ItemSlot
{
    public ItemName Name; // basically an ID
    public ItemCondition Condition;

    public static ItemSlot Empty => new ItemSlot
    {
        Name = ItemName.NoItem,
        Condition = ItemCondition.Normal
    };

    public bool IsEmpty => Name == ItemName.NoItem;
}