using SomberInertia.Enums;
using SomberInertia.Core.Combat.Spells;

namespace SomberInertia.Core.Combat.Item.Weapon;

public static class WeaponManager
{
    private static readonly Dictionary<ItemName, Weapon> _weaponsLookup = new();

    public static void Initialize()
    {
        _weaponsLookup.Clear();

        // Unarmed
        _weaponsLookup[ItemName.Unarmed] = new Weapon(ItemName.Unarmed, 0, ItemType.Unarmed, new Range(1, 1), null, Job.Any);

        BuildSwords();
        BuildAxes();
        BuildStaves();
        BuildArrows();
        BuildSpears();
        BuildLances();
    }

    public static Weapon Create(ItemName itemName)
    {
        if (_weaponsLookup.TryGetValue(itemName, out var weapon))
        {
            // Return a brand new copy
            return weapon;
        }

        throw new InvalidOperationException($"WeaponManager::Create(): Unknown weapon [{itemName}].");
    }

    private static void BuildSwords()
    {
        var itemType = ItemType.Sword;
        var weaponRange = new Range(1, 1);

        _weaponsLookup[ItemName.ShortSword] = new Weapon(ItemName.ShortSword, 5, itemType, weaponRange, null, Job.Swordsman | Job.Warrior | Job.Birdman);
        _weaponsLookup[ItemName.MiddleSword] = new Weapon(ItemName.MiddleSword, 8, itemType, weaponRange, null, Job.Swordsman | Job.Warrior | Job.Birdman);
        _weaponsLookup[ItemName.LongSword] = new Weapon(ItemName.LongSword, 12, itemType, weaponRange, null, Job.Warrior | Job.Swordsman);
        _weaponsLookup[ItemName.SteelSword] = new Weapon(ItemName.SteelSword, 18, itemType, weaponRange, null, Job.Hero | Job.Ninja | Job.SkyWarrior | Job.Samurai);
        _weaponsLookup[ItemName.BroadSword] = new Weapon(ItemName.BroadSword, 20, itemType, weaponRange, null, Job.Hero | Job.Ninja | Job.SkyWarrior | Job.Samurai);
        _weaponsLookup[ItemName.DoomBlade] = new Weapon(ItemName.DoomBlade, 25, itemType, weaponRange, null, Job.Hero | Job.Ninja | Job.SkyWarrior | Job.Samurai);
        _weaponsLookup[ItemName.Katana] = new Weapon(ItemName.Katana, 30, itemType, weaponRange, null, Job.Hero | Job.Ninja | Job.SkyWarrior | Job.Samurai);
        _weaponsLookup[ItemName.SwordOfLight] = new Weapon(ItemName.SwordOfLight, 36, itemType, weaponRange, MagicManager.CreateWithNoMPCost(MagicName.Bolt2), Job.Hero | Job.SkyWarrior);
        _weaponsLookup[ItemName.SwordOfDarkness] = new Weapon(ItemName.SwordOfDarkness, 40, itemType, weaponRange, MagicManager.CreateWithNoMPCost(MagicName.Desoul1), Job.Hero | Job.SkyWarrior, true);
        _weaponsLookup[ItemName.ChaosBreaker] = new Weapon(ItemName.ChaosBreaker, 40, itemType, weaponRange, MagicManager.CreateWithNoMPCost(MagicName.Freeze3), Job.Hero | Job.SkyWarrior);
    }

    private static void BuildAxes()
    {
        var itemType = ItemType.Axe;
        var weaponRange = new Range(1, 1);

        _weaponsLookup[ItemName.HandAxe] = new Weapon(ItemName.HandAxe, 7, itemType, weaponRange, null, Job.Warrior);
        _weaponsLookup[ItemName.MiddleAxe] = new Weapon(ItemName.MiddleAxe, 11, itemType, weaponRange, null, Job.Warrior);
        _weaponsLookup[ItemName.BattleAxe] = new Weapon(ItemName.BattleAxe, 16, itemType, weaponRange, null, Job.Warrior);
        _weaponsLookup[ItemName.HeatAxe] = new Weapon(ItemName.HeatAxe, 22, itemType, weaponRange, MagicManager.CreateWithNoMPCost(MagicName.Blaze2), Job.Gladiator);
        _weaponsLookup[ItemName.GreatAxe] = new Weapon(ItemName.GreatAxe, 26, itemType, weaponRange, null, Job.Gladiator);
        _weaponsLookup[ItemName.Atlas] = new Weapon(ItemName.Atlas, 33, itemType, weaponRange, MagicManager.CreateWithNoMPCost(MagicName.Blaze3), Job.Gladiator);
    }

