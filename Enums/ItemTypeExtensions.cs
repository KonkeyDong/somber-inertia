namespace SomberInertia.Enums;

public static class ItemTypeExtensions
{
    public static bool IsWeapon(this ItemType type)
    {
        return type is ItemType.Unarmed
            or ItemType.Sword
            or ItemType.Axe
            or ItemType.Staff
            or ItemType.Arrow
            or ItemType.Spear
            or ItemType.Lance;
    }

    public static bool IsEquippable(this ItemType type)
    {
        return type is ItemType.Unarmed
            or ItemType.Sword
            or ItemType.Axe
            or ItemType.Staff
            or ItemType.Arrow
            or ItemType.Spear
            or ItemType.Lance
            or ItemType.Ring;
    }

    public static bool IsDroppable(this ItemType type)
    {
        return type is ItemType.Sword
            or ItemType.Axe
            or ItemType.Staff
            or ItemType.Arrow
            or ItemType.Spear
            or ItemType.Lance
            or ItemType.Consumable
            or ItemType.Ring;
    }

    public static bool IsConsumable(this ItemType type)
    {
        return type is ItemType.Consumable;
    }

    public static bool IsStoryItem(this ItemType type)
    {
        return type is ItemType.Story;
    }

    public static bool IsClothes(this ItemType type)
    {
        return type is ItemType.Clothes;
    }
}