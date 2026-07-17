namespace SomberInertia.Graphics;

public static class BattleBackgrounds
{
    public static List<Sprite> Frames { get; private set; } = new();

    public static void Load()
    {
        if (Frames.Count > 0)
        {
            Logger.Debug("Battle backgrounds frame data has already been loaded.");
            return;
        }


        var basePath = "Assets/Backgrounds";
        var pngPath = Path.Combine(basePath, "BattleBackground01.png");
        var jsonPath = Path.Combine(basePath, GameConstants.Files.BATTLE_BACKGROUND_FRAME_DATA_FILE_NAME);

        foreach (var frame in SpriteManager.ExtractFrameData(jsonPath))
        {
            Frames.Add(new Sprite(pngPath, frame));
        }

        Logger.Info("Battle backgrounds have been loaded.");
    }
}