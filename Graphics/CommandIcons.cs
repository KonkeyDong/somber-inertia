using SomberInertia;
using SomberInertia.Enums;

namespace SomberInertia.Graphics;

public static class CommandIcons
{
    private static readonly IconSet<CommandIconType> _icons =
        new IconSet<CommandIconType>(
            "Assets/Sprites/Shared/CommandIcons",
            GameConfig.Animations.BlinkDelay,
            getBaseName: icon => icon.GetBaseName());

    public static void Load() => _icons.Load();
    public static void Update() => _icons.Update();
    public static void SetSelectedIcon(CommandIconType type) => _icons.SetSelected(type);
    public static Sprite GetSprite(CommandIconType type) => _icons.GetSprite(type);
    public static void Reset() => _icons.Reset();
}