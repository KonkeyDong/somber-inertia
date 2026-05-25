namespace SomberInertia.Graphics;

public static class DeathSprites
{
    public static List<SpriteV2> Frames { get; private set; } = new();

    public static void Load()
    {
        if (Frames.Count > 0)
        {
            Logger.Debug("Death sprite frame data has already been loaded.");
            return;
        }

        var basePath = "Assets/Sprites/Shared/battle_field_death";
        var pngPath = Path.Combine(basePath + ".png");
        var jsonPath = Path.Combine(basePath + ".json");

        foreach (var frame in SpriteManager.ExtractFrameData(jsonPath))
        {
            Frames.Add(new SpriteV2(pngPath, frame));
        }

        Logger.Info("Death sprites have been loaded.");
    }
}