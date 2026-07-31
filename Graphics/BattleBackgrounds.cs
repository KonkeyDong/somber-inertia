using SomberInertia.Enums;

namespace SomberInertia.Graphics;

public static class BattleBackgrounds
{
    // public static List<Sprite> Frames { get; private set; } = new();
    private static Dictionary<BackgroundNames, Sprite> _backgroundMap { get; set; } = new();

    public static void Load()
    {
        if (_backgroundMap.Count > 0)
        {
            Logger.Debug("Battle backgrounds frame data has already been loaded.");
            return;
        }

        var basePath = "Assets/Backgrounds";
        var jsonPath = Path.Combine(basePath, GameConstants.Files.FrameData);

        foreach (var name in Enum.GetValues<BackgroundNames>())
        {
        var pngPath = Path.Combine(basePath, $"{name}.png");

            foreach (var frame in SpriteManager.ExtractFrameData(jsonPath))
            {
                _backgroundMap[name] = new Sprite(pngPath, frame);
            }
        }

        Logger.Info("Battle backgrounds have been loaded.");
    }

    public static Sprite Get(BackgroundNames name)
    {
        if (_backgroundMap.TryGetValue(name, out var sprite))
        {
            return sprite;
        }

        Logger.Error($"Could not find background sprite {name.ToString()}.");
        return new Sprite();
    }
}