using SomberInertia.Core.Units;
using SomberInertia.Graphics;
using SomberInertia.Enums;
using System.Text;

namespace SomberInertia.Core.Combat;

public class AttackContext
{
    public Unit Attacker { get; private set; }
    public Unit Defender { get; private set; }

    public Effects Effect { get; private set; }

    public bool Hit { get; set; }
    public bool Crit { get; set; }
    public int Damage { get; set; }

    /// <summary>
    /// Frame index in the shared battle timeline when <see cref="Damage"/> should apply.
    /// Paced from the <b>attacker</b> attack sheet (not the defender/monster sheet).
    /// </summary>
    public int DamageApplyFrame { get; private set; }

    public BattleUnitSpriteSet ForceMemberSpriteSet { get; private set; } = new();
    public BattleUnitSpriteSet MonsterSpriteSet { get; private set; } = new();

    public AttackContext(Unit attacker, Unit defender)
    {
        Logger.Debug("Building AttackContext.");
        Attacker = attacker;
        Defender = defender;

        DetermineAttackEffect();
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
    public BattleUnitSpriteSet GetAttackerSpriteSet()
    {
        return Attacker.Friendly ? ForceMemberSpriteSet : MonsterSpriteSet;
    }

    /// <summary>Sprite set for the unit who is defending this exchange (not screen side).</summary>
    public BattleUnitSpriteSet GetDefenderSpriteSet()
    {
        return Defender.Friendly ? ForceMemberSpriteSet : MonsterSpriteSet;
    }

    private void AssignBattleSprites()
    {
        Logger.Debug("AssignBattleSprites()");
        var attackerSprites = BattleUnitSpriteManager.Get(Attacker);
        var defenderSprites = BattleUnitSpriteManager.Get(Defender);

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
        switch (Effect)
        {
            case Effects.ArtilleryExplosion:
                BuildArtilleryExplosionBattleScene();
                break;

            default: // Effects.NormalAttack
                BuildNormalAttackBattleScene();
                break;
        }
    }

    private void BuildArtilleryExplosionBattleScene()
    {
        // Debug scrubber: one entry per pose so Left/Right steps distinct frames.
        var poseCopies = Logger.InDebugMode() ? 1 : GameConstants.Animations.AttackDelay;
        var attackPoseCopies = poseCopies * 3;
        var dissolveCopies = Logger.InDebugMode() ? 1 : GameConstants.Animations.Dissolve.NumberOfFrameCopies;
        const int preHitIdleFrames = 7;

        if (Logger.InDebugMode())
        {
            AppendAttackerIdlePrefix();
        }

        var count = MonsterSpriteSet.Attack.Count - 1;

        // Wind-up: same length on both sides so sequences stay aligned.
        ForceMemberSpriteSet.BuildBattleSequence(ForceMemberSpriteSet.GetIdleFrame(0), preHitIdleFrames);
        MonsterSpriteSet.BuildBattleSequence(MonsterSpriteSet.GetIdleFrame(0), preHitIdleFrames);

        // Damage on first frame of the attacker's last attack pose (monster sheet paces artillery).
        DamageApplyFrame = preHitIdleFrames + count * attackPoseCopies;

        for (var i = 0; i < MonsterSpriteSet.Attack.Count; i++)
        {
            MonsterSpriteSet.BuildBattleSequence(MonsterSpriteSet.GetAttackFrame(i), attackPoseCopies);

            var invertFlag = Hit && i == count;

            // Same copy count as artillery poses — unequal lengths caused Max's sequence to
            // end early and GetBattleSequenceFrame modulo-wrapped back into hit-jitter.
            ForceMemberSpriteSet.BuildBattleSequence(
                ForceMemberSpriteSet.GetIdleFrame(i),
                attackPoseCopies,
                invertFlag);
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

        // Pad whichever side is shorter (hold last frame — fully dissolved stays clear).
        var syncedLength = Math.Max(
            ForceMemberSpriteSet.BattleSequence.Count,
            MonsterSpriteSet.BattleSequence.Count);
        ForceMemberSpriteSet.PadBattleSequenceToLength(syncedLength);
        MonsterSpriteSet.PadBattleSequenceToLength(syncedLength);
    }

    private void BuildNormalAttackBattleScene()
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
            // Monster attacks force member — pace from monster attack sheet.
            var count = MonsterSpriteSet.Attack.Count - 1;
            DamageApplyFrame = count * poseCopies;

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
            // Force member attacks monster — pace from force attack sheet (not monster sheet).
            var count = ForceMemberSpriteSet.Attack.Count - 1;
            DamageApplyFrame = count * poseCopies;

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

    public void DetermineAttackEffect()
    {
        Effect = Attacker.Name switch
        {
            UnitName.Artillery => Effects.ArtilleryExplosion,
            UnitName.Bowrider => Effects.ArtilleryExplosion,
            _ => Effects.NormalAttack
        };
    }

    public void Reset()
    {
        Attacker = null!;
        Defender = null!;

        Hit = false;
        Crit = false;
        Damage = 0;
        DamageApplyFrame = 0;

        ForceMemberSpriteSet.Reset();
        MonsterSpriteSet.Reset();

        // We might want to default to something like "no effect"
        Effect = Effects.NormalAttack;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine("AttackContext:");
        sb.AppendLine("Effect:");
        sb.AppendLine(Effect.ToString());
        sb.AppendLine("Attacker:");
        sb.AppendLine(Attacker.CombatToString());
        sb.AppendLine("Defender:");
        sb.AppendLine(Defender.CombatToString());

        return sb.ToString();
    }
}