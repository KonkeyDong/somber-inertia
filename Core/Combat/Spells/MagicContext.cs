using SomberInertia.Core.Units;
using System.Text;
using System.Numerics;

namespace SomberInertia.Core.Combat.Spells;

public class MagicContext
{
    public Unit Caster { get; }
    public List<Unit> Targets { get; } // 1 or multiple
    public Grid Grid { get; }

    public MagicContext(Unit caster, List<Unit> targets, Grid grid)
    {
        Caster = caster;
        Targets = targets ?? new List<Unit>();
        Grid = grid;
    }

    public override string ToString() 
    {
        var sb = new StringBuilder();
        sb.AppendLine("MagicContext:");
        sb.AppendLine($"Caster = [{Caster.GetDisplayName()}]; Target Count = [{Targets.Count}]");
        sb.AppendLine("Unfolding targets:");
        foreach (var target in Targets)
        {
            sb.AppendLine($"  => {target.GetDisplayName()}");
        }

        return sb.ToString();
    }
        
}