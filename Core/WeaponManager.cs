using SomberInertia.Enums;

namespace SomberInertia.Core;

public static class WeaponManager
{
    private static readonly Dictionary<WeaponName, Weapon> _weaponsLookup = new();

    public static void Initialize()
    {
        _weaponsLookup.Clear();

        // Unarmed
        _weaponsLookup[WeaponName.Unarmed] = new Weapon(WeaponName.Unarmed.GetDisplayName(), 0, WeaponType.Unarmed, new WeaponRange(1, 1), "", Job.Any);

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

        Logger.Error($"WeaponManager::Create(): Unknown weapon [{weaponName}].");
        throw new ArgumentException($"unknown weapon: [{weaponName}].");
    }

    private static void BuildSwords()
    {
        var weaponType = WeaponType.Sword;
        var weaponRange = new WeaponRange(1, 1);

        _weaponsLookup[WeaponName.ShortSword] = new Weapon(WeaponName.ShortSword.GetDisplayName(), 5, weaponType, weaponRange, "", Job.Swordsman | Job.Warrior | Job.Birdman);
        _weaponsLookup[WeaponName.MiddleSword] = new Weapon(WeaponName.MiddleSword.GetDisplayName(), 8, weaponType, weaponRange, "", Job.Swordsman | Job.Warrior | Job.Birdman);
        _weaponsLookup[WeaponName.LongSword] = new Weapon(WeaponName.LongSword.GetDisplayName(), 12, weaponType, weaponRange, "", Job.Warrior | Job.Swordsman);
        _weaponsLookup[WeaponName.SteelSword] = new Weapon(WeaponName.SteelSword.GetDisplayName(), 18, weaponType, weaponRange, "", Job.Hero | Job.Ninja | Job.SkyWarrior | Job.Samurai);
        _weaponsLookup[WeaponName.BroadSword] = new Weapon(WeaponName.BroadSword.GetDisplayName(), 20, weaponType, weaponRange, "", Job.Hero | Job.Ninja | Job.SkyWarrior | Job.Samurai);
        _weaponsLookup[WeaponName.DoomBlade] = new Weapon(WeaponName.DoomBlade.GetDisplayName(), 25, weaponType, weaponRange, "", Job.Hero | Job.Ninja | Job.SkyWarrior | Job.Samurai);
        _weaponsLookup[WeaponName.Katana] = new Weapon(WeaponName.Katana.GetDisplayName(), 30, weaponType, weaponRange, "", Job.Hero | Job.Ninja | Job.SkyWarrior | Job.Samurai);
        _weaponsLookup[WeaponName.SwordOfLight] = new Weapon(WeaponName.SwordOfLight.GetDisplayName(), 36, weaponType, weaponRange, "BOLT 2", Job.Hero | Job.SkyWarrior);
        _weaponsLookup[WeaponName.SwordOfDarkness] = new Weapon(WeaponName.SwordOfDarkness.GetDisplayName(), 40, weaponType, weaponRange, "DESOUL 1", Job.Hero | Job.SkyWarrior, true);
        _weaponsLookup[WeaponName.ChaosBreaker] = new Weapon(WeaponName.ChaosBreaker.GetDisplayName(), 40, weaponType, weaponRange, "FREEZE 3", Job.Hero | Job.SkyWarrior);
    }

    private static void BuildAxes()
    {
        var weaponType = WeaponType.Axe;
        var weaponRange = new WeaponRange(1, 1);

        _weaponsLookup[WeaponName.HandAxe] = new Weapon(WeaponName.HandAxe.GetDisplayName(), 7, weaponType, weaponRange, "", Job.Warrior);
        _weaponsLookup[WeaponName.MiddleAxe] = new Weapon(WeaponName.MiddleAxe.GetDisplayName(), 11, weaponType, weaponRange, "", Job.Warrior);
        _weaponsLookup[WeaponName.BattleAxe] = new Weapon(WeaponName.BattleAxe.GetDisplayName(), 16, weaponType, weaponRange, "", Job.Warrior);
        _weaponsLookup[WeaponName.HeatAxe] = new Weapon(WeaponName.HeatAxe.GetDisplayName(), 22, weaponType, weaponRange, "BLAZE 2", Job.Gladiator);
        _weaponsLookup[WeaponName.GreatAxe] = new Weapon(WeaponName.GreatAxe.GetDisplayName(), 26, weaponType, weaponRange, "", Job.Gladiator);
        _weaponsLookup[WeaponName.Atlas] = new Weapon(WeaponName.Atlas.GetDisplayName(), 33, weaponType, weaponRange, "BLAZE 3", Job.Gladiator);
    }

