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

    // Constructor for creating inverted sprite
    private Sprite(Texture2D texture, FrameRect frameRect)
    {
        Texture = texture;
        FrameRect = frameRect;
    }

    public Sprite Invert()
    {
        // Create a copy of the image
        var image = Raylib.LoadImageFromTexture(Texture);
        
        // Invert the copy
        Raylib.ImageColorInvert(ref image);
        
        // Convert back to texture
        var invertedTexture = Raylib.LoadTextureFromImage(image);
        
        // Clean up
        Raylib.UnloadImage(image);

        return new Sprite(invertedTexture, FrameRect);
    }
}