using SomberInertia.Core.Units;
using System.Text;

namespace SomberInertia.Core.Combat;

public class AttackContext
{
    public Unit Attacker { get; }
    public Unit Defender { get; }

    public AttackContext(Unit attacker, Unit defender)
    {
        Attacker = attacker;
        Defender = defender;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine("AttackContext:");
        sb.AppendLine("Attacker:");
        sb.AppendLine(Attacker.CombatToString());
        sb.AppendLine("Defender:");
        sb.AppendLine(Defender.CombatToString());

        return sb.ToString();
    }
}