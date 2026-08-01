using SomberInertia.Enums;

namespace SomberInertia.Core.Combat.Spells;

public static class MagicDatabase
{
    private static readonly Dictionary<MagicName, MagicData> _spells = new();

    public static void Initialize()
    {
        _spells.Clear();

        RegisterFireMagic();
        RegisterIceMagic();
        RegisterLightningMagic();
        RegisterHealMagic();
        RegisterBuffMagic();
        RegisterDebuffMagic();
        RegisterMiscMagic();
    }

    public static MagicData Get(MagicName name)
    {
        if (_spells.TryGetValue(name, out var data))
        {
            return data;
        }

        Logger.Error($"MagicDatabase.Get(): Unknown spell [{name}]. Returning NoSpell.");
        return _spells[MagicName.NoSpell];
    }

    /// <summary>
    /// MP cost helper. Item-cast spells are free.
    /// </summary>
    public static int GetMPCost(MagicName name, bool fromItem = false)
    {
        if (fromItem)
        {
            return 0;
        }

        return Get(name).MPCost;
    }

    public static void Cast(MagicName name, MagicContext context, bool fromItem = false)
    {
        var data = Get(name);

        if (!fromItem && context.Caster.MP.Current < data.MPCost)
        {
            Logger.Warning($"Not enough MP to cast [{name}].");
            return;
        }

        if (!fromItem && data.MPCost > 0)
        {
            context.Caster.MP.Current -= data.MPCost;
        }

        ExecuteEffect(data, context);
    }

    private static void ExecuteEffect(MagicData data, MagicContext context)
    {
        switch (data.EffectType)
        {
            case MagicEffectType.Damage:
                foreach (var target in context.Targets)
                {
                    if (context.Caster.Friendly != target.Friendly)
                    {
                        CombatSystem.MagicAttack(context.Caster, target, data.EffectValue, data.MagicType);
                    }
                }
                break;

            case MagicEffectType.Heal:
                foreach (var target in context.Targets)
                {
                    if (context.Caster.Friendly == target.Friendly)
                    {
                        Logger.Warning("HealEffect: change CombatSystem.MagicAttack to Heal.");
                        CombatSystem.MagicAttack(context.Caster, target, data.EffectValue, data.MagicType);
                    }
                }
                break;

            case MagicEffectType.Egress:
                Logger.Info("Egress: escaping battle (not yet implemented).");
                break;

            case MagicEffectType.Desoul:
                Logger.Info("Desoul: not yet implemented.");
                break;

            case MagicEffectType.None:
            default:
                Logger.Warning($"No effect for spell [{data.Name}].");
                break;
        }
    }

    private static void Register(MagicData data)
    {
        _spells[data.Name] = data;
    }

    private static MagicData Make(
        MagicName name,
        int level,
        int mpCost,
        MagicType magicType,
        Range distanceRange,
        Range targetRange,
        bool offensive,
        MagicEffectType effectType,
        int effectValue)
    {
        return new MagicData
        {
            Name = name,
            Level = level,
            MPCost = mpCost,
            MagicType = magicType,
            DistanceRange = distanceRange,
            TargetRange = targetRange,
            Offensive = offensive,
            EffectType = effectType,
            EffectValue = effectValue
        };
    }

    private static void RegisterFireMagic()
    {
        var type = MagicType.Fire;
        var distance = new Range(1, 2);

        Register(Make(MagicName.Blaze1, 1, 2, type, distance, new Range(0, 0), true, MagicEffectType.Damage, 7));
        Register(Make(MagicName.Blaze2, 2, 5, type, distance, new Range(0, 1), true, MagicEffectType.Damage, 8));
        Register(Make(MagicName.Blaze3, 3, 8, type, distance, new Range(0, 1), true, MagicEffectType.Damage, 12));
        Register(Make(MagicName.Blaze4, 4, 8, type, distance, new Range(0, 0), true, MagicEffectType.Damage, 32));
    }

    private static void RegisterIceMagic()
    {
        var type = MagicType.Ice;

        Register(Make(MagicName.Freeze1, 1, 3, type, new Range(1, 2), new Range(0, 0), true, MagicEffectType.Damage, 8));
        Register(Make(MagicName.Freeze2, 2, 7, type, new Range(1, 2), new Range(0, 1), true, MagicEffectType.Damage, 10));
        Register(Make(MagicName.Freeze3, 3, 10, type, new Range(1, 3), new Range(0, 1), true, MagicEffectType.Damage, 15));
        Register(Make(MagicName.Freeze4, 4, 10, type, new Range(1, 4), new Range(0, 0), true, MagicEffectType.Damage, 40));
    }

    private static void RegisterLightningMagic()
    {
        var type = MagicType.Lightning;

        Register(Make(MagicName.Bolt1, 1, 8, type, new Range(1, 2), new Range(0, 1), true, MagicEffectType.Damage, 12));
        Register(Make(MagicName.Bolt2, 2, 15, type, new Range(1, 3), new Range(0, 2), true, MagicEffectType.Damage, 13));
        Register(Make(MagicName.Bolt3, 3, 20, type, new Range(1, 3), new Range(0, 2), true, MagicEffectType.Damage, 20));
        Register(Make(MagicName.Bolt4, 4, 20, type, new Range(1, 3), new Range(0, 0), true, MagicEffectType.Damage, 48));
    }

    private static void RegisterHealMagic()
    {
        var type = MagicType.Heal;
        var single = new Range(0, 0);

        Register(Make(MagicName.Heal1, 1, 3, type, new Range(0, 1), single, false, MagicEffectType.Heal, 12));
        Register(Make(MagicName.Heal2, 2, 6, type, new Range(0, 2), single, false, MagicEffectType.Heal, 12));
        Register(Make(MagicName.Heal3, 3, 10, type, new Range(0, 3), single, false, MagicEffectType.Heal, 24));
        Register(Make(MagicName.Heal4, 4, 15, type, new Range(0, 1), single, false, MagicEffectType.Heal, 1000));

        Register(Make(MagicName.Aura1, 1, 7, type, new Range(0, 3), new Range(0, 1), false, MagicEffectType.Heal, 12));
        Register(Make(MagicName.Aura2, 2, 11, type, new Range(0, 3), new Range(0, 2), false, MagicEffectType.Heal, 12));
        Register(Make(MagicName.Aura3, 3, 15, type, new Range(0, 3), new Range(0, 2), false, MagicEffectType.Heal, 24));
        Register(Make(MagicName.Aura4, 4, 18, type, new Range(0, 1000), new Range(0, 1000), false, MagicEffectType.Heal, 1000));
    }

    private static void RegisterBuffMagic()
    {
    }

    private static void RegisterDebuffMagic()
    {
    }

    private static void RegisterMiscMagic()
    {
        var type = MagicType.Misc;

        Register(Make(MagicName.Egress1, 1, 8, type, new Range(0, 0), new Range(0, 0), false, MagicEffectType.Egress, 0));
        Register(Make(MagicName.NoSpell, 1, 0, type, new Range(0, 0), new Range(0, 0), false, MagicEffectType.None, 0));
        Register(Make(MagicName.Desoul1, 1, 8, type, new Range(1, 2), new Range(0, 0), true, MagicEffectType.Desoul, 0));
        Register(Make(MagicName.Desoul2, 2, 15, type, new Range(1, 2), new Range(0, 1), true, MagicEffectType.Desoul, 0));
    }
}