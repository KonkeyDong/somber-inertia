using SomberInertia.Core.Combat;
using SomberInertia.Enums;
using System.Collections.Generic;

namespace SomberInertia.Core.Combat;

public static class MagicManager
{
    private static readonly Dictionary<string, Magic> _MagicLookup = new();

    public static void Initialize()
    {
        _MagicLookup.Clear();

        // Register all Magics here
        RegisterFireMagic();
        // RegisterHealMagics();
        // RegisterBuffMagics();
        // ... add more categories as needed
    }

    // public static Magic Create(MagicName MagicName)
    // {
    //     if (_MagicsLookup.TryGetValue(MagicName, out var Magic))
    //     {
    //         return Magic;                    // You can decide later if you want to return a copy or the same instance
    //     }

    //     throw new InvalidOperationException($"MagicDatabase.Create(): Unknown Magic [{MagicName}].");
    // }

    // Example category
    private static void RegisterFireMagic()
    {
        _MagicLookup["Fire 1"] = new Magic(
            name: MagicType.Fire.ToString(),
            level: 1,
            MPCost: 2,
            magicType: MagicType.Fire,
            distanceRange: new Range(1, 2),
            targetRange: new Range(0, 0),
            effect: new DamageEffect(8)
        );

        _MagicLookup["Fire 2"] = new Magic(
            name: "Fire 2",
            level: 2,
            MPCost: 5,
            magicType: MagicType.Fire,
            distanceRange: new Range(1, 2),
            targetRange: new Range(0, 1),
            effect: new DamageEffect(12)
        );
    }

    // Add more methods like RegisterHealMagics(), RegisterBuffMagics(), etc.
}