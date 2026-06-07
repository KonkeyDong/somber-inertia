namespace SomberInertia.Enums;

public static class MagicFamilyExtensions
{
    public static string GetBaseName(this MagicFamily magicFamily)
    {
        return magicFamily switch
        {
            MagicFamily.NoSpell => "NoSpell",

            _ => magicFamily.ToString(),
        };
    }
}