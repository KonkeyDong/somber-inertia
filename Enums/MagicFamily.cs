namespace SomberInertia.Enums;

// A unit can have at most four distinct spells, but some of the
// spells can have several levels. Some characters could have two
// buffing or debuffing spells, which wouldn't work if we just had
// a singular bucket for magic buffs/debuffs (see magic type). 
// I decided that we want to separate out the spells into a family
// of spells where each family can go ingo its own buckets: 
// Blaze level 1 to 4 go into the Blaze bucket, same with Freeze, 
// Bolt, Desoul, etc. It's another enum to maintain, but it's for
// maintenance than having raw strings.
public enum MagicFamily
{
    Blaze,
    Freeze,
    Bolt,
    Heal,
    Aura,
    Slow,
    Quick,
    Desoul,
    Dispel,
    Muddle,
    Sleep,
    Egress,
    Detox,
    Shield,
    Boost,
    NoSpell,
}