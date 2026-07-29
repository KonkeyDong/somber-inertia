using SomberInertia.Enums;

namespace SomberInertia.Graphics;

public static class BattleForegrounds
{
    private static Dictionary<ForegroundNames, Sprite> _foregroundMap { get; set; } = new();

    public static void Load()
    {
        // Logger.Error("BattleForegrounds.cs has not been implemented.");
        
        if (_foregroundMap.Count > 0)
        {
            Logger.Debug("Battle foregrounds frame data has already been loaded.");
            return;
        }

        var basePath = "Assets/Foregrounds";
        var jsonPath = Path.Combine(basePath, GameConstants.Files.FrameData);

        foreach (var name in Enum.GetValues<ForegroundNames>())
        {
            var pngPath = Path.Combine(basePath, $"{name}.png");

            foreach (var frame in SpriteManager.ExtractFrameData(jsonPath))
            {
                _foregroundMap[name] = new Sprite(pngPath, frame);
            }
        }

        Logger.Info("Battle foregounds have been loaded.");
    }

    public static Sprite Get(ForegroundNames name)
    {
        if (_foregroundMap.TryGetValue(name, out var sprite))
        {
            return sprite;
        }

        Logger.Error($"Could not find foreground sprite {name.ToString()}.");
        return new Sprite();
    }
}