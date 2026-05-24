using SomberInertia;
using System.Text.Json.Serialization;
using System.Numerics;

using Raylib_cs;

namespace SomberInertia.Graphics;

public class Sprite
{
    public Texture2D Texture { get; private set; }

    public int FrameWidth { get; private set; }
    public int FrameHeight { get; private set; }

    private int _currentFrame = 0;
    private int _currentRow = 0;

    private int _offsetWidth { get; set; }
    private int _offsetHeight { get; set; }

    public Sprite(string path, int frameWidth, int frameHeight)
    {
        Texture = SpriteManager.Load(path);
        FrameWidth = frameWidth;
        FrameHeight = frameHeight;

        _offsetWidth = 0;
        _offsetHeight = 0;

        Logger.Warning("<DEPRECATED CLASS> use SpriteV2 instead.");
    }

    public Sprite(string path, int frameWidth, int frameHeight, int offsetWidth, int offsetHeight)
    {
        Texture = SpriteManager.Load(path);
        FrameWidth = frameWidth;
        FrameHeight = frameHeight;

        _offsetWidth = offsetWidth;
        _offsetHeight = offsetHeight;

        Logger.Warning("<DEPRECATED CLASS> use SpriteV2 instead.");
    }


    public void SetFrame(int frame) => _currentFrame = frame;

    public void SetRow(int row)
    {
        if (_currentRow != row)
        {
            _currentRow = row;
            _currentFrame = 0;           // Reset frame when changing row
        }
    }

    public void Draw(Vector2 position, float scale)
    {
        var source = new Rectangle(
            x: _currentFrame * FrameWidth + _offsetWidth,
            y: _currentRow * FrameHeight + _offsetHeight * _currentRow,
            width: FrameWidth,
            height: FrameHeight
        );

        var dest = new Rectangle(
            x: position.X,
            y: position.Y,
            width: FrameWidth * scale,
            height: FrameHeight * scale
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

public class SpriteV2
{
    public Texture2D Texture { get; private set; }
    public FrameRect FrameRect { get; private set; }

    public SpriteV2(string path, FrameRect frameRect)
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