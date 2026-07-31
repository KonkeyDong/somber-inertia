using SomberInertia.Enums;

namespace SomberInertia.Graphics;

/// <summary>
/// Battle plane sprites (background / foreground): one sprite per enum name from a
/// folder, sharing a FrameData.json source rect plus <c>{Name}.png</c> per key.
/// </summary>
public class BattlePlaneSet<TKey> where TKey : struct, Enum
{
    private readonly string _basePath;
    private readonly string _label;
    private readonly Dictionary<TKey, Sprite> _map = new();

    public BattlePlaneSet(string basePath, string label)
    {
        _basePath = basePath;
        _label = label;
    }

    public void Load()
    {
        if (_map.Count > 0)
        {
            Logger.Debug($"{_label} have already been loaded.");
            return;
        }

        var jsonPath = Path.Combine(_basePath, GameConstants.Files.FrameData);
        var frames = SpriteManager.ExtractFrameData(jsonPath);

        if (frames.Count == 0)
        {
            Logger.Error($"{_label}: no frames in {jsonPath}.");
            return;
        }

        var sourceRect = frames[0];

        foreach (var name in Enum.GetValues<TKey>())
        {
            var pngPath = Path.Combine(_basePath, $"{name}.png");
            _map[name] = new Sprite(pngPath, sourceRect);
        }

        Logger.Info($"{_label} have been loaded ({_map.Count}).");
    }

    public Sprite Get(TKey name)
    {
        if (_map.TryGetValue(name, out var sprite))
        {
            return sprite;
        }

        Logger.Error($"Could not find {_label} sprite {name}.");
        return new Sprite();
    }
}
