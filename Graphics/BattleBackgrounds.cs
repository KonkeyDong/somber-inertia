using SomberInertia.Enums;

namespace SomberInertia.Graphics;

public static class BattleBackgrounds
{
    private static readonly BattlePlaneSet<BackgroundNames> _set =
        new BattlePlaneSet<BackgroundNames>(GameConstants.Paths.Backgrounds, "Battle backgrounds");

    public static void Load() => _set.Load();

    public static Sprite Get(BackgroundNames name) => _set.Get(name);
}
