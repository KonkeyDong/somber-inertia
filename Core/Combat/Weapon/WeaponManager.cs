using SomberInertia.Enums;
using SomberInertia.Core.Combat.Spells;

namespace SomberInertia.Core.Combat.Weapon;

public static class WeaponManager
{
    private static readonly Dictionary<WeaponName, Weapon> _weaponsLookup = new();

    public static void Initialize()
    {
        _weaponsLookup.Clear();

        // Unarmed
        _weaponsLookup[WeaponName.Unarmed] = new Weapon(WeaponName.Unarmed, 0, WeaponType.Unarmed, new Range(1, 1), null, Job.Any);

        BuildSwords();
        BuildAxes();
        BuildStaves();
        BuildArrows();
        BuildSpears();
        BuildLances();
    }

    public static Weapon Create(WeaponName weaponName)
    {
        if (_weaponsLookup.TryGetValue(weaponName, out var weapon))
        {
            // Return a brand new copy
            return weapon;
        }

        throw new InvalidOperationException($"WeaponManager::Create(): Unknown weapon [{weaponName}].");
    }

    private static void BuildSwords()
    {
        var weaponType = WeaponType.Sword;
        var weaponRange = new Range(1, 1);

        _weaponsLookup[WeaponName.ShortSword] = new Weapon(WeaponName.ShortSword, 5, weaponType, weaponRange, null, Job.Swordsman | Job.Warrior | Job.Birdman);
        _weaponsLookup[WeaponName.MiddleSword] = new Weapon(WeaponName.MiddleSword, 8, weaponType, weaponRange, null, Job.Swordsman | Job.Warrior | Job.Birdman);
        _weaponsLookup[WeaponName.LongSword] = new Weapon(WeaponName.LongSword, 12, weaponType, weaponRange, null, Job.Warrior | Job.Swordsman);
        _weaponsLookup[WeaponName.SteelSword] = new Weapon(WeaponName.SteelSword, 18, weaponType, weaponRange, null, Job.Hero | Job.Ninja | Job.SkyWarrior | Job.Samurai);
        _weaponsLookup[WeaponName.BroadSword] = new Weapon(WeaponName.BroadSword, 20, weaponType, weaponRange, null, Job.Hero | Job.Ninja | Job.SkyWarrior | Job.Samurai);
        _weaponsLookup[WeaponName.DoomBlade] = new Weapon(WeaponName.DoomBlade, 25, weaponType, weaponRange, null, Job.Hero | Job.Ninja | Job.SkyWarrior | Job.Samurai);
        _weaponsLookup[WeaponName.Katana] = new Weapon(WeaponName.Katana, 30, weaponType, weaponRange, null, Job.Hero | Job.Ninja | Job.SkyWarrior | Job.Samurai);
        _weaponsLookup[WeaponName.SwordOfLight] = new Weapon(WeaponName.SwordOfLight, 36, weaponType, weaponRange, MagicManager.CreateWithNoMPCost(MagicName.Bolt2), Job.Hero | Job.SkyWarrior);
        _weaponsLookup[WeaponName.SwordOfDarkness] = new Weapon(WeaponName.SwordOfDarkness, 40, weaponType, weaponRange, MagicManager.CreateWithNoMPCost(MagicName.Desoul1), Job.Hero | Job.SkyWarrior, true);
        _weaponsLookup[WeaponName.ChaosBreaker] = new Weapon(WeaponName.ChaosBreaker, 40, weaponType, weaponRange, MagicManager.CreateWithNoMPCost(MagicName.Freeze3), Job.Hero | Job.SkyWarrior);
    }

    private static void BuildAxes()
    {
        var weaponType = WeaponType.Axe;
        var weaponRange = new Range(1, 1);

        _weaponsLookup[WeaponName.HandAxe] = new Weapon(WeaponName.HandAxe, 7, weaponType, weaponRange, null, Job.Warrior);
        _weaponsLookup[WeaponName.MiddleAxe] = new Weapon(WeaponName.MiddleAxe, 11, weaponType, weaponRange, null, Job.Warrior);
        _weaponsLookup[WeaponName.BattleAxe] = new Weapon(WeaponName.BattleAxe, 16, weaponType, weaponRange, null, Job.Warrior);
        _weaponsLookup[WeaponName.HeatAxe] = new Weapon(WeaponName.HeatAxe, 22, weaponType, weaponRange, MagicManager.CreateWithNoMPCost(MagicName.Blaze2), Job.Gladiator);
        _weaponsLookup[WeaponName.GreatAxe] = new Weapon(WeaponName.GreatAxe, 26, weaponType, weaponRange, null, Job.Gladiator);
        _weaponsLookup[WeaponName.Atlas] = new Weapon(WeaponName.Atlas, 33, weaponType, weaponRange, MagicManager.CreateWithNoMPCost(MagicName.Blaze3), Job.Gladiator);
    }

