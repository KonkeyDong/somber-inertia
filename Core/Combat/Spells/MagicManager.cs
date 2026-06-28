using SomberInertia.Core.Combat;
using SomberInertia.Enums;
using System.Collections.Generic;

namespace SomberInertia.Core.Combat.Spells;

public static class MagicManager
{
    private static readonly Dictionary<MagicName, Magic> _MagicLookup = new();

    public static void Initialize()
    {
        _MagicLookup.Clear();

        RegisterFireMagic();
        RegisterIceMagic();
        RegisterLightningMagic();
        RegisterHealMagic();
        RegisterBuffMagic();
        RegisterDebuffMagic();
        RegisterMiscMagic();
    }

    public static Magic Create(MagicName magicName)
    {
        if (_MagicLookup.TryGetValue(magicName, out var spell))
        {
            // Return a brand new copy
            return spell;
        }

        throw new InvalidOperationException($"Unknown spell [{magicName}].");
    }

    // Used for attaching spells to weapons to be used as items.
    // Spells casted through items are always free, but can break
    // with repeated use.
    public static Magic CreateWithNoMPCost(MagicName magicName)
    {
        var spell = Create(magicName);
        spell.MPCost = 0;

        return spell;
    }

    private static void RegisterFireMagic()
    {
        var magicType = MagicType.Fire;
        var distanceRange = new Range(1, 2);
        var offensive = true;

        _MagicLookup[MagicName.Blaze1] = new Magic(
            name: MagicName.Blaze1,
            level: 1,
            MPCost: 2,
            magicType: magicType,
            distanceRange: distanceRange,
            targetRange: new Range(0, 0),
            offensive: offensive,
            effect: new DamageEffect(7)
        );

        _MagicLookup[MagicName.Blaze2] = new Magic(
            name: MagicName.Blaze2,
            level: 2,
            MPCost: 5,
            magicType: magicType,
            distanceRange: distanceRange,
            targetRange: new Range(0, 1),
            offensive: offensive,
            effect: new DamageEffect(8)
        );

        _MagicLookup[MagicName.Blaze3] = new Magic(
            name: MagicName.Blaze3,
            level: 3,
            MPCost: 8,
            magicType: magicType,
            distanceRange: distanceRange,
            targetRange: new Range(0, 1),
            offensive: offensive,
            effect: new DamageEffect(12)
        );

        _MagicLookup[MagicName.Blaze4] = new Magic(
            name: MagicName.Blaze4,
            level: 4,
            MPCost: 8,
            magicType: magicType,
            distanceRange: distanceRange,
            targetRange: new Range(0, 0),
            offensive: offensive,
            effect: new DamageEffect(32)
        );
    }

    private static void RegisterIceMagic()
    {
        var magicType = MagicType.Ice;
        var offensive = true;

        _MagicLookup[MagicName.Freeze1] = new Magic(
            name: MagicName.Freeze1,
            level: 1,
            MPCost: 3,
            magicType: magicType,
            distanceRange: new Range(1, 2),
            targetRange: new Range(0, 0),
            offensive: offensive,
            effect: new DamageEffect(8)
        );

        _MagicLookup[MagicName.Freeze2] = new Magic(
            name: MagicName.Freeze2,
            level: 2,
            MPCost: 7,
            magicType: magicType,
            distanceRange: new Range(1, 2),
            targetRange: new Range(0, 1),
            offensive: offensive,
            effect: new DamageEffect(10)
        );

        _MagicLookup[MagicName.Freeze3] = new Magic(
            name: MagicName.Freeze3,
            level: 3,
            MPCost: 10,
            magicType: magicType,
            distanceRange: new Range(1, 3),
            targetRange: new Range(0, 1),
            offensive: offensive,
            effect: new DamageEffect(15)
        );

        _MagicLookup[MagicName.Freeze4] = new Magic(
            name: MagicName.Freeze4,
            level: 4,
            MPCost: 10,
            magicType: magicType,
            distanceRange: new Range(1, 4),
            targetRange: new Range(0, 0),
            offensive: offensive,
            effect: new DamageEffect(40)
        );
    }

    private static void RegisterLightningMagic()
    {
        var magicType = MagicType.Lightning;
        var offensive = true;

        _MagicLookup[MagicName.Bolt1] = new Magic(
            name: MagicName.Bolt1,
            level: 1,
            MPCost: 8,
            magicType: magicType,
            distanceRange: new Range(1, 2),
            targetRange: new Range(0, 1),
            offensive: offensive,
            effect: new DamageEffect(12)
        );

        _MagicLookup[MagicName.Bolt2] = new Magic(
            name: MagicName.Bolt2,
            level: 2,
            MPCost: 15,
            magicType: magicType,
            distanceRange: new Range(1, 3),
            targetRange: new Range(0, 2),
            offensive: offensive,
            effect: new DamageEffect(13)
        );

        _MagicLookup[MagicName.Bolt3] = new Magic(
            name: MagicName.Bolt3,
            level: 3,
            MPCost: 20,
            magicType: magicType,
            distanceRange: new Range(1, 3),
            targetRange: new Range(0, 2),
            offensive: offensive,
            effect: new DamageEffect(20)
        );

        _MagicLookup[MagicName.Bolt4] = new Magic(
            name: MagicName.Bolt4,
            level: 4,
            MPCost: 20,
            magicType: magicType,
            distanceRange: new Range(1, 3),
            targetRange: new Range(0, 0),
            offensive: offensive,
            effect: new DamageEffect(48)
        );
    }

