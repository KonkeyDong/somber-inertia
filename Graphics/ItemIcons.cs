using SomberInertia;
using SomberInertia.Enums;

namespace SomberInertia.Graphics;

public static class ItemIcons
{
    private static readonly IconSet<ItemName> _icons =
        new IconSet<ItemName>(
            GameConstants.Paths.ItemIcons,
            GameConstants.Animations.BlinkDelay,
            getBaseName: icon => icon.GetBaseName());

    public static void Load() => _icons.Load();
    public static void Tick() => _icons.Tick();

    /// Prefer <see cref="GetSprite(ItemName, bool)"/> for inventory radials so
    /// selection is per slot index, not per item name.
    public static void SetSelectedItem(ItemName itemName) => _icons.SetSelected(itemName);

    public static Sprite GetSprite(ItemName itemName) => _icons.GetSprite(itemName);

    public static Sprite GetSprite(ItemName itemName, bool isSelected) => _icons.GetSprite(itemName, isSelected);

    public static void Reset() => _icons.Reset();

    public static void ClearSelection() => _icons.SetSelected(ItemName.NoItem);
}