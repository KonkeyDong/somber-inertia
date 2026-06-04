using System.Collections.Generic;
using System.IO;
using SomberInertia.Enums;
using SomberInertia.Timers;
using SomberInertia.Core.Graphics;

namespace SomberInertia.Graphics;

public static class MagicIcons
{
    private static readonly FrameFlipper _frameFlipper = new FrameFlipper(GameConfig.Animations.IconDelay);
    private static readonly string _rootPath = "Assets/Sprites/Shared/Magic Icons";

    private static Dictionary<MagicName, List<SpriteV2>> _magicIconAnimations = new();
    private static MagicName _currentSelectedSpell = MagicName.Egress1;

    public static void Load()
    {
        _magicIconAnimations.Clear();

        var totalFramesLoaded = 0;
        var cache = new HashSet<string>();

        foreach (var magicName in Enum.GetValues<MagicName>())
        {
            _magicIconAnimations[magicName] = new List<SpriteV2>();

            var baseName = magicName.GetBaseName();
            var basePath = Path.Combine(_rootPath, baseName);

            var jsonPath = Path.Combine(basePath + ".json");
            var pngPath  = Path.Combine(basePath + ".png");

            if (cache.Contains(jsonPath))
            {
                continue;
            }

            cache.Add(jsonPath);

            var frames = SpriteManager.ExtractFrameData(jsonPath);

            foreach (var frame in frames)
            {
                _magicIconAnimations[magicName].Add(new SpriteV2(pngPath, frame));
                totalFramesLoaded++;
            }
        }

        Logger.Info($"MagicIcons.Load() completed. Loaded [{totalFramesLoaded}] frames.");
    }

    public static void SetSelectedSpell(MagicName magicName)
    {
        _currentSelectedSpell = magicName;
        Reset();
    }

    public static void Update() => _frameFlipper.Tick();

    public static SpriteV2 GetSprite(MagicName magicName)
    {
        if (!_magicIconAnimations.ContainsKey(magicName))
        {
            Logger.Error($"Magic icon for [{magicName}] not found.");
            return null;
        }

        var frame = 0;

        if (magicName == _currentSelectedSpell)
        {
            frame = _frameFlipper.IsOn ? 1 : 0;
        }

        return _magicIconAnimations[magicName][frame];
    }

    public static void Reset() => _frameFlipper.Reset();
}