    private static void RegisterHealMagic()
    {
        var magicType = MagicType.Heal;
        var targetRange = new Range(0, 0);
        var offensive = false;

        // Heal
        _MagicLookup[MagicName.Heal1] = new Magic(
            name: MagicName.Heal1,
            level: 1,
            MPCost: 3,
            magicType: magicType,
            distanceRange: new Range(0, 1),
            targetRange: targetRange,
            offensive: offensive,
            effect: new HealEffect(12)
        );

        _MagicLookup[MagicName.Heal2] = new Magic(
            name: MagicName.Heal2,
            level: 2,
            MPCost: 6,
            magicType: magicType,
            distanceRange: new Range(0, 2),
            targetRange: targetRange,
            offensive: offensive,
            effect: new HealEffect(12)
        );

        _MagicLookup[MagicName.Heal3] = new Magic(
            name: MagicName.Heal3,
            level: 3,
            MPCost: 10,
            magicType: magicType,
            distanceRange: new Range(0, 3),
            targetRange: targetRange,
            offensive: offensive,
            effect: new HealEffect(24)
        );

        _MagicLookup[MagicName.Heal4] = new Magic(
            name: MagicName.Heal4,
            level: 4,
            MPCost: 15, // originally 20, but why make Aura4 18?
            magicType: magicType,
            distanceRange: new Range(0, 1),
            targetRange: targetRange,
            offensive: offensive,
            effect: new HealEffect(1000) // full heal
        );
        
        // Aura
        _MagicLookup[MagicName.Aura1] = new Magic(
            name: MagicName.Aura1,
            level: 1,
            MPCost: 7,
            magicType: magicType,
            distanceRange: new Range(0, 3),
            targetRange: new Range(0, 1),
            offensive: offensive,
            effect: new HealEffect(12)
        );

        _MagicLookup[MagicName.Aura2] = new Magic(
            name: MagicName.Aura2,
            level: 2,
            MPCost: 11,
            magicType: magicType,
            distanceRange: new Range(0, 3),
            targetRange: new Range(0, 2),
            offensive: offensive,
            effect: new HealEffect(12)
        );

        _MagicLookup[MagicName.Aura3] = new Magic(
            name: MagicName.Aura3,
            level: 3,
            MPCost: 15,
            magicType: magicType,
            distanceRange: new Range(0, 3),
            targetRange: new Range(0, 2),
            offensive: offensive,
            effect: new HealEffect(24)
        );

        _MagicLookup[MagicName.Aura4] = new Magic(
            name: MagicName.Aura4,
            level: 4,
            MPCost: 18,
            magicType: magicType,
            distanceRange: new Range(0, 1000), // entire map
            targetRange: new Range(0, 1000), // entire map
            offensive: offensive,
            effect: new HealEffect(1000) // full heal
        );
    }

    public static void RegisterBuffMagic()
    {

    }

    public static void RegisterDebuffMagic()
    {

    }

    public static void RegisterMiscMagic()
    {
        var magicType = MagicType.Misc;

        Logger.Warning("Egress effect is DamageEffect. Change to different effect.");
        _MagicLookup[MagicName.Egress1] = new Magic(
            name: MagicName.Egress1,
            level: 1,
            MPCost: 8,
            magicType: magicType,
            distanceRange: new Range(0, 0),
            targetRange: new Range(0, 0),
            offensive: false,
            effect: new DamageEffect(0)
        );

        // Dummy spell for error returning purposes (although, Logger.Error() should throw an exception)
        _MagicLookup[MagicName.NoSpell] = new Magic(
            name: MagicName.NoSpell,
            level: 1,
            MPCost: 0,
            magicType: magicType,
            distanceRange: new Range(0, 0),
            targetRange: new Range(0, 0),
            offensive: false,
            effect: new DamageEffect(0)
        );

        Logger.Warning("Desoul might need to be registered to a different magic type from Misc.");
        _MagicLookup[MagicName.Desoul1] = new Magic(
            name: MagicName.Desoul1,
            level: 1,
            MPCost: 8,
            magicType: magicType,
            distanceRange: new Range(1, 2),
            targetRange: new Range(0, 0),
            offensive: true,
            effect: new DamageEffect(0)
        );

        _MagicLookup[MagicName.Desoul2] = new Magic(
            name: MagicName.Desoul1,
            level: 2,
            MPCost: 15,
            magicType: magicType,
            distanceRange: new Range(1, 2),
            targetRange: new Range(0, 1),
            offensive: true,
            effect: new DamageEffect(0)
        );
    }
}