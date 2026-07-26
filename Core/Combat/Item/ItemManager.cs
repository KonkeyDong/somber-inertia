using SomberInertia.Enums;
using SomberInertia.Core.Combat.Item.Weapon;

namespace SomberInertia.Core.Combat.Item;

public static class ItemManager
{
    private static readonly Dictionary<ItemName, Item> _itemsLookup = new();

    public static void Initialize()
    {
        _itemsLookup.Clear();

        _itemsLookup[ItemName.NoItem] = new Consumable(ItemName.NoItem, new Range(0, 0), 0, new HealEffect(0));

        // Weapons are already handled by WeaponManager
        // We only register non-weapon items here for now
        BuildConsumables();
        // BuildRings();
        // BuildKeyItems();
        // BuildClothes();
    }

    public static Item Create(ItemName itemName)
    {
        // First try the local lookup (consumables, etc.)
        if (_itemsLookup.TryGetValue(itemName, out var item))
        {
            return item.Clone();
        }

        Logger.Error($"ItemManager::Create(): Unknown item [{itemName}].");
        return new Consumable(ItemName.NoItem, new Range(0, 0), 0, new HealEffect(0));
    }

    private static void BuildConsumables()
    {
        _itemsLookup[ItemName.MedicalHerb] = new Consumable(ItemName.MedicalHerb, new Range(0, 1), 10, new HealEffect(8));
        _itemsLookup[ItemName.HealingSeed] = new Consumable(ItemName.HealingSeed, new Range(0, 1), 200, new HealEffect(16));
        _itemsLookup[ItemName.Antidote] = new Consumable(ItemName.Antidote, new Range(0, 1), 20, new RemovePoisonEffect());
        _itemsLookup[ItemName.AngelWing] = new Consumable(ItemName.AngelWing, new Range(0, 0), 40, new EscapeEffect());
    }
}