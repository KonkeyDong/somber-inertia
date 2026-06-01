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
}