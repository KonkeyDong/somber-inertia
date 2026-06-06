using System;
using System.Collections.Generic;
using System.IO;
using Raylib_cs;
using SomberInertia.Enums;
using SomberInertia.Timers;
using SomberInertia.Core.Graphics;

namespace SomberInertia.Graphics;

public interface IIconSet<TKey> where TKey : Enum
{
    void Load();
    void Update();
    void SetSelected(TKey key);
    Sprite GetSprite(TKey key);
    void Reset();
}

public class IconSet<TKey> : IIconSet<TKey> where TKey : Enum
{
    private readonly FrameFlipper _frameFlipper;
    private readonly string _rootPath;
    private readonly Dictionary<TKey, List<Sprite>> _animations = new();
    private TKey _selectedKey = default!;
    private readonly Func<TKey, string> _getBaseName;   // ← New

    public IconSet(string rootPath, int animationDelay, Func<TKey, string>? getBaseName = null)
    {
        _rootPath = rootPath;
        _frameFlipper = new FrameFlipper(animationDelay);

        // If no custom function is provided, use default behavior
        _getBaseName = getBaseName ?? (key => key.ToString().ToLowerInvariant());
    }

    public void Load()
    {
        _animations.Clear();
        var totalFrames = 0;

        var jsonPath = Path.Combine(_rootPath, GameConstants.FRAME_DATA_FILE_NAME);
        var frames = SpriteManager.ExtractFrameData(jsonPath);

        foreach (var key in Enum.GetValues(typeof(TKey)))
        {
            var enumValue = (TKey)key;
            _animations[enumValue] = new List<Sprite>();

            var baseName = _getBaseName(enumValue);
            var basePath = Path.Combine(_rootPath, baseName);
            var pngPath  = Path.Combine(basePath + ".png");

            foreach (var frame in frames)
            {
                _animations[enumValue].Add(new Sprite(pngPath, frame));
                totalFrames++;
            }
        }

        Logger.Info($"IconSet<{typeof(TKey).Name}>.Load() completed. Loaded {totalFrames} frames.");
    }

    public void SetSelected(TKey key)
    {
        _selectedKey = key;
        Reset();
    }

    public void Update() => _frameFlipper.Tick();

    public Sprite GetSprite(TKey key)
    {
        if (!_animations.ContainsKey(key))
        {
            Logger.Error($"Icon for [{key}] not found in IconSet.");
            return null;
        }

        var frame = 0;

        if (EqualityComparer<TKey>.Default.Equals(key, _selectedKey))
        {
            frame = _frameFlipper.IsOn ? 1 : 0;
        }

        return _animations[key][frame];
    }

    public void Reset() => _frameFlipper.Reset();
}