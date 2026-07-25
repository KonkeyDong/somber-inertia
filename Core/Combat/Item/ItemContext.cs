using SomberInertia.Core.Units;
using System.Text;

namespace SomberInertia.Core.Combat.Items;

public class ItemContext
{
    public Unit User { get; }
    public List<Unit> Targets { get; }
    public Grid Grid { get; }

    public ItemContext(Unit user, List<Unit> targets, Grid grid)
    {
        User = user;
        Targets = targets ?? new List<Unit>();
        Grid = grid;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine("ItemContext:");
        sb.AppendLine($"User = [{User.GetDisplayName()}]; Target Count = [{Targets.Count}]");
        sb.AppendLine("Unfolding targets:");

        foreach (var target in Targets)
        {
            sb.AppendLine($"  => {target.GetDisplayName()}");
        }

        return sb.ToString();
    }
}