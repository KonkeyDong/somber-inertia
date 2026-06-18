using SomberInertia;
using SomberInertia.Enums;

namespace SomberInertia.Graphics;

public static class MagicIcons
{
    private static readonly IconSet<MagicFamily> _icons =
        new IconSet<MagicFamily>(
            "Assets/Sprites/Shared/MagicIcons",
            GameConfig.Animations.BlinkDelay,
            getBaseName: icon => icon.GetBaseName());

    public static void Load() => _icons.Load();
    public static void Update() => _icons.Update();
    public static void SetSelectedSpell(MagicFamily family) => _icons.SetSelected(family);
    public static Sprite GetSprite(MagicFamily family) => _icons.GetSprite(family);
    public static void Reset() => _icons.Reset();
}