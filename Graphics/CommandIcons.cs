using System.Numerics;
using Raylib_cs;
using SomberInertia.Enums;
using SomberInertia.Timers;
using SomberInertia.Core.Graphics;

namespace SomberInertia.Graphics;

public static class CommandIcons
{
    private static readonly FrameFlipper _frameFlipper = new FrameFlipper(GameConfig.Animations.IconDelay);
    private static readonly string _rootPath = "Assets/Sprites/Shared/Command Icons";
    private static Dictionary<CommandIconType, List<SpriteV2>> _commandIconAnimations = new();
    private static CommandIconType _currentSelectedIcon = CommandIconType.Yes;

    public static void Load()
    {
        _commandIconAnimations.Clear();

        var totalFramesLoaded = 0;

        foreach (var commandIconType in Enum.GetValues<CommandIconType>())
        {
            _commandIconAnimations[commandIconType] = new List<SpriteV2>();

            var basePath = Path.Combine(_rootPath, commandIconType.ToString().ToLower());
            var jsonPath = Path.Combine(basePath + ".json");
            var pngPath = Path.Combine(basePath + ".png");

            var frames = SpriteManager.ExtractFrameData(jsonPath);

            foreach (var frame in frames)
            {
                _commandIconAnimations[commandIconType].Add(new SpriteV2(pngPath, frame));
                totalFramesLoaded++;
            }
        }

        Logger.Info($"CommandIcon.Load() completed. Loaded [{totalFramesLoaded}] frames.");
    }

    public static void SetSelectedIcon(CommandIconType commandIconType)
    {
        _currentSelectedIcon = commandIconType;
        Reset();
    }

    public static void Update() =>  _frameFlipper.Tick();

    public static SpriteV2 GetSprite(CommandIconType commandIconType) 
    {
        if (!_commandIconAnimations.ContainsKey(commandIconType))
        {
            Logger.Error($"command icon type [{commandIconType.ToString().ToLower()}] not found in dictionary.");
        }

        var frame = 0;
        if (commandIconType == _currentSelectedIcon)
        {
            frame = _frameFlipper.IsOn ? 1 : 0;
        }

        return _commandIconAnimations[commandIconType][frame];
    } 
        
    public static void Reset() => _frameFlipper.Reset();
}