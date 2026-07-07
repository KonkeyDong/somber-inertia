using SomberInertia.Core.Units;
using SomberInertia.Graphics;
using System.Text;

namespace SomberInertia.Core.Combat;

public class AttackContext
{
    public Unit Attacker { get; }
    public Unit Defender { get; }

    public BattleSpriteSet ForceMemberSpriteSet { get; private set; } = new();
    public BattleSpriteSet MonsterSpriteSet { get; private set; } = new();

    public AttackContext(Unit attacker, Unit defender)
    {
        Logger.Warning("Attacker and Defender properties might not be necessary. Revisit later.");
        Attacker = attacker;
        Defender = defender;

        AssignBattleSprites();
    }

    private void AssignBattleSprites()
    {
        var attackerSprites = BattleSpriteManager.Get(Attacker);
        var defenderSprites = BattleSpriteManager.Get(Defender);

        if (Defender.Friendly)
        {
            ForceMemberSpriteSet = defenderSprites;
            MonsterSpriteSet = attackerSprites;
        }
        else
        {
            MonsterSpriteSet = defenderSprites;
            ForceMemberSpriteSet = attackerSprites;
        }
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