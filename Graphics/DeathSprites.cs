using SomberInertia.Enums;

namespace SomberInertia.Graphics;

public static class DeathSprites
{
    public static List<Sprite> Frames { get; private set; } = new();

    public static void Load()
    {
        if (Frames.Count > 0)
        {
            Logger.Debug("Death sprite frame data has already been loaded.");
            return;
        }

        var basePath = Path.Combine(GameConstants.Paths.Shared, GameConstants.Paths.Effects, Effects.BattleFieldDeath.GetBaseName(), GameConstants.Files.Effect);
        var pngPath = basePath + GameConstants.Files.PngExtension;
        var jsonPath = basePath + GameConstants.Files.JsonExtension;

        foreach (var frame in SpriteManager.ExtractFrameData(jsonPath))
        {
            Frames.Add(new Sprite(pngPath, frame));
        }

        Logger.Info("Death sprites have been loaded.");
    }
}