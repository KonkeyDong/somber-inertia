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

    /// <summary>Unit drawn on the unfriendly (left) side of the battle screen.</summary>
    public Unit GetMonster()
    {
        return Defender.Friendly ? Attacker : Defender;
    }

    /// <summary>Unit drawn on the friendly (right) side of the battle screen.</summary>
    public Unit GetForceMember()
    {
        return Defender.Friendly ? Defender : Attacker;
    }

    /// <summary>Sprite set for the unit who is attacking this exchange (not screen side).</summary>
    public BattleSpriteSet GetAttackerSpriteSet()
    {
        return Attacker.Friendly ? ForceMemberSpriteSet : MonsterSpriteSet;
    }

    /// <summary>Sprite set for the unit who is defending this exchange (not screen side).</summary>
    public BattleSpriteSet GetDefenderSpriteSet()
    {
        return Defender.Friendly ? ForceMemberSpriteSet : MonsterSpriteSet;
    }

    private void AssignBattleSprites()
    {
        Logger.Debug("AssignBattleSprites()");
        var attackerSprites = BattleSpriteManager.Get(Attacker);
        var defenderSprites = BattleSpriteManager.Get(Defender);

        if (Defender.Friendly)
        {
            ForceMemberSpriteSet = defenderSprites;
            MonsterSpriteSet = attackerSprites;

            MonsterSpriteSet.SetBasePosition(Attacker);
            ForceMemberSpriteSet.SetBasePosition(Defender);
        }
        else
        {
            MonsterSpriteSet = defenderSprites;
            ForceMemberSpriteSet = attackerSprites;

            MonsterSpriteSet.SetBasePosition(Defender);
            ForceMemberSpriteSet.SetBasePosition(Attacker);
        }

        Logger.Debug(ForceMemberSpriteSet.ToString());
        Logger.Debug(MonsterSpriteSet.ToString());
    }

    private void BuildBattleScene()
    {
        // Debug scrubber: one entry per pose so Left/Right steps distinct frames.
        var poseCopies = Logger.InDebugMode() ? 1 : GameConstants.Animations.AttackDelay;
        var dissolveCopies = Logger.InDebugMode() ? 1 : GameConstants.Animations.Dissolve.NumberOfFrameCopies;

        if (Logger.InDebugMode())
        {
            AppendAttackerIdlePrefix();
        }

        if (Defender.Friendly)
        {
            var count = MonsterSpriteSet.Attack.Count - 1;

            for (var i = 0; i < MonsterSpriteSet.Attack.Count; i++)
            {
                MonsterSpriteSet.BuildBattleSequence(MonsterSpriteSet.GetAttackFrame(i), poseCopies);

                var invertFlag = Hit && i == count;

                ForceMemberSpriteSet.BuildBattleSequence(ForceMemberSpriteSet.GetIdleFrame(i), poseCopies, invertFlag);
            }

            if (Hit && (Defender.HP.Current - Damage <= 0)) // killed
            {
                MonsterSpriteSet.BuildBattleSequence(
                    MonsterSpriteSet.GetAttackFrame(MonsterSpriteSet.Attack.Count - 1),
                    dissolveCopies * GameConstants.Animations.Dissolve.GroupSize);

                for (var i = 1; i <= GameConstants.Animations.Dissolve.GroupSize; i++)
                {
                    var sprite = ForceMemberSpriteSet.GetIdleFrame(0).Dissolve(i);

                    ForceMemberSpriteSet.BuildBattleSequence(sprite, dissolveCopies);
                }
            }
        }
        else
        {
            var count = ForceMemberSpriteSet.Attack.Count - 1;

            for (var i = 0; i < ForceMemberSpriteSet.Attack.Count; i++)
            {
                ForceMemberSpriteSet.BuildBattleSequence(ForceMemberSpriteSet.GetAttackFrame(i), poseCopies);

                var invertFlag = Hit && i == count;

                MonsterSpriteSet.BuildBattleSequence(MonsterSpriteSet.GetIdleFrame(i), poseCopies, invertFlag);
            }

            if (Hit && (Defender.HP.Current - Damage <= 0)) // killed
            {
                ForceMemberSpriteSet.BuildBattleSequence(
                    ForceMemberSpriteSet.GetAttackFrame(ForceMemberSpriteSet.Attack.Count - 1),
                    dissolveCopies * GameConstants.Animations.Dissolve.GroupSize);

                for (var i = 1; i <= GameConstants.Animations.Dissolve.GroupSize; i++)
                {
                    var sprite = MonsterSpriteSet.GetIdleFrame(0).Dissolve(i);

                    MonsterSpriteSet.BuildBattleSequence(sprite, dissolveCopies);
                }
            }
        }
    }

    /// <summary>
    /// Debug only: prepend attacker's full idle sheet so the scrubber can check idle
    /// positioning before attack poses. Defender holds idle frame 0 for pairing.
    /// </summary>
    private void AppendAttackerIdlePrefix()
    {
        var attackerSet = GetAttackerSpriteSet();
        var defenderSet = GetDefenderSpriteSet();

        if (attackerSet.Idle.Count == 0)
        {
            Logger.Warning("AppendAttackerIdlePrefix: attacker has no idle frames; skipping.");
            return;
        }

        Logger.Debug($"AppendAttackerIdlePrefix: {attackerSet.Idle.Count} idle pose(s).");

        for (var i = 0; i < attackerSet.Idle.Count; i++)
        {
            attackerSet.BuildBattleSequence(attackerSet.Idle[i], 1);

            if (defenderSet.Idle.Count > 0)
            {
                defenderSet.BuildBattleSequence(defenderSet.GetIdleFrame(0), 1);
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