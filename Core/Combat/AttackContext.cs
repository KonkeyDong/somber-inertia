using SomberInertia.Core.Units;
using SomberInertia.Graphics;
using System.Text;

namespace SomberInertia.Core.Combat;

public class AttackContext
{
    public Unit Attacker { get; private set; }
    public Unit Defender { get; private set; }

    public bool Hit { get; set; }
    public bool Crit { get; set; }
    public int Damage { get; set; }

    public BattleSpriteSet ForceMemberSpriteSet { get; private set; } = new();
    public BattleSpriteSet MonsterSpriteSet { get; private set; } = new();

    public AttackContext(Unit attacker, Unit defender)
    {
        Logger.Debug("Building AttackContext.");
        Attacker = attacker;
        Defender = defender;

        CombatSystem.CalculateAttackOutcome(this);

        AssignBattleSprites();
        BuildBattleScene();
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

        Logger.Debug(ForceMemberSpriteSet.ToString());
        Logger.Debug(MonsterSpriteSet.ToString());
    }

    private void BuildBattleScene()
    {
        if (Defender.Friendly)
        {
            var count = MonsterSpriteSet.Attack.Count - 1;

            for (var i = 0; i < MonsterSpriteSet.Attack.Count; i++)
            {
                MonsterSpriteSet.BuildBattleSequence(MonsterSpriteSet.GetAttackFrame(i), 10);
                
                var invertFlag = Hit && i == count;

                ForceMemberSpriteSet.BuildBattleSequence(ForceMemberSpriteSet.GetIdleFrame(i), 10, invertFlag);
            }
        }
        else
        {
            var count = ForceMemberSpriteSet.Attack.Count - 1;
            
            for (var i = 0; i < ForceMemberSpriteSet.Attack.Count; i++)
            {
                ForceMemberSpriteSet.BuildBattleSequence(ForceMemberSpriteSet.GetAttackFrame(i), 10);
                
                var invertFlag = Hit && i == count;

                MonsterSpriteSet.BuildBattleSequence(MonsterSpriteSet.GetIdleFrame(i), 10, invertFlag);
            }
        }
    }

    public void Reset()
    {
        Attacker = null!;
        Defender = null!;

        Hit = false;
        Crit = false;
        Damage = 0;

        ForceMemberSpriteSet.Reset();
        MonsterSpriteSet.Reset();
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