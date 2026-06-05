using SomberInertia;
using SomberInertia.Enums;

namespace SomberInertia.Graphics;

public static class MagicIcons
{
    private static readonly IconSet<MagicName> _icons =
        new IconSet<MagicName>(
            "Assets/Sprites/Shared/Magic Icons",
            GameConfig.Animations.IconDelay,
            getBaseName: icon => icon.GetBaseName());

    public static void Load() => _icons.Load();
    public static void Update() => _icons.Update();
    public static void SetSelectedSpell(MagicName spell) => _icons.SetSelected(spell);
    public static SpriteV2 GetSprite(MagicName spell) => _icons.GetSprite(spell);
    public static void Reset() => _icons.Reset();
}