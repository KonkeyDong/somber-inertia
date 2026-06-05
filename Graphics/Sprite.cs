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
}