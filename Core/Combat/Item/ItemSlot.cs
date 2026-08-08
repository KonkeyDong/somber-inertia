using SomberInertia.Enums;
using System.Text;

namespace SomberInertia.Core.Combat.Item;

public readonly struct ItemSlot
{
    public ItemName Name { get; init; } // basically an ID
    public bool Damaged { get; init; }

    public static ItemSlot Empty => new ItemSlot
    {
        Name = ItemName.NoItem,
        Damaged = false
    };

    public bool IsEmpty => Name == ItemName.NoItem;

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine("Item name: " + Name.GetDisplayName());
        sb.AppendLine("Damaged: " + Damaged);

        return sb.ToString();
    }
}
