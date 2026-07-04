namespace SomberInertia.Graphics;

public static class BattleForegrounds
{
    public static List<Sprite> Frames { get; private set; } = new();

    public static void Load()
    {
        Logger.Error("BattleForegrounds.cs has not been implemented.");
        
        if (Frames.Count > 0)
        {
            Logger.Debug("Battle foregrounds frame data has already been loaded.");
            return;
        }

        var basePath = "Assets/Foregrounds";
        var pngPath = Path.Combine(basePath, "Foregrounds.png");
        var jsonPath = Path.Combine(basePath, GameConstants.BATTLE_BACKGROUND_FRAME_DATA_FILE_NAME);

        foreach (var frame in SpriteManager.ExtractFrameData(jsonPath))
        {
            Frames.Add(new Sprite(pngPath, frame));
        }

        Logger.Info("Battle foregounds have been loaded.");
    }
}