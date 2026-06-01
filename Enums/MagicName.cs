namespace SomberInertia.Enums;

// Magic Name + Magic Level = Blaze1 (as an example)
public enum MagicName
{
    Blaze1,
    Blaze2,
    Blaze3,
    Blaze4,

    Freeze1,
    Freeze2,
    Freeze3,
    Freeze4,

    Bolt1,
    Bolt2,
    Bolt3,
    Bolt4,

    Desoul1,
    Desoul2,

    // Dispel (and some other spells) will have
    // one level. Since you have to confirm your
    // spell level choice when selecting a spell
    // before casting, we keep the level number
    // even if the spell only has one level.
    Dispel1,
    Muddle1,
    Sleep1,
    Egress1,
    Detox1,
    Shield1,
    Boost1,
    
    Slow1,
    Slow2,
    Quick1,
    Quick2,

    Heal1,
    Heal2,
    Heal3,
    Heal4,

    Aura1,
    Aura2,
    Aura3,
    Aura4,
}