using SomberInertia;
using System.Text.Json.Serialization;
using System.Numerics;

using Raylib_cs;

namespace SomberInertia.Graphics;

public class Sprite
{
    public Texture2D Texture { get; private set; }
    public FrameRect FrameRect { get; private set; }

    private static readonly Random _random = new Random();

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

    public Sprite Jitter()
    {
        Logger.Warning("Jitter(): make jitter offset amount a constant.");
        var jittered = new Sprite(Texture, FrameRect.Copy());

        var jitterOffset = GameConstants.Animations.JitterOffset;

        jittered.FrameRect.OffsetX += (_random.Next(0, 2) * (jitterOffset * 2) - jitterOffset);
        jittered.FrameRect.OffsetY += (_random.Next(0, 2) * (jitterOffset * 2) - jitterOffset);

        return jittered;
    }

    public Sprite Dissolve(int clearAmount)
    {
        // 1. Load the original texture as an editable Image
        var image = Raylib.LoadImageFromTexture(Texture);

        var spriteWidth = FrameRect.W;
        var spriteHeight = FrameRect.H;
        var groupSize = GameConstants.Animations.Dissolve.GroupSize;

        var totalPixels = spriteWidth * spriteHeight;
        // var pixelsToClear = (int)(totalPixels / clearAmount); // or adjust as needed

        // var cleared = 0;

        var pattern = new bool[groupSize];
        for (var i = 0; i < groupSize; i++)
        {
            pattern[i] = i < clearAmount;
        }

        for (var y = 0; y < spriteHeight; y++)
        {
            for (var x = 0; x < spriteWidth; x++)
            {
                var groupIndex = (y * spriteWidth + x) % groupSize;

                if (pattern[groupIndex]) // clear first half of each group
                {
                    unsafe
                    {
                        Raylib.ImageDrawPixel(&image, x + FrameRect.X, (y + FrameRect.Y), Color.Blank);
                    }
                    // cleared++;
                }
            }
        }

        // 2. Convert the modified image back to a texture
        var newTexture = Raylib.LoadTextureFromImage(image);

        // 3. Clean up the image
        Raylib.UnloadImage(image);

        // 4. Return a new sprite with the destroyed texture
        return new Sprite(newTexture, FrameRect);
    }
}