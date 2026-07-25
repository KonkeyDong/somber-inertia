using SomberInertia.Enums;
using SomberInertia.Core.Combat.Item.Weapon;
using SomberInertia.Core.Combat.Items;

namespace SomberInertia.Core.Combat.Item;

public static class ItemManager
{
    private static readonly Dictionary<ItemName, Item> _itemsLookup = new();

    public static void Initialize()
    {
        _itemsLookup.Clear();

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
            return item;
        }

        // Fall back to weapons
        try
        {
            return WeaponManager.Create(itemName);
        }
        catch
        {
            throw new InvalidOperationException($"ItemManager::Create(): Unknown item [{itemName}].");
        }
    }

    private static void BuildConsumables()
    {
        // Example – we’ll flesh these out next
        // _itemsLookup[ItemName.MedicalHerb] = new Consumable(...);
        // _itemsLookup[ItemName.HealingSeed] = new Consumable(...);
        // _itemsLookup[ItemName.Antidoe] = new Consumable(...);
        // _itemsLookup[ItemName.AngelWing] = new Consumable(...);
    }
}