using SomberInertia.Enums;

namespace SomberInertia.Core.Combat.Item;

public static class ItemDatabase
{
    private static readonly Dictionary<ItemName, ItemData> _items = new();

    public static void Initialize()
    {
        _items.Clear();

        RegisterNoItem();
        RegisterUnarmed();
        RegisterSwords();
        RegisterAxes();
        RegisterStaves();
        RegisterArrows();
        RegisterSpears();
        RegisterLances();
        RegisterConsumables();
    }

    public static ItemData Get(ItemName name)
    {
        if (_items.TryGetValue(name, out var data))
        {
            return data;
        }

        Logger.Error($"ItemDatabase.Get(): Unknown item [{name}]. Returning NoItem.");
        return _items[ItemName.NoItem];
    }

    private static void Register(ItemData data)
    {
        _items[data.Name] = data;
    }

    private static void RegisterNoItem()
    {
        Register(new ItemData
        {
            Name = ItemName.NoItem,
            Type = ItemType.Consumable,
            Price = 0,
            Attack = 0,
            DistanceRange = new Range(0, 0),
            AllowedJobs = Job.Any,
            Cursed = false,
            EffectType = ItemEffectType.None,
            EffectValue = 0,
            SpellName = MagicName.NoSpell
        });
    }

    private static void RegisterUnarmed()
    {
        Register(new ItemData
        {
            Name = ItemName.Unarmed,
            Type = ItemType.Unarmed,
            Price = 0,
            Attack = 0,
            DistanceRange = new Range(1, 1),
            AllowedJobs = Job.Any,
            Cursed = false,
            EffectType = ItemEffectType.None,
            EffectValue = 0,
            SpellName = MagicName.NoSpell
        });
    }

    private static void RegisterSwords()
    {
        var type = ItemType.Sword;
        var range = new Range(1, 1);

        Register(MakeWeapon(ItemName.ShortSword, 5, type, range, Job.Swordsman | Job.Warrior | Job.Birdman, 100));
        Register(MakeWeapon(ItemName.MiddleSword, 8, type, range, Job.Swordsman | Job.Warrior | Job.Birdman, 250));
        Register(MakeWeapon(ItemName.LongSword, 12, type, range, Job.Warrior | Job.Swordsman, 750));
        Register(MakeWeapon(ItemName.SteelSword, 18, type, range, Job.Hero | Job.Ninja | Job.SkyWarrior | Job.Samurai, 2500));
        Register(MakeWeapon(ItemName.BroadSword, 20, type, range, Job.Hero | Job.Ninja | Job.SkyWarrior | Job.Samurai, 4800));
        Register(MakeWeapon(ItemName.DoomBlade, 25, type, range, Job.Hero | Job.Ninja | Job.SkyWarrior | Job.Samurai, 0));
        Register(MakeWeapon(ItemName.Katana, 30, type, range, Job.Hero | Job.Ninja | Job.SkyWarrior | Job.Samurai, 0));
        Register(MakeWeapon(ItemName.SwordOfLight, 36, type, range, Job.Hero | Job.SkyWarrior, 0, MagicName.Bolt2));
        Register(MakeWeapon(ItemName.SwordOfDarkness, 40, type, range, Job.Hero | Job.SkyWarrior, 0, MagicName.Desoul1, cursed: true));
        Register(MakeWeapon(ItemName.ChaosBreaker, 40, type, range, Job.Hero | Job.SkyWarrior, 0, MagicName.Freeze3));
    }

    private static void RegisterAxes()
    {
        var type = ItemType.Axe;
        var range = new Range(1, 1);

        Register(MakeWeapon(ItemName.HandAxe, 7, type, range, Job.Warrior, 200));
        Register(MakeWeapon(ItemName.MiddleAxe, 11, type, range, Job.Warrior, 600));
        Register(MakeWeapon(ItemName.BattleAxe, 16, type, range, Job.Warrior, 2600));
        Register(MakeWeapon(ItemName.HeatAxe, 22, type, range, Job.Gladiator, 0, MagicName.Blaze2));
        Register(MakeWeapon(ItemName.GreatAxe, 26, type, range, Job.Gladiator, 10000));
        Register(MakeWeapon(ItemName.Atlas, 33, type, range, Job.Gladiator, 0, MagicName.Blaze3));
    }

