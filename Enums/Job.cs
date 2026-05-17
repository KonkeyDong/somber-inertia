namespace SomberInertia.Enums;

// Class is a reserved word; job will have to do.
[Flags]
public enum Job
{
    Any = 0,
    Swordsman = 1 << 0,
    Hero = 1 << 1,
    Warrior = 1 << 2,
    Gladiator = 1 << 3,
    Archer = 1 << 4,
    BowMaster = 1 << 5,
    Mage = 1 << 6,
    Wizard = 1 << 7,
    Knight = 1 << 8,
    Paladin = 1 << 9,
    Birdman = 1 << 10,
    SkyWarrior = 1 << 11,
    Ninja = 1 << 12,
    Samurai = 1 << 13,
    Healer = 1 << 14,
    Vicar = 1 << 15,
    Sniper = 1 << 16,
    Robot = 1 << 17,
    Cyborg = 1 << 18,
    Dragon = 1 << 19,
    GreatDragon = 1 << 20,
    AssaultKnight = 1 << 21,
    StrikeKnight = 1 << 22,
    SkyKnight = 1 << 23,
    SkyLord = 1 << 24,
    SkyBaron = 1 << 25,
}