    private static void BuildStaves()
    {
        var weaponType = WeaponType.Staff;
        var weaponRange = new Range(1, 1);

        _weaponsLookup[WeaponName.WoodenStaff] = new Weapon(WeaponName.WoodenStaff, 5, weaponType, weaponRange, null, Job.Healer | Job.Mage);
        _weaponsLookup[WeaponName.PowerStaff] = new Weapon(WeaponName.PowerStaff, 8, weaponType, weaponRange, null, Job.Healer | Job.Mage);
        _weaponsLookup[WeaponName.GuardianStaff] = new Weapon(WeaponName.GuardianStaff, 12, weaponType, weaponRange, null, Job.Vicar | Job.Wizard);
        _weaponsLookup[WeaponName.HolyStaff] = new Weapon(WeaponName.HolyStaff, 18, weaponType, weaponRange, MagicManager.CreateWithNoMPCost(MagicName.Blaze2), Job.Vicar);
        _weaponsLookup[WeaponName.DemonRod] = new Weapon(WeaponName.DemonRod, 20, weaponType, weaponRange, null /*"DRAINS MP"*/, Job.Wizard);
    }

    private static void BuildArrows()
    {
        var weaponType = WeaponType.Arrow;

        _weaponsLookup[WeaponName.WoodenArrow] = new Weapon(WeaponName.WoodenArrow, 8, weaponType, new Range(2, 2), null, Job.Archer | Job.AssaultKnight);
        _weaponsLookup[WeaponName.SteelArrow] = new Weapon(WeaponName.SteelArrow, 13, weaponType, new Range(2, 2), null, Job.Archer | Job.AssaultKnight);
        _weaponsLookup[WeaponName.ElvenArrow] = new Weapon(WeaponName.ElvenArrow, 18, weaponType, new Range(2, 3), null, Job.Archer | Job.Sniper | Job.BowMaster | Job.AssaultKnight | Job.StrikeKnight);
        _weaponsLookup[WeaponName.AssaultShell] = new Weapon(WeaponName.AssaultShell, 27, weaponType, new Range(2, 3), null, Job.StrikeKnight | Job.BowMaster | Job.Sniper);
        _weaponsLookup[WeaponName.BusterShot] = new Weapon(WeaponName.BusterShot, 35, weaponType, new Range(2, 3), null, Job.StrikeKnight | Job.BowMaster | Job.Sniper);
    }

    private static void BuildSpears()
    {
        var weaponType = WeaponType.Spear;
        var weaponRange = new Range(1, 2);

        _weaponsLookup[WeaponName.Spear] = new Weapon(WeaponName.Spear, 8, weaponType, weaponRange, null, Job.Knight | Job.SkyKnight);
        _weaponsLookup[WeaponName.PowerSpear] = new Weapon(WeaponName.PowerSpear, 8, weaponType, weaponRange, null, Job.Knight | Job.SkyKnight);
    }

    private static void BuildLances()
    {
        var weaponType = WeaponType.Lance;
        var weaponRange = new Range(1, 1);

        _weaponsLookup[WeaponName.BronzeLance] = new Weapon(WeaponName.BronzeLance, 9, weaponType, weaponRange, null, Job.Knight | Job.SkyKnight);
        _weaponsLookup[WeaponName.SteelLance] = new Weapon(WeaponName.SteelLance, 18, weaponType, weaponRange, null, Job.Paladin | Job.SkyBaron | Job.SkyLord);
        _weaponsLookup[WeaponName.ChromeLance] = new Weapon(WeaponName.ChromeLance, 22, weaponType, weaponRange, null, Job.Paladin | Job.SkyBaron | Job.SkyLord);
        _weaponsLookup[WeaponName.Halberd] = new Weapon(WeaponName.Halberd, 25, weaponType, weaponRange, MagicManager.CreateWithNoMPCost(MagicName.Bolt1), Job.Paladin | Job.SkyBaron | Job.SkyLord);
        _weaponsLookup[WeaponName.DevilLance] = new Weapon(WeaponName.DevilLance, 35, weaponType, weaponRange, null, Job.Paladin | Job.SkyBaron | Job.SkyLord, true);
        _weaponsLookup[WeaponName.Valkyrie] = new Weapon(WeaponName.Valkyrie, 35, weaponType, weaponRange, null, Job.Paladin | Job.SkyBaron | Job.SkyLord);
    }
}