using SomberInertia;
using System.Text.Json.Serialization;
using System.Numerics;

using Raylib_cs;

namespace SomberInertia.Graphics;

public class Sprite
{
    public Texture2D Texture { get; private set; }
    public FrameRect FrameRect { get; private set; }

    public Sprite(string path, FrameRect frameRect)
    {
        Texture = SpriteManager.Load(path);
        FrameRect = frameRect;
    }

    public void Draw(Vector2 position, float scale)
    {
        var source = new Rectangle(
            x: FrameRect.x,
            y: FrameRect.y,
            width: FrameRect.w,
            height: FrameRect.h
        );

        var dest = new Rectangle(
            x: position.X,
            y: position.Y,
            width: FrameRect.w * scale,
            height: FrameRect.h * scale
        );

        Raylib.DrawTexturePro(
            Texture,
            source,
            dest,
            GameConfig.Textures.BaseOrigin,
            GameConfig.Textures.BaseRotation,
            GameConfig.Textures.ClearColor
        );
    }
}