    private static void BuildStaves()
    {
        var itemType = ItemType.Staff;
        var weaponRange = new Range(1, 1);

        _weaponsLookup[ItemName.WoodenStaff] = new Weapon(ItemName.WoodenStaff, 5, itemType, weaponRange, null, Job.Healer | Job.Mage);
        _weaponsLookup[ItemName.PowerStaff] = new Weapon(ItemName.PowerStaff, 8, itemType, weaponRange, null, Job.Healer | Job.Mage);
        _weaponsLookup[ItemName.GuardianStaff] = new Weapon(ItemName.GuardianStaff, 12, itemType, weaponRange, null, Job.Vicar | Job.Wizard);
        _weaponsLookup[ItemName.HolyStaff] = new Weapon(ItemName.HolyStaff, 18, itemType, weaponRange, MagicManager.CreateWithNoMPCost(MagicName.Blaze2), Job.Vicar);
        _weaponsLookup[ItemName.DemonRod] = new Weapon(ItemName.DemonRod, 20, itemType, weaponRange, null /*"DRAINS MP"*/, Job.Wizard);
    }

    private static void BuildArrows()
    {
        var itemType = ItemType.Arrow;

        _weaponsLookup[ItemName.WoodenArrow] = new Weapon(ItemName.WoodenArrow, 8, itemType, new Range(2, 2), null, Job.Archer | Job.AssaultKnight);
        _weaponsLookup[ItemName.SteelArrow] = new Weapon(ItemName.SteelArrow, 13, itemType, new Range(2, 2), null, Job.Archer | Job.AssaultKnight);
        _weaponsLookup[ItemName.ElvenArrow] = new Weapon(ItemName.ElvenArrow, 18, itemType, new Range(2, 3), null, Job.Archer | Job.Sniper | Job.BowMaster | Job.AssaultKnight | Job.StrikeKnight);
        _weaponsLookup[ItemName.AssaultShell] = new Weapon(ItemName.AssaultShell, 27, itemType, new Range(2, 3), null, Job.StrikeKnight | Job.BowMaster | Job.Sniper);
        _weaponsLookup[ItemName.BusterShot] = new Weapon(ItemName.BusterShot, 35, itemType, new Range(2, 3), null, Job.StrikeKnight | Job.BowMaster | Job.Sniper);
    }

    private static void BuildSpears()
    {
        var itemType = ItemType.Spear;
        var weaponRange = new Range(1, 2);

        _weaponsLookup[ItemName.Spear] = new Weapon(ItemName.Spear, 8, itemType, weaponRange, null, Job.Knight | Job.SkyKnight);
        _weaponsLookup[ItemName.PowerSpear] = new Weapon(ItemName.PowerSpear, 8, itemType, weaponRange, null, Job.Knight | Job.SkyKnight);
    }

    private static void BuildLances()
    {
        var itemType = ItemType.Lance;
        var weaponRange = new Range(1, 1);

        _weaponsLookup[ItemName.BronzeLance] = new Weapon(ItemName.BronzeLance, 9, itemType, weaponRange, null, Job.Knight | Job.SkyKnight);
        _weaponsLookup[ItemName.SteelLance] = new Weapon(ItemName.SteelLance, 18, itemType, weaponRange, null, Job.Paladin | Job.SkyBaron | Job.SkyLord);
        _weaponsLookup[ItemName.ChromeLance] = new Weapon(ItemName.ChromeLance, 22, itemType, weaponRange, null, Job.Paladin | Job.SkyBaron | Job.SkyLord);
        _weaponsLookup[ItemName.Halberd] = new Weapon(ItemName.Halberd, 25, itemType, weaponRange, MagicManager.CreateWithNoMPCost(MagicName.Bolt1), Job.Paladin | Job.SkyBaron | Job.SkyLord);
        _weaponsLookup[ItemName.DevilLance] = new Weapon(ItemName.DevilLance, 35, itemType, weaponRange, null, Job.Paladin | Job.SkyBaron | Job.SkyLord, true);
        _weaponsLookup[ItemName.Valkyrie] = new Weapon(ItemName.Valkyrie, 35, itemType, weaponRange, null, Job.Paladin | Job.SkyBaron | Job.SkyLord);
    }
}