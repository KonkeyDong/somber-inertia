using SomberInertia.Core.Units;
using System.Text;
using System.Numerics;

namespace SomberInertia.Core.Combat.Item;

public class ItemContext
{
    public Unit Caster { get; } // could be user, but meh
    public List<Unit> Targets { get; } // 1 or multiple
    public Grid Grid { get; }
    public ItemSlot ItemSlot { get; }

    public ItemContext(Unit caster, List<Unit> targets, Grid grid, ItemSlot itemSlot)
    {
        Caster = caster;
        Targets = targets ?? new List<Unit>();
        Grid = grid;
        ItemSlot = itemSlot;
    }

    public override string ToString() 
    {
        var sb = new StringBuilder();

        sb.AppendLine("ItemContext:");
        sb.AppendLine($"Caster/User = [{Caster.GetDisplayName()}]; Target Count = [{Targets.Count}]");
        sb.AppendLine(ItemSlot.ToString());
        sb.AppendLine("Unfolding targets:");
        foreach (var target in Targets)
        {
            sb.AppendLine($"  => {target.GetDisplayName()}");
        }

        return sb.ToString();
    }
        
}