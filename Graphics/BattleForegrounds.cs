using SomberInertia.Enums;

namespace SomberInertia.Graphics;

public static class BattleForegrounds
{
    private static readonly BattlePlaneSet<ForegroundNames> _set =
        new BattlePlaneSet<ForegroundNames>("Assets/Foregrounds", "Battle foregrounds");

    public static void Load() => _set.Load();

    public static Sprite Get(ForegroundNames name) => _set.Get(name);
}
