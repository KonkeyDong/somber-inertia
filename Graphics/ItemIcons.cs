using SomberInertia;
using SomberInertia.Enums;

namespace SomberInertia.Graphics;

public static class ItemIcons
{
    private static readonly IconSet<ItemName> _icons =
        new IconSet<ItemName>(
            "Assets/Sprites/Shared/ItemIcons",
            GameConstants.Animations.BlinkDelay,
            getBaseName: icon => icon.GetBaseName());

    public static void Load() => _icons.Load();
    public static void Tick() => _icons.Tick();
    public static void SetSelectedSpell(ItemName itemName) => _icons.SetSelected(itemName);
    public static Sprite GetSprite(ItemName itemName) => _icons.GetSprite(itemName);
    public static void Reset() => _icons.Reset();
}