    private static void BuildStaves()
    {
        var weaponType = WeaponType.Staff;
        var weaponRange = new WeaponRange(1, 1);

        _weaponsLookup[WeaponName.WoodenStaff] = new Weapon(WeaponName.WoodenStaff.GetDisplayName(), 5, weaponType, weaponRange, "", Job.Healer | Job.Mage);
        _weaponsLookup[WeaponName.PowerStaff] = new Weapon(WeaponName.PowerStaff.GetDisplayName(), 8, weaponType, weaponRange, "", Job.Healer | Job.Mage);
        _weaponsLookup[WeaponName.GuardianStaff] = new Weapon(WeaponName.GuardianStaff.GetDisplayName(), 12, weaponType, weaponRange, "", Job.Vicar | Job.Wizard);
        _weaponsLookup[WeaponName.HolyStaff] = new Weapon(WeaponName.HolyStaff.GetDisplayName(), 18, weaponType, weaponRange, "BLAZE 2", Job.Vicar);
        _weaponsLookup[WeaponName.DemonRod] = new Weapon(WeaponName.DemonRod.GetDisplayName(), 20, weaponType, weaponRange, "DRAINS MP", Job.Wizard);
    }

    private static void BuildArrows()
    {
        var weaponType = WeaponType.Arrow;

        _weaponsLookup[WeaponName.WoodenArrow] = new Weapon(WeaponName.WoodenArrow.GetDisplayName(), 8, weaponType, new WeaponRange(2, 2), "", Job.Archer | Job.AssaultKnight);
        _weaponsLookup[WeaponName.SteelArrow] = new Weapon(WeaponName.SteelArrow.GetDisplayName(), 13, weaponType, new WeaponRange(2, 2), "", Job.Archer | Job.AssaultKnight);
        _weaponsLookup[WeaponName.ElvenArrow] = new Weapon(WeaponName.ElvenArrow.GetDisplayName(), 18, weaponType, new WeaponRange(2, 3), "", Job.Archer | Job.Sniper | Job.BowMaster | Job.AssaultKnight | Job.StrikeKnight);
        _weaponsLookup[WeaponName.AssaultShell] = new Weapon(WeaponName.AssaultShell.GetDisplayName(), 27, weaponType, new WeaponRange(2, 3), "", Job.StrikeKnight | Job.BowMaster | Job.Sniper);
        _weaponsLookup[WeaponName.BusterShot] = new Weapon(WeaponName.BusterShot.GetDisplayName(), 35, weaponType, new WeaponRange(2, 3), "", Job.StrikeKnight | Job.BowMaster | Job.Sniper);
    }

    private static void BuildSpears()
    {
        var weaponType = WeaponType.Spear;
        var weaponRange = new WeaponRange(1, 2);

        Logger.Warning("WeaponManager::BuildSpears(): need to revisit which jobs can throw spears.");
        _weaponsLookup[WeaponName.Spear] = new Weapon(WeaponName.Spear.GetDisplayName(), 8, weaponType, weaponRange, "", Job.Knight | Job.SkyKnight);
        _weaponsLookup[WeaponName.Spear] = new Weapon(WeaponName.Spear.GetDisplayName(), 8, weaponType, weaponRange, "", Job.Knight | Job.SkyKnight);
    }

    private static void BuildLances()
    {
        var weaponType = WeaponType.Lance;
        var weaponRange = new WeaponRange(1, 1);

        _weaponsLookup[WeaponName.BronzeLance] = new Weapon(WeaponName.BronzeLance.GetDisplayName(), 9, weaponType, weaponRange, "", Job.Knight | Job.SkyKnight);
        _weaponsLookup[WeaponName.SteelLance] = new Weapon(WeaponName.SteelLance.GetDisplayName(), 18, weaponType, weaponRange, "", Job.Paladin | Job.SkyBaron | Job.SkyLord);
        _weaponsLookup[WeaponName.ChromeLance] = new Weapon(WeaponName.ChromeLance.GetDisplayName(), 22, weaponType, weaponRange, "", Job.Paladin | Job.SkyBaron | Job.SkyLord);
        _weaponsLookup[WeaponName.Halberd] = new Weapon(WeaponName.Halberd.GetDisplayName(), 25, weaponType, weaponRange, "BOLT 1", Job.Paladin | Job.SkyBaron | Job.SkyLord);
        _weaponsLookup[WeaponName.DevilLance] = new Weapon(WeaponName.Halberd.GetDisplayName(), 35, weaponType, weaponRange, "", Job.Paladin | Job.SkyBaron | Job.SkyLord, true);
        _weaponsLookup[WeaponName.Valkyrie] = new Weapon(WeaponName.Valkyrie.GetDisplayName(), 35, weaponType, weaponRange, "", Job.Paladin | Job.SkyBaron | Job.SkyLord);
    }
}