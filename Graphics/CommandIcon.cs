using System.Numerics;
using Raylib_cs;
using Timer = SomberInertia.Timers;
using SomberInertia.Enums;

namespace SomberInertia.Graphics;

public class CommandIcon
{
    public readonly Sprite Sprite;
    private readonly Timer.CommandIcon Animator;

    private const string SpriteSheetPath = "Assets/commands.png";

    public CommandIcon(CommandIconType type)      // Use enum instead of raw int
    {
        var offset = (int)type;
        Sprite = new Sprite(SpriteSheetPath, 
                           GameConstants.WORLD_MAP_SPRITE_SIZE + 1, 
                           GameConstants.WORLD_MAP_SPRITE_SIZE + 1);

        Animator = new Timer.CommandIcon(6);
        
        Sprite.SetRow((int)type);   // Set initial row based on enum
    }

    public void Update()
    {
        Animator.Tick();
        Sprite.SetFrame(Animator.GetCurrentFrameIndex());
    }

    public void Draw(Vector2 position, float scale) => Sprite.Draw(position, scale);

    public void Reset()
    {
        Animator.Reset();
        Sprite.SetFrame(0);
    }
}