    private static void RegisterStaves()
    {
        var type = ItemType.Staff;
        var range = new Range(1, 1);

        Register(MakeWeapon(ItemName.WoodenStaff, 5, type, range, Job.Healer | Job.Mage, 80));
        Register(MakeWeapon(ItemName.PowerStaff, 8, type, range, Job.Healer | Job.Mage, 500));
        Register(MakeWeapon(ItemName.GuardianStaff, 12, type, range, Job.Vicar | Job.Wizard, 3200));
        Register(MakeWeapon(ItemName.HolyStaff, 18, type, range, Job.Vicar, 8000, MagicName.Blaze2));
        Register(MakeWeapon(ItemName.DemonRod, 20, type, range, Job.Wizard, 0));
    }

    private static void RegisterArrows()
    {
        var type = ItemType.Arrow;

        Register(MakeWeapon(ItemName.WoodenArrow, 8, type, new Range(2, 2), Job.Archer | Job.AssaultKnight, 320));
        Register(MakeWeapon(ItemName.SteelArrow, 13, type, new Range(2, 2), Job.Archer | Job.AssaultKnight, 1200));
        Register(MakeWeapon(ItemName.ElvenArrow, 18, type, new Range(2, 3), Job.Archer | Job.Sniper | Job.BowMaster | Job.AssaultKnight | Job.StrikeKnight, 3200));
        Register(MakeWeapon(ItemName.AssaultShell, 27, type, new Range(2, 3), Job.StrikeKnight | Job.BowMaster | Job.Sniper, 4500));
        Register(MakeWeapon(ItemName.BusterShot, 35, type, new Range(2, 3), Job.StrikeKnight | Job.BowMaster | Job.Sniper, 12400));
    }

    private static void RegisterSpears()
    {
        var type = ItemType.Spear;
        var range = new Range(1, 2);

        Register(MakeWeapon(ItemName.Spear, 8, type, range, Job.Knight | Job.SkyKnight, 150));
        Register(MakeWeapon(ItemName.PowerSpear, 8, type, range, Job.Knight | Job.SkyKnight, 900));
    }

    private static void RegisterLances()
    {
        var type = ItemType.Lance;
        var range = new Range(1, 1);

        Register(MakeWeapon(ItemName.BronzeLance, 9, type, range, Job.Knight | Job.SkyKnight, 300));
        Register(MakeWeapon(ItemName.SteelLance, 18, type, range, Job.Paladin | Job.SkyBaron | Job.SkyLord, 3000));
        Register(MakeWeapon(ItemName.ChromeLance, 22, type, range, Job.Paladin | Job.SkyBaron | Job.SkyLord, 4500));
        Register(MakeWeapon(ItemName.Halberd, 25, type, range, Job.Paladin | Job.SkyBaron | Job.SkyLord, 0, MagicName.Bolt1));
        Register(MakeWeapon(ItemName.DevilLance, 35, type, range, Job.Paladin | Job.SkyBaron | Job.SkyLord, 0, MagicName.NoSpell, cursed: true));
        Register(MakeWeapon(ItemName.Valkyrie, 35, type, range, Job.Paladin | Job.SkyBaron | Job.SkyLord, 0));
    }

    private static void RegisterConsumables()
    {
        // Consumables: AllowedJobs = Job.Any (any unit job may Use). Target friendlies in later states.
        Register(MakeConsumable(ItemName.MedicalHerb, new Range(0, 1), 10, ItemEffectType.Heal, 10));
        Register(MakeConsumable(ItemName.HealingSeed, new Range(0, 1), 200, ItemEffectType.Heal, 20));
        Register(MakeConsumable(ItemName.Antidote, new Range(0, 1), 20, ItemEffectType.RemovePoison, 0));
        Register(MakeConsumable(ItemName.AngelWing, new Range(0, 0), 40, ItemEffectType.Escape, 0));
    }

    private static ItemData MakeWeapon(
        ItemName name,
        int attack,
        ItemType type,
        Range range,
        Job jobs,
        int price,
        MagicName spellName = MagicName.NoSpell,
        bool cursed = false)
    {
        return new ItemData
        {
            Name = name,
            Type = type,
            Price = price,
            Attack = attack,
            DistanceRange = range,
            AllowedJobs = jobs,
            Cursed = cursed,
            EffectType = ItemEffectType.None,
            EffectValue = 0,
            SpellName = spellName
        };
    }

    private static ItemData MakeConsumable(
        ItemName name,
        Range range,
        int price,
        ItemEffectType effectType,
        int effectValue)
    {
        return new ItemData
        {
            Name = name,
            Type = ItemType.Consumable,
            Price = price,
            Attack = 0,
            DistanceRange = range,
            AllowedJobs = Job.Any,
            Cursed = false,
            EffectType = effectType,
            EffectValue = effectValue,
            SpellName = MagicName.NoSpell
        };
    }
}