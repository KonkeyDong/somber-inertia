namespace SomberInertia.Enums;

public static class MagicNameExtensions
{
    public static string GetDisplayName(this MagicName magicName)
    {
        return magicName switch
        {
            MagicName.Blaze1 => "Blaze level 1",
            MagicName.Blaze2 => "Blaze level 2",
            MagicName.Blaze3 => "Blaze level 3",
            MagicName.Blaze4 => "Blaze level 4",

            MagicName.Freeze1 => "Freeze level 1",
            MagicName.Freeze2 => "Freeze level 2",
            MagicName.Freeze3 => "Freeze level 3",
            MagicName.Freeze4 => "Freeze level 4",

            MagicName.Bolt1 => "Bolt level 1",
            MagicName.Bolt2 => "Bolt level 2",
            MagicName.Bolt3 => "Bolt level 3",
            MagicName.Bolt4 => "Bolt level 4",

            MagicName.Desoul1 => "Desoul level 1",
            MagicName.Desoul2 => "Desoul level 2",

            MagicName.Dispel1 => "Dispel",
            MagicName.Muddle1 => "Muddle",
            MagicName.Sleep1 => "Sleep",
            MagicName.Egress1 => "Egress",
            MagicName.Detox1 => "Detox",
            MagicName.Shield1 => "Shield",
            MagicName.Boost1 => "Boost",

            MagicName.Slow1 => "Slow level 1",
            MagicName.Slow2 => "Slow level 2",
            MagicName.Quick1 => "Quick level 1",
            MagicName.Quick2 => "Quick level 2",

            MagicName.Heal1 => "Heal level 1",
            MagicName.Heal2 => "Heal level 2",
            MagicName.Heal3 => "Heal level 3",
            MagicName.Heal4 => "Heal level 4",

            MagicName.Aura1 => "Aura level 1",
            MagicName.Aura2 => "Aura level 2",
            MagicName.Aura3 => "Aura level 3",
            MagicName.Aura4 => "Aura level 4",

            _ => magicName.ToString()
        };
    }

    public static string GetBaseName(this MagicName magicName, bool capitalize = false)
    {
        var result = magicName switch
        {
            // All Blaze levels share the same sprite
            MagicName.Blaze1 or MagicName.Blaze2 or MagicName.Blaze3 or MagicName.Blaze4 
                => "Blaze",

            // All Freeze levels share the same sprite
            MagicName.Freeze1 or MagicName.Freeze2 or MagicName.Freeze3 or MagicName.Freeze4 
                => "Freeze",

            // All Bolt levels share the same sprite
            MagicName.Bolt1 or MagicName.Bolt2 or MagicName.Bolt3 or MagicName.Bolt4 
                => "Bolt",

            // All Heal levels share the same sprite
            MagicName.Heal1 or MagicName.Heal2 or MagicName.Heal3 or MagicName.Heal4 
                => "Heal",

            // All Aura levels share the same sprite
            MagicName.Aura1 or MagicName.Aura2 or MagicName.Aura3 or MagicName.Aura4 
                => "Aura",

            // Level 1 + Level 2 variants that share the same sprite
            MagicName.Slow1 or MagicName.Slow2   => "Slow",
            MagicName.Quick1 or MagicName.Quick2 => "Quick",
            MagicName.Desoul1 or MagicName.Desoul2 => "Desoul",

            // Single-level spells (no number in filename)
            MagicName.Dispel1 => "Dispel",
            MagicName.Muddle1 => "Muddle",
            MagicName.Sleep1  => "Sleep",
            MagicName.Egress1 => "Egress",
            MagicName.Detox1  => "Detox",
            MagicName.Shield1 => "Shield",
            MagicName.Boost1  => "Boost",

            MagicName.NoSpell => "NoSpell",

            // Fallback (just in case)
            _ => magicName.ToString().ToLowerInvariant()
        };

        return capitalize ? result.ToUpper() : result;
    }
    
    public static MagicFamily ToFamily(this MagicName magicName)
    {
        return magicName.GetBaseName() switch
        {
            "Blaze"  => MagicFamily.Blaze,
            "Freeze" => MagicFamily.Freeze,
            "Bolt"   => MagicFamily.Bolt,
            "Heal"   => MagicFamily.Heal,
            "Aura"   => MagicFamily.Aura,
            "Slow"   => MagicFamily.Slow,
            "Quick"  => MagicFamily.Quick,
            "Desoul" => MagicFamily.Desoul,
            "Dispel" => MagicFamily.Dispel,
            "Muddle" => MagicFamily.Muddle,
            "Sleep"  => MagicFamily.Sleep,
            "Egress" => MagicFamily.Egress,
            "Detox"  => MagicFamily.Detox,
            "Shield" => MagicFamily.Shield,
            "Boost"  => MagicFamily.Boost,
            "NoSpell" => MagicFamily.NoSpell,

            _ => throw new ArgumentException($"Invalid MagicName passed to ToFamily(): {magicName.GetBaseName()}"),
        };
    }
}