using SomberInertia.Enums;

namespace SomberInertia.Graphics;

public static class ArtilleryExplosion
{
    public static List<Sprite> Frames { get; private set; } = new();

    public static void Load()
    {
        if (Frames.Count > 0)
        {
            Logger.Debug("'Artillery Explosion' sprite frame data has already been loaded.");
            return;
        }

        var basePath = Path.Combine(GameConstants.Paths.Shared, GameConstants.Paths.Effects, Effects.ArtilleryExplosion.GetBaseName(), GameConstants.Files.Effect);
        var pngPath = basePath + GameConstants.Files.PngExtension;
        var jsonPath = basePath + GameConstants.Files.JsonExtension;

        foreach (var frame in SpriteManager.ExtractFrameData(jsonPath))
        {
            Frames.Add(new Sprite(pngPath, frame));
        }

        Logger.Info("'Artillery Explosion' sprites have been loaded.");
    }
}